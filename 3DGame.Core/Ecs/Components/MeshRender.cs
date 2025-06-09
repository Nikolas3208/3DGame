using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs.Components
{
    public class MeshRender : Component
    {
        private Mesh[] meshs;

        public MeshRender()
        {
            meshs = new Mesh[0];
        }

        public MeshRender(params Mesh[] meshs)
        {
            this.meshs = meshs;
        }

        public override void Draw(Renderer renderer)
        {
            foreach (var mesh in meshs)
                mesh?.Draw(renderer);
        }
    }
}
