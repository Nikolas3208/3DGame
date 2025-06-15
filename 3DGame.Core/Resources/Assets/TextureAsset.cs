namespace _3DGame.Core.Resources.Assets
{
    public class TextureAsset : Asset
    {
        private Texture? texture;

        public Texture? Texture { get { if (texture == null) { LoadAsset(); } return texture; } }

        public int Handle { get { if (texture == null) { LoadAsset(); } return texture!.Handle; } }

        public TextureAsset(string fullPath, string name) : base(fullPath, name)
        {
            LoadAsset();
        }

        protected override void LoadAsset() 
        {
            texture = Texture.LoadFromFile(FullPath);
        }
    }
}
