using _3DGame.Core;
using _3DGame.Core.Resources.Assets;
using ImGuiNET;
using System.Numerics;

namespace _3DGame.Editor.UIs
{
    public class AssetViewer
    {
        private Scene scene;

        private string[] directorys = [];
        private string[] filesCurentDirectory = [];

        private int iconSize = 64;
        private int folderIconHandle = 0;
        private int textIconHandle = 0;
        private int textureIconHandle = 0;
        private int materialIconHandle = 0;
        private int meshIconHandle = 0;
        private int audioIconHandle = 0;

        public string CurentDirectoryName = "Assets";
        public string CurentDirectoryFullPath = "Assets";
        public string PreviosDerectoryFullPath = "Assets";

        public bool IsFocused { get; private set; } = false;

        public AssetViewer(Scene scene)
        {
            this.scene = scene;

            folderIconHandle = scene.AssetsManager.GetAsset<TextureAsset>("folder").Texture.Handle;
            audioIconHandle = scene.AssetsManager.GetAsset<TextureAsset>("audio_icon").Texture.Handle;
            textIconHandle = scene.AssetsManager.GetAsset<TextureAsset>("text_icon").Texture.Handle;
        }

        public void Update()
        {
            directorys = Directory.GetDirectories(CurentDirectoryFullPath, "*");
            filesCurentDirectory = Directory.GetFiles(CurentDirectoryFullPath, "*");
        }

        public void AssetView()
        {
            ImGui.Begin("Asset viewer");

            IsFocused = ImGui.IsWindowFocused();

            if (ImGui.Button("<") && CurentDirectoryName != "Assets")
            {
                SetCurentDirectory(Directory.GetParent(CurentDirectoryFullPath)!.FullName);
            }
            ImGui.SameLine();

            if (ImGui.Button(">") && CurentDirectoryFullPath != PreviosDerectoryFullPath)
            {
                SetCurentDirectory(Path.GetFullPath(PreviosDerectoryFullPath));
            }

            ImGui.SameLine();
            ImGui.Text(CurentDirectoryFullPath);

            int columss = (int)ImGui.GetContentRegionAvail().X / (iconSize + 16);

            ImGui.Columns(columss, "asset_grid", false);

            foreach(string dir in directorys)
            {
                ImGui.BeginGroup();

                if(ImGui.ImageButton(dir, folderIconHandle, new Vector2(iconSize, iconSize), new Vector2(0, 1), new Vector2(1, 0)))
                {
                    SetCurentDirectory(dir);
                }
                ImGui.Text(Path.GetFileNameWithoutExtension(dir));

                ImGui.EndGroup();
                ImGui.NextColumn();

            }

            foreach (string file in filesCurentDirectory)
            {
                ImGui.BeginGroup();

                var name = Path.GetFileNameWithoutExtension(file);

                var extension = Path.GetExtension(file);

                switch(extension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                        int textureHandle = scene.AssetsManager.GetAsset<TextureAsset>(name).Handle;
                        textureHandle = textureHandle == 0 ? textureIconHandle : textureHandle;
                        ImGui.ImageButton(name, textureHandle, new Vector2(iconSize, iconSize), new Vector2(0, 1), new Vector2(1, 0));
                        ImGui.Text(name);
                        break;
                    default:
                        ImGui.PushID(Guid.NewGuid().ToString());
                        ImGui.ImageButton(name, textIconHandle, new Vector2(iconSize, iconSize), new Vector2(0, 1), new Vector2(1, 0));
                        ImGui.PopID();
                        ImGui.Text(name);
                        break;
                }

                ImGui.EndGroup();
                ImGui.NextColumn();
            }

            ImGui.End();
        }

        private void SetCurentDirectory(string newCurentDirectory)
        {
            PreviosDerectoryFullPath = CurentDirectoryFullPath;

            var fullPathToAssets = Path.GetFullPath("Assets");

            CurentDirectoryFullPath = "Assets\\" + Path.GetRelativePath(fullPathToAssets, newCurentDirectory);
            CurentDirectoryName = Path.GetFileName(newCurentDirectory);
            Update();
        }
    }
}
