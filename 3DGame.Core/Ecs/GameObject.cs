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
            foreach (var component in components)
            {
                component.Draw(renderer);
            }
        }
    }
}
