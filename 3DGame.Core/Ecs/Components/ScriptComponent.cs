using _3DGame.Core.Graphics;
using _3DGame.Core.Physics;

namespace _3DGame.Core.Ecs.Components
{
    public class ScriptComponent : Component
    {
        public T? GetComponent<T>() where T : Component
        {
            return GameObject?.GetComponent<T>();
        }

        public override void Start() { }
        public override void Update(float deltaTime) { }
        public override void FixedUpdate(float deltaTime) { }
        public override void Draw(Renderer renderer) { }

        public virtual void OnCollided(CollidedEventArgs args) { }
    }
}
