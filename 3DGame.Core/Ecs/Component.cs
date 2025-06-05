using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs
{
    public abstract class Component
    {
        public string Name { get; } = nameof(Component);

        public GameObject? Perent { get; }

        public virtual void Start()
        {

        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void Draw(Renderer renderer)
        {

        }
    }
}
