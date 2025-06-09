namespace _3DGame.Core.Resources.Assets
{
    public abstract class Asset
    {
        public string FullPath { get; set; }
        public string Name { get; set; }

        public Asset(string fullPath, string name)
        {
            FullPath = fullPath;
            Name = name;
        }

        protected abstract void LoadAsset();
    }
}
