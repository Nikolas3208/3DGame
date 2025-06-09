using _3DGame.Core.Resources.Loaders;

namespace _3DGame.Core.Resources.Assets
{
    public class MeshAsset : Asset
    {
        private Mesh[]? meshs;

        public Mesh[]? Meshs { get { if (meshs == null) { LoadAsset(); } return meshs; } }

        public MeshAsset(string fullPath, string name) : base(fullPath, name)
        {
        }

        protected override void LoadAsset()
        {
            meshs = MeshLoader.LoadMesh(FullPath);
        }
    }
}
