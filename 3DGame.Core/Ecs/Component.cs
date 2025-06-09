using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs
{
    public abstract class Component
    {
        public string Name { get; } = nameof(Component);

        public GameObject? GameObject { get; set; }

        public Scene Scene => GameObject!.Scene;

        public virtual void Start()
        {

        }

        public virtual void Update(float deltaTime)
        {

        }

        public virtual void FixedUpdate(float fixedDeltaTime)
        {

        }

        public virtual void Draw(Renderer renderer)
        {

        }
    }
}
