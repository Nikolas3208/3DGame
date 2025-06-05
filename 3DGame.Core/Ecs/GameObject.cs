using System.Net.NetworkInformation;
using _3DGame.Core.Ecs.Components;
using _3DGame.Core.Graphics;

namespace _3DGame.Core.Ecs
{
    public class GameObject
    {
        public List<Component> components;

        public GameObject()
        {
            components = new List<Component>();
        }

        public void Start()
        {

        }

        public void Update(float deltaTime)
        {

        }

        public void Draw(Renderer renderer)
        {
            renderer.Transform *= GetComponent<Transformable>()!.Transform;

            foreach (var component in components)
            {
                component.Draw(renderer);
            }
        }

        public bool AddComponent(Component component)
        {
            if (!components.Contains(component))
            {
                components.Add(component);
                return true;
            }

            return false;
        }

        public T? GetComponent<T>() where T : Component
        {
            return (T)components.Find(c => c.GetType() == typeof(T))!;
        }
    }
}
