using _3DGame.Core.Ecs.Components;
using System.IO;
using System.Runtime.Loader;

namespace _3DGame.Core.Resources.Assets
{
    public class ScriptAsset : Asset
    {
        private ScriptComponent? script;

        public ScriptComponent Script { get { if (script == null) { LoadAsset(); } return script!; } }

        public ScriptAsset(string fullPath, string name) : base(fullPath, name)
        {
        }

        protected override void LoadAsset()
        {
        }
    }
}
