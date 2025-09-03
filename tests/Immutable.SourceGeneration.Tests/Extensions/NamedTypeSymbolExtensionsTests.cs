using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Toarnbeike.Immutable.SourceGeneration.Extensions;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Extensions;

public class NamedTypeSymbolExtensionsTests
{
    [Fact(Skip = "Further modifying the logic is planned, for now not tested.")]
    public void GetOrderedProperties_Should_ReturnRoslynOrder_WhenSyntaxIsNotAvailable()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.DeclaringSyntaxReferences.Returns(ImmutableArray<SyntaxReference>.Empty);

        var property1 = CreatePropertySymbol("B");
        var property2 = CreatePropertySymbol("A");

        symbol.GetMembers().Returns([property1, property2]);

        var result = symbol.GetOrderedProperties();

        // order can't be guaranteed, only a ShouldContain
        var names = result.Select(p => p.Name).ToList();
        names.ShouldContain("A");
        names.ShouldContain("B");
    }

    [Fact(Skip = "Further modifying the logic is planned, for now not tested.")]
    public void GetOrderedProperties_Should_ReturnDeclarationOrder_WhenSyntaxIsAvailable()
    {
        var code = """
                   public class Test
                   {
                       public string A { get; set; }
                       public string B { get; set; }
                   }
                   """;

        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetCompilationUnitRoot();
        var typeDecl = root.DescendantNodes().OfType<ClassDeclarationSyntax>().First();

        var syntaxRef = Substitute.For<SyntaxReference>();
        syntaxRef.GetSyntax().Returns(typeDecl);

        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.DeclaringSyntaxReferences.Returns([syntaxRef]);

        var propertyA = CreatePropertySymbol("A");
        var propertyB = CreatePropertySymbol("B");

        // symbol returns in reverse order (can happen).
        symbol.GetMembers().Returns([propertyB, propertyA]);

        var result = symbol.GetOrderedProperties();

        // but the result is still in declaration order
        var names = result.Select(p => p.Name).ToList();
        names.ShouldBe(["A", "B"]);
    }

    private static IPropertySymbol CreatePropertySymbol(string name)
    {
        var property = Substitute.For<IPropertySymbol>();
        property.Name.Returns(name);
        property.DeclaredAccessibility.Returns(Accessibility.Public);
        property.Type.Returns(Substitute.For<ITypeSymbol>());
        property.Type.ToDisplayString().Returns("System.String");
        return property;
    }
}