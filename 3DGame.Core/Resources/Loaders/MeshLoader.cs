namespace _3DGame.Core.Resources.Loaders
{
    public static class MeshLoader
    {
        public static Mesh[] LoadMesh(string path)
        {
            var obj = ObjLoader.Load(path);

            var mesh = ObjModelToMesh.Convert(obj);

            return mesh.ToArray();
        }
    }
}
