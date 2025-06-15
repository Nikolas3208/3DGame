using _3DGame.Core.Ecs.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Loader;

namespace _3DGame.Core.Resources.Assets
{
    public class ScriptAsset : Asset
    {
        private Component? script;

        public Component Script { get { if (script == null) { LoadAsset(); } return script!; } }

        public ScriptAsset(string fullPath, string name) : base(fullPath, name)
        {
        }

        protected override void LoadAsset()
        {
            var syntaxTrees = new List<SyntaxTree>() { CSharpSyntaxTree.ParseText(File.ReadAllText(FullPath)) };

            // Подключение нужных сборок
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ScriptComponent).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Vector3).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Keys).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            };

            var compilation = CSharpCompilation.Create(
                "ScriptAssembly",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                foreach (var diagnostic in emitResult.Diagnostics)
                    throw new Exception(diagnostic.GetMessage());
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(Component).IsAssignableFrom(type))
                    continue;

                if (Activator.CreateInstance(type) is Component scriptInstance)
                {
                    script = scriptInstance;
                }
            }
        }
    }
}
