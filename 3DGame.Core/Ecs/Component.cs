using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;
using _3DGame.Core.Physics;

namespace _3DGame.Core.Ecs
{
    public abstract class Component
    {
        public string Name { get; } = nameof(Component);

        public GameObject? GameObject { get; set; }

        public Scene Scene => GameObject!.Scene;

        protected Transform transform { get => GameObject?.GetComponent<Transform>()!; }

        protected T? GetComponent<T>() where T : Component => GameObject!.GetComponent<T>();

        protected bool AddComponent(Component component) => GameObject!.AddComponent(component);

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

        public virtual void OnCollided(CollidedEventArgs args)
        {

        }
    }
}
