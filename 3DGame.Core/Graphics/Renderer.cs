using OpenTK.Mathematics;

namespace _3DGame.Core.Graphics
{
    public struct Renderer
    {
        private Matrix4 transform = Matrix4.Identity;

        public Matrix4 Transform { get => transform; set { transform = value; Shader?.SetMatrix4("model", Transform); } }
        public Shader Shader { get; set; }

        public Renderer(Shader shader)
        {
            Shader = shader;
        }

        public static Renderer Default => new Renderer(new Shader("Shaders\\default.vert", "Shaders\\default.frag"));
    }
}
