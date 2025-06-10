using _3DGame.Core;
using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using ImGuiNET;
using System.Numerics;

namespace _3DGame.Editor
{
    public static class EditorUI
    {
        private static int sceneImage = 0;
        private static Scene scene;
        private static GameObject selectedObject;

        public static Vector2 SceneWindowSize;

        public static void SetSceneImage(int sceneImage) => EditorUI.sceneImage = sceneImage;
        public static void SetScene(Scene scene) => EditorUI.scene = scene;

        public static void Draw()
        {
            ImGui.DockSpaceOverViewport();
            SceneView();
            SceneObjects();
            Properties();
        }

        private static void SceneView()
        {
            ImGui.Begin("Scene");

            ImGui.Image(sceneImage, ImGui.GetContentRegionAvail(), new Vector2(0, 1), new Vector2(1, 0));
            SceneWindowSize = ImGui.GetWindowSize();

            ImGui.End();
        }

        private static void SceneObjects()
        {
            ImGui.Begin("Scene objects");

            if (ImGui.TreeNodeEx(scene.Name, ImGuiTreeNodeFlags.DefaultOpen))
            {
                foreach (var obj in scene.GetAllGameObject())
                    if (ImGui.Selectable(obj.Name))
                        selectedObject = obj;

                ImGui.TreePop();
            }

            ImGui.End();
        }

        private static void Properties()
        {
            ImGui.Begin("Properti");

            if (selectedObject == null)
            {
                ImGui.End();
                return;
            }

            foreach(var component in selectedObject.GetAllComponent())
            {
                if(ImGui.TreeNodeEx(component.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (component is Transformable transform)
                    {
                        var position = new Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
                        var rotation = new Vector3(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z);
                        var scale = new Vector3(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);

                        ImGui.DragFloat3("Position", ref position);
                        ImGui.DragFloat3("Rotation", ref rotation);
                        ImGui.DragFloat3("Scale", ref scale);

                        transform.Position = new OpenTK.Mathematics.Vector3(position.X, position.Y, position.Z);
                        transform.Rotation = new OpenTK.Mathematics.Vector3(rotation.X, rotation.Y, rotation.Z);
                        transform.Scale = new OpenTK.Mathematics.Vector3(scale.X, scale.Y, scale.Z);
                    }
                    else if (component is Light light)
                    {
                        int selectedLightType = (int)light.Type;

                        var diffuse = new Vector3(light.Diffuse.X, light.Diffuse.Y, light.Diffuse.Z);
                        var specular = new Vector3(light.Specular.X, light.Specular.Y, light.Specular.Z);
                        var ambient = new Vector3(light.Ambient.X, light.Ambient.Y, light.Ambient.Z);
                        var color = new Vector4(light.Color.X, light.Color.Y, light.Color.Z, light.Color.W);

                        float linear = light.Linear;
                        float constant = light.Constant;
                        float quadratic = light.Quadratic;

                        float cutOff = light.CutOff;
                        float outerCutOff = light.OuterCutOff;

                        ImGui.Combo("Light type", ref selectedLightType, Enum.GetNames<LightType>(), 3);

                        ImGui.DragFloat3("Ambient", ref ambient);
                        ImGui.DragFloat3("Diffuse", ref diffuse);
                        ImGui.DragFloat3("Specular", ref specular);

                        ImGui.ColorEdit4("Color", ref color);

                        if (selectedLightType > 0)
                        {

                            ImGui.DragFloat("Linear", ref linear);
                            ImGui.DragFloat("Constant", ref constant);
                            ImGui.DragFloat("Quadratic", ref quadratic);

                            if (selectedLightType > 1)
                            {
                                ImGui.DragFloat("CutOff", ref cutOff, 0.1f, 0, outerCutOff);
                                ImGui.DragFloat("OuterCutOff", ref outerCutOff);
                            }
                        }

                        light.Type = (LightType)selectedLightType;

                        light.Ambient = new OpenTK.Mathematics.Vector3(ambient.X, ambient.Y, ambient.Z);
                        light.Diffuse = new OpenTK.Mathematics.Vector3(diffuse.X, diffuse.Y, diffuse.Z);
                        light.Specular = new OpenTK.Mathematics.Vector3(specular.X, specular.Y, specular.Z);

                        light.Color = new OpenTK.Mathematics.Vector4(color.X, color.Y, color.Z, color.W);

                        light.Linear = linear;
                        light.Constant = constant;
                        light.Quadratic = quadratic;

                        light.CutOff = cutOff;
                        light.OuterCutOff = outerCutOff;
                    }


                    ImGui.TreePop();
                }
            }

            ImGui.End();
        }
    }
}
