using _3DGame.Core.Ecs;
using _3DGame.Core.Ecs.Components;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Reflection;
using System.Runtime.Loader;

namespace _3DGame.Core.Resources
{
    public static class ScriptsManager
    {
        private static Dictionary<string, ScriptComponent> scripts = new();
        
        public static void LoadScripts(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException("Script folder not found: " + path);

            var scriptFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);

            var syntaxTrees = scriptFiles.Select(file =>
                CSharpSyntaxTree.ParseText(File.ReadAllText(file))
            ).ToList();

            // Подключение нужных сборок
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
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
                    Console.WriteLine(diagnostic);
                return;
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(ScriptComponent).IsAssignableFrom(type))
                    continue;

                if (Activator.CreateInstance(type) is ScriptComponent scriptInstance)
                    scripts.Add(type.Name, scriptInstance);
            }
        }

        public static ScriptComponent? GetScript(string name)
        {
            if(scripts.TryGetValue(name, out var script))
            {
                return script;
            }

            return null;
        }

        public static T? GetScript<T>(string name) where T : ScriptComponent
        {
            if (scripts.TryGetValue(name, out var script))
            {
                return (T)script;
            }

            return null;
        }
    }
}
