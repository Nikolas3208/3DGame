using _3DGame.Core.Graphics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;
using System.Runtime.Loader;

namespace _3DGame.Core.Ecs.Components
{
    public class ScriptComponent : Component
    {
        public Component Script { get; }

        public ScriptComponent(Component script)
        {
            Script = script ?? throw new ArgumentNullException(nameof(script), "Script cannot be null.");
        }

        public override void Start() 
        {
            Script?.Start();
        }
        public override void Update(float deltaTime)
        {
            Script?.Update(deltaTime);
        }
        public override void FixedUpdate(float deltaTime)
        {
            Script?.FixedUpdate(deltaTime);
        }
        public override void Draw(Renderer renderer) { Script.Draw(renderer); }
    }
}
