using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs.Components
{
    public class MeshRender : Component
    {
        private Mesh? mesh;
        private Material material;

        public MeshRender()
        {
            material = Material.BaseMaterial;
        }

        public MeshRender(Mesh mesh)
        {
            this.mesh = mesh;
            material = Material.BaseMaterial;
        }

        public override void Draw(Renderer renderer)
        {
            material?.Draw(renderer);
            mesh?.Draw(renderer);
            material?.UnbindTexture(renderer);
        }
        public void SetMeshe(Mesh mesh)
        {
            this.mesh = mesh;
        }

        public Mesh? GetMesh() => mesh;

        public void SetMaterial(Material material)
        {
            this.material = material;
        }

        public Material GetMaterial() => material;
    }
}
