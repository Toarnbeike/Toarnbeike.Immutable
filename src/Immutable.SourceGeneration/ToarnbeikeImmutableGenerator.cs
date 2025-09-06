using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Toarnbeike.Immutable.Abstractions.Entities;
using Toarnbeike.Immutable.SourceGeneration.Entities;
using Toarnbeike.Immutable.SourceGeneration.Repositories;
using Toarnbeike.Immutable.SourceGeneration.TypeInformation;

namespace Toarnbeike.Immutable.SourceGeneration;

/// <summary>
/// Generates a partial implementation of an Entity{TKey},
/// given that the entity contains an [Aggregate] attribute.
/// Provides private constructors and static factory methods. 
/// </summary>
[Generator]
[ExcludeFromCodeCoverage]
public class ToarnbeikeImmutableGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        //if (!Debugger.IsAttached) Debugger.Launch();
        var aggregates = GetAggregates(context.SyntaxProvider);
        var entityKeys = GetEntityKeys(context.SyntaxProvider);
        var aggregateCollectionWithAssembly =
            aggregates.Collect().Combine(context.CompilationProvider)
                .Select((tuple, _) => (tuple.Left, tuple.Right.AssemblyName));
        
        // AggregateGenerator
        context.RegisterSourceOutput(aggregates, static (spc, aggregate) =>
            spc.AddSource(AggregateGenerator.FileName(aggregate!), 
                SourceText.From(AggregateGenerator.Execute(aggregate!), Encoding.UTF8)));
        
        // EntityKeyGenerator
        context.RegisterSourceOutput(entityKeys, static (spc, key) =>
            spc.AddSource(EntityKeyGenerator.FileName(key!),
                SourceText.From(EntityKeyGenerator.Execute(key!), Encoding.UTF8)));
        
        // RepositoryGenerator
        context.RegisterSourceOutput(aggregates, static (spc, aggregate) =>
            spc.AddSource(RepositoryGenerator.FileName(aggregate!), 
                SourceText.From(RepositoryGenerator.Execute(aggregate!), Encoding.UTF8)));
       
        // DataContextGenerator
        context.RegisterSourceOutput(aggregateCollectionWithAssembly, static (spc, tuple) =>
        {
            if (tuple.Left.IsEmpty)
            {
                return; // do not generate empty DataContext
            }
            spc.AddSource($"{tuple.AssemblyName}.{DataContextGenerator.FileName}",
                SourceText.From(DataContextGenerator.Execute(tuple.Left), Encoding.UTF8));
        });
    }

    private static IncrementalValuesProvider<AggregateInfo?> GetAggregates(SyntaxValueProvider syntax) =>
        syntax
            .ForAttributeWithMetadataName(
                typeof(AggregateAttribute).FullName!,
                predicate: static (node, _) => true,
                transform: (ctx, _) => AggregateInfo.Create((INamedTypeSymbol)ctx.TargetSymbol))
            .Where(static aggregate => aggregate is not null);
    
    private static IncrementalValuesProvider<EntityKeyInfo?> GetEntityKeys(SyntaxValueProvider syntax) =>
        syntax
            .ForAttributeWithMetadataName(
                typeof(EntityKeyAttribute).FullName!,
                predicate: static (node, _) => node is RecordDeclarationSyntax,
                transform: static (ctx, _) => EntityKeyInfo.Create((INamedTypeSymbol)ctx.TargetSymbol))
            .Where(static m => m is not null);
}