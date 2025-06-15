using _3DGame.Core.Resources.Assets;

namespace _3DGame.Core.Resources
{
    public class AssetsManager
    {
        private SortedDictionary<string, Asset> assets = new();

        public void LoadAssets(string path)
        {
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file).ToLowerInvariant();

                var name = Path.GetFileNameWithoutExtension(file);

                switch (extension)
                {
                    case ".obj":
                        if (!assets.ContainsKey(name))
                        {
                            assets.Add(name, new MeshAsset(file, name));
                        }
                        break;
                    case ".cs":
                        if (!assets.ContainsKey(name))
                        {
                            assets.Add(name, new ScriptAsset(file, name));
                        }
                        break;
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                        if(!assets.ContainsKey(name))
                        {
                            assets.Add(name, new TextureAsset(file, name));
                        }
                        break;

                }
            }
        }

        public Asset? GetAsset(string name)
        {
            if (assets.TryGetValue(name, out var asset))
                return asset;

            return null;
        }

        public T GetAsset<T>(string name) where T : Asset
        {
            if (assets.TryGetValue(name, out var asset) && asset is T typedAsset)
                return typedAsset;

            throw new KeyNotFoundException($"Asset with name '{name}' not found or is not of type {typeof(T).Name}.");
        }

        public SortedDictionary<string, Asset> GetAllAssets()
        {
            return assets;
        }

        public Dictionary<string, T>? GetAllAssetsOfType<T>() where T : Asset
        {
            var assetsOfType = assets.Where(a => a.Value is T).Select(a => a.Value as T).ToDictionary(a => a!.Name);

            return assetsOfType!;
        }
    }
}
