using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.TestHelpers;

public static class RoslynTestHelper
{
    public static INamedTypeSymbol GetNamedTypeSymbol(string source, string fullyQualifiedMetadataName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [syntaxTree],
            [
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Guid).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            ],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        var symbol = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
        return symbol ?? throw new InvalidOperationException($"Type {fullyQualifiedMetadataName} not found in compilation.");
    }
}