using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Resources.Assets;
using _3DGame.Core.Utils;
using _3DGame.Editor.UIs;
using ImGuiNET;
using OpenTK.Mathematics;

namespace _3DGame.Editor
{
    public class EditorUI
    {
        private static int sceneImage = 0;
        private static Scene scene;
        private static GameObject selectedObject;
        private static AssetViewer assetViewer;
        private static Inspector inspector;

        public static Vector2 SceneWindowSize;

        public static bool SceneWindowFocused { get; private set; } = false;

        public EditorUI(Scene scene)
        {
            EditorUI.scene = scene;

            inspector = new Inspector(scene);

            assetViewer = new AssetViewer(scene);
            assetViewer.Update();
        }

        public static Inspector GetInspector() => inspector;
        public static AssetViewer GetAssetViewer() => assetViewer;

        public static void SetSceneImage(int sceneImage) => EditorUI.sceneImage = sceneImage;
        public static void Draw()
        {
            ImGui.DockSpaceOverViewport();
            SceneView();
            SceneObjects();
            //Properties();
            assetViewer.AssetView();
            inspector.Draw();
        }

        private static void SceneView()
        {
            ImGui.Begin("Scene");

            SceneWindowFocused = ImGui.IsWindowFocused();

            if(ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.SetWindowFocus("Scene");
            }

            ImGuiImpl.Image(sceneImage, ImGuiImpl.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));
            SceneWindowSize = ImGuiImpl.GetWindowSize();

            ImGui.End();
        }

        private static void SceneObjects()
        {
            ImGui.Begin("Scene objects");

            if (ImGui.TreeNodeEx(scene.Name, ImGuiTreeNodeFlags.DefaultOpen))
            {
                foreach (var obj in scene.GetAllGameObject())
                    if (ImGui.Selectable(obj.Name))
                        inspector.SetSelectedGameObject(obj);

                ImGui.TreePop();
            }

            ImGui.End();
        }

        private static void Properties()
        {
            ImGui.Begin("Propertis");

            if (selectedObject == null)
            {
                ImGui.End();
                return;
            }

            var gameObjectName = selectedObject.Name;
            if(ImGui.InputText("Name", ref gameObjectName, 64))
                selectedObject.Name = gameObjectName;

            foreach (var component in selectedObject.GetAllComponent())
            {
                bool open = ImGui.TreeNodeEx(component.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen);

                if (component is not Transform)
                {
                    // Кнопка справа от заголовка
                    float buttonSize = 20f;
                    float availWidth = ImGui.GetContentRegionAvail().X;
                    float textWidth = ImGui.CalcTextSize(component.GetType().Name).X;

                    ImGui.SameLine(availWidth - buttonSize);
                    ImGui.PushID(component.GetHashCode()); // Уникальный ID, чтобы не конфликтовало с другими
                    if (ImGui.SmallButton("X"))
                    {
                        selectedObject.RemoveComponent(component);
                        ImGui.PopID();
                        ImGui.TreePop();
                        break; // Прерываем, т.к. коллекция изменилась
                    }
                    ImGui.PopID();
                }

                if (open)
                {
                    ImGui.Indent();

                    if (component is Transform transform)
                    {
                        var position = transform.Position;
                        var rotation = transform.Rotation;
                        var scale = transform.Scale;

                        if(ImGuiImpl.DragLabeledFloat3("Position", ref position, 0.1f))
                            transform.Position = position;
                        if(ImGuiImpl.DragLabeledFloat3("Rotation", ref rotation, 0.1f))
                            transform.Rotation = rotation;
                        if(ImGuiImpl.DragLabeledFloat3("Scale", ref scale, 0.1f))
                            transform.Scale = scale;

                    }
                    else if (component is Light light)
                    {
                        int selectedLightType = (int)light.Type;

                        var diffuse = light.Diffuse;
                        var specular = light.Specular;
                        var ambient = light.Ambient;
                        var color = light.Color;

                        float linear = light.Linear;
                        float constant = light.Constant;
                        float quadratic = light.Quadratic;

                        float cutOff = light.CutOff;
                        float outerCutOff = light.OuterCutOff;

                        ImGui.Combo("Light type", ref selectedLightType, Enum.GetNames<LightType>(), 3);

                        if(ImGuiImpl.DragFloat3("Ambient", ref ambient, 0.1f))
                            light.Ambient = ambient;
                        if(ImGuiImpl.DragFloat3("Diffuse", ref diffuse, 0.1f))
                            light.Diffuse = diffuse;
                        if(ImGuiImpl.DragFloat3("Specular", ref specular, 0.1f))
                            light.Specular = specular;

                        if(ImGuiImpl.ColorEdit4("Color", ref color))
                            light.Color = color;

                        if (selectedLightType > 0)
                        {
                            ImGui.DragFloat("Linear", ref linear, 0.1f);
                            ImGui.DragFloat("Constant", ref constant, 0.1f);
                            ImGui.DragFloat("Quadratic", ref quadratic, 0.1f);

                            if (selectedLightType > 1)
                            {
                                ImGui.DragFloat("CutOff", ref cutOff, 0.1f, 0, outerCutOff);
                                ImGui.DragFloat("OuterCutOff", ref outerCutOff, 0.1f);
                            }
                        }

                        light.Type = (LightType)selectedLightType;

                        light.Color = new OpenTK.Mathematics.Vector4(color.X, color.Y, color.Z, color.W);

                        light.Linear = linear;
                        light.Constant = constant;
                        light.Quadratic = quadratic;

                        light.CutOff = cutOff;
                        light.OuterCutOff = outerCutOff;
                    }
                    else if (component is MeshRender meshRender)
                    {
                        if(ImGui.SmallButton("Select mesh##MeshRender"))
                        {
                            ImGui.OpenPopup("Select mesh");
                        }

                        if (ImGui.BeginPopup("Select mesh"))
                        {
                            var m = scene.AssetsManager.GetAllAssetsOfType<MeshAsset>();

                            foreach (var mesh in m)
                            {
                                if (ImGui.MenuItem(mesh.Value.Meshs!.Name + $"##{mesh.Value.GetHashCode()}"))
                                {
                                    ImGui.CloseCurrentPopup();
                                }
                            }

                            ImGui.EndPopup();
                        }

                        //var mesh = meshRender.GetMeshe();

                        //DrawMaterialEditor(mesh.Material);
                    }

                    ImGui.Unindent();
                    ImGui.TreePop();
                }

                ImGui.Spacing();
            }

            if (ImGui.Button("Add component"))
            {
                ImGui.OpenPopup("AddComponentPopup");
            }

            if (ImGui.BeginPopup("AddComponentPopup"))
            {
                if (ImGui.MenuItem("Light"))
                {
                    selectedObject.AddComponent(new Light(LightType.Directional));
                }
                if (ImGui.MenuItem("MeshRender"))
                {
                    selectedObject.AddComponent(new MeshRender()); // Заглушка, нужно будет добавить логику выбора меша
                }
                ImGui.EndPopup();
            }

            ImGui.End();
        }

