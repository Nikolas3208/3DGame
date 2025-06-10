using System.Net.NetworkInformation;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs
{
    public class GameObject
    {
        public List<Component> components;

        public string Name { get; set; }

        public Scene Scene { get; set; }

        public GameObject(Scene scene, string name = nameof(GameObject))
        {
            Scene = scene;
            Name = name;

            components = new List<Component>();

            AddComponent(new Transformable());
        }

        public void Start()
        {
            components.ForEach(c => c.Start());
        }

        public void Update(float deltaTime)
        {
            components.ForEach(c => c.Update(deltaTime));
        }

        public virtual void FixedUpdate(float fixedDeltaTime)
        {
            components.ForEach(c => c.FixedUpdate(fixedDeltaTime));
        }

        public void Draw(Renderer renderer)
        {
            renderer.Transform *= GetComponent<Transformable>()!.Transform;

            components.ForEach(c => c.Draw(renderer));
        }

        public bool AddComponent(Component component)
        {
            if (!components.Contains(component))
            {
                components.Add(component);
                component.GameObject = this;

                if (Scene.IsStarted)
                    Start();
                return true;
            }

            return false;
        }

        public T? GetComponent<T>() where T : Component
        {
            return (T)components.Find(c => c is T)!;
        }

        public List<Component> GetAllComponent() => components;
        public bool ContainsComponent<T>() where T : Component
        {
            return components.Find(c => c is T) == null ? false : true;
        }
    }
}
