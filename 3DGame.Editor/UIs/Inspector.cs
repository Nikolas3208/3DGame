using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Resources.Assets;
using _3DGame.Core.Utils;
using ImGuiNET;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace _3DGame.Editor.UIs
{
    public enum SelectedObjectType
    {
        None,
        GameObject,
        Material,
        Texture
    }
    public class Inspector
    {
        private TextureType selectedTextureType = TextureType.Diffuse;

        public SelectedObjectType SelectedObjectType { get; private set; } = SelectedObjectType.None;

        public Scene scene;
        public GameObject? SelectedGameObject { get; private set; }
        public Material? SelectedMaterial { get; private set; } = new Material("Test");
        public Texture? SelectedTexture { get; private set; }
        
        public Inspector(Scene scene)
        {
            this.scene = scene;
        }

        public void SetSelectedGameObject(GameObject gameObject)
        {
            SelectedGameObject = gameObject;
            SelectedObjectType = SelectedObjectType.GameObject;
        }

        public void SetSelectedMaterial(Material? material)
        {
            SelectedMaterial = material;
            SelectedObjectType = SelectedObjectType.Material;
        }

        public void SetSelectedTexture(Texture? texture)
        {
            SelectedTexture = texture;
            SelectedObjectType = SelectedObjectType.Texture;
        }

        public void Draw()
        {
            ImGui.Begin("Inspector");

            switch (SelectedObjectType)
            {
                case SelectedObjectType.None:
                    ImGui.Text("Nothing selected");
                    break;
                case SelectedObjectType.GameObject:
                    DrawGameObjectInspector(SelectedGameObject);
                    break;
                case SelectedObjectType.Material:
                    DrawMaterialInspector(SelectedMaterial);
                    break;
                case SelectedObjectType.Texture:
                    DrawTextureInspector(SelectedTexture);
                    break;
            }
            ImGui.End();
        }

        private void DrawGameObjectInspector(GameObject? selectedGameObject)
        {
            if(selectedGameObject == null)
            {
                ImGui.Text("No GameObject selected");
                return;
            }

            ImGui.Text("Name: ");

            ImGui.SameLine();

            ImGui.InputText("##gameObject_name", ref selectedGameObject.Name, 64);

            ImGui.Spacing();

            foreach (var component in selectedGameObject.components)
            {
                if(ImGui.TreeNodeEx(component.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (component is Transform transform)
                    {
                        var position = transform.Position;
                        var rotation = transform.Rotation;
                        var scale = transform.Scale;

                        ImGuiImpl.DragLabeledFloat3("Position", ref position, 0.01f);
                        ImGui.Spacing();
                        ImGuiImpl.DragLabeledFloat3("Rotation", ref rotation, 0.01f);
                        ImGui.Spacing();
                        ImGuiImpl.DragLabeledFloat3("Scale", ref scale, 0.01f);
                        ImGui.Spacing();

                        transform.Position = position;
                        transform.Rotation = rotation;
                        transform.Scale = scale;
                    }
                    else if (component is Light light)
                    {
                        var lightType = (int)light.Type;

                        ImGui.Text("Type: ");

                        ImGui.SameLine();

                        if (ImGui.Combo("##light_type", ref lightType, "Directional\0Point\0Spot\0"))
                            light.Type = (LightType)lightType;

                        var color = light.Color;

                        ImGui.Spacing();

                        ImGui.Text("Color: ");

                        ImGui.SameLine();

                        if (ImGuiImpl.ColorEdit4("##light_color", ref color))
                            light.Color = color;

                        ImGui.Spacing();

                        if (lightType > 0)
                        {
                            var linear = light.Linear;
                            var constant = light.Constant;
                            var quadratic = light.Quadratic;

                            ImGui.Text("Linear: ");

                            ImGui.SameLine();

                            if (ImGui.SliderFloat("##light_linear", ref linear, 0.1f, 1.0f, "%.2f"))
                                light.Linear = linear;

                            ImGui.Spacing();
                            ImGui.Text("Constant: ");

                            ImGui.SameLine();

                            if (ImGui.SliderFloat("##light_constant", ref constant, 0.1f, 1.0f, "%.2f"))
                                light.Constant = constant;

                            ImGui.Spacing();
                            ImGui.Text("Quadratic: ");

                            ImGui.SameLine();

                            if (ImGui.SliderFloat("##light_quadratic", ref quadratic, 0.1f, 1.0f, "%.2f"))
                                light.Quadratic = quadratic;

                            ImGui.Spacing();
                        }

                        if (lightType > 1)
                        {
                            var cutOff = light.CutOff;
                            var outerCutOff = light.OuterCutOff;

                            ImGui.Text("Cut Off: ");

                            ImGui.SameLine();

                            if (ImGui.SliderFloat("##light_cutoff", ref cutOff, 0.1f, light.OuterCutOff - 1, "%.2f"))
                                light.CutOff = cutOff;

                            ImGui.Spacing();

                            ImGui.Text("Outer Cut Off: ");

                            ImGui.SameLine();

                            if (ImGui.SliderFloat("##light_outer_cutoff", ref outerCutOff, 0.1f, 90.0f, "%.2f"))
                                light.OuterCutOff = outerCutOff;

                            ImGui.Spacing();
                        }
                    }
                    else if (component is MeshRender meshRender)
                    {
                        if (ImGui.ArrowButton("##mesh_render_mesh", ImGuiDir.Right))
                        {
                            ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                            ImGui.OpenPopup("MeshRenderPopup");
                        }

                        ImGui.SameLine();

                        ImGui.Text("Mesh: ");

                        if (meshRender.GetMesh() != null)
                        {
                            ImGui.SameLine();
                            ImGui.Text(meshRender.GetMesh()!.Name);
                        }
                        else
                        {
                            ImGui.SameLine();
                            ImGui.Text("No mesh assigned");
                        }

                        ImGui.Spacing();

                        var material = meshRender.GetMaterial();

                        if (material != null)
                        {
                            ImGui.Text("Material: ");
                            ImGui.SameLine();
                            ImGui.Text(material.Name);
                            ImGui.SameLine();
                            if(ImGui.SmallButton("Edit"))
                            {
                                SetSelectedMaterial(material);
                            }
                        }
                    }


                    ImGui.Separator();

                    ImGui.TreePop();
                }
            }
        }

        private void DrawTextureInspector(Texture? selectedTexture)
        {
            if(selectedTexture == null)
            {
                ImGui.Text("No Texture selected");
                return;
            }
        }

        private void DrawMaterialInspector(Material? selectedMaterial)
        {
            if(selectedMaterial == null)
            {
                ImGui.Text("No Material selected");
                return;
            }

            ImGui.Text($"Material: {selectedMaterial.Name}");

            var diffuse = selectedMaterial.Diffuse;
            var specular = selectedMaterial.Specular;
            var shininess = selectedMaterial.Shininess;

            selectedMaterial.Textures.TryGetValue(TextureType.Diffuse, out var diffuseTexture);
            selectedMaterial.Textures.TryGetValue(TextureType.Specular, out var specularTexture);
            selectedMaterial.Textures.TryGetValue(TextureType.Normal, out var normalTexture);
            selectedMaterial.Textures.TryGetValue(TextureType.Depth, out var depthTexture);

            if (diffuseTexture == null)
            {
                if (ImGui.ArrowButton("##material_texture_diffuse", ImGuiDir.Right))
                {
                    selectedTextureType = TextureType.Diffuse;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }
            else
            {
                if(ImGuiImpl.ImageButton("##material_texture_diffuse", diffuseTexture.Handle, new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    selectedTextureType = TextureType.Diffuse;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }

            ImGui.SameLine();

            ImGui.Text("Diffuse: ");

            ImGui.SameLine();

            if(ImGuiImpl.ColorEdit3("##Diffuse_color", ref diffuse))
                selectedMaterial.Diffuse = diffuse;

            ImGui.Spacing();

            if (specularTexture == null)
            {
                if(ImGui.ArrowButton("##material_texture_specular", ImGuiDir.Right))
                {
                    selectedTextureType = TextureType.Specular;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }
            else
            {
                if (ImGuiImpl.ImageButton("##material_texture_base", specularTexture.Handle, new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    selectedTextureType = TextureType.Specular;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }

            ImGui.SameLine();

            ImGui.Text("Specular: ");

            ImGui.SameLine();

            ImGuiImpl.ColorEdit3("##Speculr_color", ref specular);

            selectedMaterial.Specular = specular;

            ImGui.Indent(32);
            ImGui.Text("Shininess: ");

            ImGui.SameLine();

            if (ImGui.SliderFloat("##material_shininess", ref shininess, 0.001f, 1000f))
                selectedMaterial.Shininess = shininess;

            ImGui.Unindent();

            ImGui.Spacing();

            if (normalTexture == null)
            {
                if (ImGui.ArrowButton("##material_texture_normal", ImGuiDir.Right))
                {
                    selectedTextureType = TextureType.Normal;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }
            else
            {
                if (ImGuiImpl.ImageButton("##material_texture_normal", normalTexture.Handle, new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    selectedTextureType = TextureType.Normal;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }

            ImGui.SameLine();
            ImGui.Text("Normal Map");

            ImGui.SameLine();

            var normalStrength = selectedMaterial.NormalStrength;

            if (ImGui.SliderFloat("##normal_strength", ref normalStrength, 0.0001f, 10.0f, "%.2f", ImGuiSliderFlags.AlwaysClamp))
                selectedMaterial.NormalStrength = normalStrength;

            ImGui.Spacing();


            if (depthTexture == null)
            {
                if (ImGui.ArrowButton("##material_texture_depth", ImGuiDir.Right))
                {
                    selectedTextureType = TextureType.Depth;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }
            else
            {
                if (ImGuiImpl.ImageButton("##material_texture_depth", depthTexture.Handle, new Vector2(16, 16), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    selectedTextureType = TextureType.Depth;
                    ImGuiImpl.SetNextWindowSize(new Vector2(300, 400));
                    ImGui.OpenPopup("MaterialTexturesPopup");
                }
            }

            ImGui.SameLine();
            ImGui.Text("Depth Map");

            ImGui.SameLine();

            var depthScale = selectedMaterial.HeightScale;

            if (ImGui.SliderFloat("##depth_scale", ref depthScale, 0.0f, 0.2f, "%.2f", ImGuiSliderFlags.AlwaysClamp))
                selectedMaterial.HeightScale = depthScale;

            ImGui.Spacing();

            bool useTBN = selectedMaterial.useTBN == 1 ? true : false;

            ImGui.Checkbox("Use TBN", ref useTBN);

            if (useTBN)
                selectedMaterial.useTBN = 1;
            else
                selectedMaterial.useTBN = 0;

            ImGui.Spacing();

            var textureScale = new Vector2(selectedMaterial.TextureScale.X, selectedMaterial.TextureScale.Y);

            ImGui.Text("Texture scale: ");
            ImGui.SameLine();
            if (ImGuiImpl.DragFloat2("##texture_scale", ref textureScale, 0.1f, 0.1f, 100))
                selectedMaterial.TextureScale = new Vector3(textureScale.X, textureScale.Y, 1);

            ImGui.Spacing();

            var textureOffset = new Vector2(selectedMaterial.TextureOffset.X, selectedMaterial.TextureOffset.Y);

            ImGui.Text("Texture offset: ");
            ImGui.SameLine();
            if (ImGuiImpl.DragFloat2("##texture_offset", ref textureOffset, 0.1f, 0f))
                selectedMaterial.TextureOffset = new Vector3(textureOffset.X, textureOffset.Y, 1);

            if (ImGui.BeginPopup("MaterialTexturesPopup", ImGuiWindowFlags.AlwaysVerticalScrollbar))
            {
                if (ImGuiImpl.ImageButton("None", 0, new Vector2(64, 64), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    selectedMaterial.RemoveTexture(selectedTextureType);
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.IsItemHovered())
                    ImGui.SetTooltip("None");

                ImGui.SameLine();

                int count = 1;
                foreach (var kv in scene.AssetsManager.GetAllAssetsOfType<TextureAsset>()!)
                {
                    var name = kv.Key;
                    var tex = kv.Value;

                    ImGui.PushID(name);

                    if (ImGuiImpl.ImageButton(name, tex.Handle, new Vector2(64, 64), new Vector2(0, 1), new Vector2(1, 0)))
                    {
                        selectedMaterial.AddTexture(selectedTextureType, tex.Texture!);
                        ImGui.CloseCurrentPopup();
                    }

                    if (ImGui.IsItemHovered())
                        ImGui.SetTooltip(name);

                    ImGui.SameLine();

                    count++;
                    if (count % 4 == 0)
                        ImGui.NewLine();

                    ImGui.PopID();
                }

                ImGui.EndPopup();
            }
        }
    }
}
