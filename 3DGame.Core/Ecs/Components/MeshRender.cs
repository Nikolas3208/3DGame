using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs.Components
{
    public class MeshRender : Component
    {
        private Mesh? mesh;

        public MeshRender()
        {

        }

        public override void Draw(Renderer renderer)
        {
            mesh?.Draw(renderer);
        }
    }
}