        private static void DrawMaterialEditor(Material material)
        {
            if (material == null)
            {
                ImGui.Text("Material is not selected.");
                return;
            }

            if (ImGui.CollapsingHeader($"Material: {material.Name}", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Indent();
                
                // Редактируем цвета
                if (ImGui.CollapsingHeader("Colors", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Indent();

                    var diffuse = material.Diffuse;
                    if (ImGuiImpl.ColorEdit3("Diffuse", ref diffuse))
                        material.Diffuse = diffuse;

                    var specular = material.Specular;
                    if (ImGuiImpl.ColorEdit3("Specular", ref specular))
                        material.Specular = specular;

                    var ambient = material.Ambient;
                    if (ImGuiImpl.ColorEdit3("Ambient", ref ambient))
                        material.Ambient = ambient;

                    var baseColor = material.Color;
                    if (ImGuiImpl.ColorEdit3("Base Color", ref baseColor))
                        material.Color = baseColor;

                    ImGui.Unindent();
                }

                // Редактируем параметры
                if (ImGui.CollapsingHeader("Params", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Indent();

                    var shininess = material.Shininess;
                    if (ImGui.DragFloat("Shininess", ref shininess, 0.1f, 0, 256))
                        material.Shininess = shininess;

                    var texScale = material.TextureScale;
                    if (ImGuiImpl.DragFloat3("Texture Scale", ref texScale, 0.1f))
                        material.TextureScale = texScale;

                    ImGui.Unindent();
                }

                // Список текстур
                if (ImGui.CollapsingHeader("Textures", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Indent();

                    foreach (var pair in material.Textures)
                    {
                        var type = pair.Key.ToString();
                        var texture = pair.Value;

                        ImGui.Text($"{type} Texture");

                        // Если есть превьюшки — можно использовать AddImage()
                        if (texture.Handle != 0)
                        {
                            // Допустим, ты хочешь отображать 64x64 иконку
                            ImGui.Image((IntPtr)texture.Handle, new System.Numerics.Vector2(64, 64));
                        }

                        ImGui.SameLine();
                        if (ImGui.SmallButton($"Delate##{type}"))
                        {
                            material.RemoveTexture(pair.Key);
                            break;
                        }
                    }

                    ImGui.Unindent();
                }

                // Кнопка добавления (с popup)
                if (ImGui.Button("Add texture"))
                {
                    ImGui.OpenPopup("Select Texture");
                }

                // Popup с текстурами
                if (ImGui.BeginPopup("Select Texture"))
                {
                    ImGui.SetWindowSize(new System.Numerics.Vector2(200,300));

                    int count = 0;
                    foreach (var kv in scene.AssetsManager.GetAllAssetsOfType<TextureAsset>())
                    {
                        var name = kv.Key;
                        var tex = kv.Value;

                        ImGui.PushID(name);

                        if (ImGuiImpl.ImageButton(name, tex.Texture.Handle, new Vector2(64, 64)))
                        {
                            // Пример: добавим текстуру как диффузную
                            material.Textures[TextureType.Diffuse] = tex.Texture;
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

                ImGui.Unindent();
            }
        }
    }
}
