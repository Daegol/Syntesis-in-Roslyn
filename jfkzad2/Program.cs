using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;

namespace jfkzad2
{
    class Program
    {

        private static readonly string AssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(File.ReadAllText("Class.cs"));
            var comp = CSharpCompilation.Create("comp", syntaxTrees: new[] { tree });
            var model = comp.GetSemanticModel(tree);
            var newRoot = new Rewriter(model).Visit(tree.GetRoot());
            File.WriteAllText(@"..\..\Class.altered.cs", newRoot.GetText().ToString());

            var mscorlib = MetadataReference.CreateFromFile(Path.Combine(AssemblyPath, "mscorlib.dll"));
            var system = MetadataReference.CreateFromFile(Path.Combine(AssemblyPath, "System.dll"));
            var systemCore = MetadataReference.CreateFromFile(Path.Combine(AssemblyPath, "System.Core.dll"));

            var compilation = CSharpCompilation.Create(
                "Altered",
                syntaxTrees: new[] { newRoot.SyntaxTree },
                references: new[] { mscorlib, system, systemCore },
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            

            foreach (var item in compilation.GetDiagnostics())
                Console.WriteLine($"Diagnostics: {item}");

            var emitResult = compilation.Emit("Altered.exe", "Altered.pdb");

            if (!emitResult.Success)
                foreach (var error in emitResult.Diagnostics)
                    Console.WriteLine(error);


            Console.ReadLine();
        }
    }
}
