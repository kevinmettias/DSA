using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace DSAExperimentation.Tests.Architecture;

// The operations a member's source holds, read through the semantic model of whichever of the
// compilations declares it: BaselineTrace's view of the code it walks.
internal sealed class SourceModels(IReadOnlyList<Compilation> compilations)
{
    private readonly Dictionary<SyntaxTree, SemanticModel> _models = [];
    private readonly Dictionary<ISymbol, Dictionary<ISymbol, List<IOperation>>> _assignmentsByType = new(SymbolEqualityComparer.Default);

    // What runs when the member is reached: a method's or accessor's body, a property's accessors,
    // and a static field's or property's initializer.
    public IEnumerable<IOperation> BodiesOf(ISymbol member)
        => member switch
        {
            IMethodSymbol method => method.DeclaringSyntaxReferences.Select(reference => OperationOf(reference.GetSyntax())).OfType<IOperation>(),
            IPropertySymbol property => PropertyBodiesOf(property),
            IFieldSymbol { IsStatic: true } field => InitializersOf(field),
            _ => [],
        };

    // A property's accessors, and its initializer when it is static; an instance property's runs
    // with the constructor.
    private IEnumerable<IOperation> PropertyBodiesOf(IPropertySymbol property)
    {
        var accessors = new[] { property.GetMethod, property.SetMethod }.OfType<IMethodSymbol>().SelectMany(BodiesOf);

        if (property.IsStatic)
        {
            return accessors.Concat(InitializersOf(property));
        }

        return accessors;
    }

    // The initializers of a type's instance fields and properties, which run with every constructor.
    public IEnumerable<IOperation> InstanceInitializersOf(INamedTypeSymbol type)
        => type.GetMembers()
            .Where(member => !member.IsStatic && member is IFieldSymbol or IPropertySymbol)
            .SelectMany(InitializersOf);

    // Every value the member's own type assigns it, its initializer first.
    public IEnumerable<IOperation> ValuesAssignedTo(ISymbol member)
    {
        if (member.ContainingType is not { } owner)
        {
            return [];
        }

        var assignments = AssignmentsIn(owner.OriginalDefinition);

        return InitializersOf(member).Concat(assignments.GetValueOrDefault(member.OriginalDefinition, []));
    }

    private IEnumerable<IOperation> InitializersOf(ISymbol member)
        => member.DeclaringSyntaxReferences
            .Select(reference => reference.GetSyntax())
            .Select(syntax => syntax switch
            {
                VariableDeclaratorSyntax { Initializer: { } initializer } => OperationOf(initializer),
                PropertyDeclarationSyntax { Initializer: { } initializer } => OperationOf(initializer),
                _ => null,
            })
            .OfType<IOperation>();

    private Dictionary<ISymbol, List<IOperation>> AssignmentsIn(INamedTypeSymbol type)
    {
        if (!_assignmentsByType.TryGetValue(type, out var assignments))
        {
            assignments = CollectAssignments(type);
            _assignmentsByType[type] = assignments;
        }

        return assignments;
    }

    // The values each member of a type is assigned anywhere in the type's own source.
    private Dictionary<ISymbol, List<IOperation>> CollectAssignments(INamedTypeSymbol type)
    {
        var assignments = new Dictionary<ISymbol, List<IOperation>>(SymbolEqualityComparer.Default);
        var syntax = type.DeclaringSyntaxReferences.SelectMany(reference => reference.GetSyntax().DescendantNodes());

        foreach (var assignment in syntax.OfType<AssignmentExpressionSyntax>().Select(OperationOf).OfType<IAssignmentOperation>())
        {
            if (AssignedMember(assignment) is { } target)
            {
                assignments.TryAdd(target, []);
                assignments[target].Add(assignment.Value);
            }
        }

        return assignments;
    }

    private static ISymbol? AssignedMember(IAssignmentOperation assignment)
        => assignment.Target switch
        {
            IFieldReferenceOperation field => field.Field.OriginalDefinition,
            IPropertyReferenceOperation property => property.Property.OriginalDefinition,
            _ => null,
        };

    private IOperation? OperationOf(SyntaxNode syntax) => ModelOf(syntax.SyntaxTree).GetOperation(syntax);

    private SemanticModel ModelOf(SyntaxTree tree)
    {
        if (!_models.TryGetValue(tree, out var model))
        {
            var owner = compilations.FirstOrDefault(compilation => compilation.ContainsSyntaxTree(tree))
                ?? throw new InvalidOperationException($"No compilation the trace reads holds {tree.FilePath}.");
            model = owner.GetSemanticModel(tree);
            _models[tree] = model;
        }

        return model;
    }
}
