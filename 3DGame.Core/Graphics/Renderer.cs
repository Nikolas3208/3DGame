namespace _3DGame.Core.Graphics
{
    public struct Renderer
    {
        public Shader? Shader { get; set; }

        public Renderer(Shader shader)
        {
            Shader = shader;
        }

        public static Renderer Default => new Renderer(new Shader("default.vert", "default.frag"));
    }
}
