using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Benchmarks.Generators;

// The parameter types a generated arm can fill from LeetCode's notation, stated over compile-time types:
// the rules NotationReader applies at run time, so a strategy this accepts is one the reader can feed.
// Also how a type is written into generated code, fully qualified and with tuple element names dropped,
// since typeof refuses them.
internal static class NotationTypes
{
    // A ValueTuple holds seven items before its eighth slot nests the rest.
    public const int MostTupleItems = 7;

    // A grid's rank, the one rank past a list's that LeetCode's rows of rows can fill.
    private const int GridRank = 2;

    private const string TupleOpen = "(";
    private const string TupleClose = ")";
    private const string ItemSeparator = ", ";
    private const char RankSeparator = ',';
    private const char ArrayOpen = '[';
    private const char ArrayClose = ']';
    private const string OneRank = "[]";
    private const string MaybeMark = "?";

    // Sequences a strategy can only read, which an array fills, and those it may add to, which a List does.
    private static readonly string[] Sequences =
    [
        "System.Collections.Generic.IEnumerable`1",
        "System.Collections.Generic.IReadOnlyList`1",
        "System.Collections.Generic.IReadOnlyCollection`1",
        "System.Collections.Generic.List`1",
        "System.Collections.Generic.IList`1",
        "System.Collections.Generic.ICollection`1",
    ];

    // LeetCode's tree and list inputs, read through LeetCodeWireFormat, which builds them over int.
    private static readonly string[] IntegerNodes =
    [
        "DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode`1",
        "DSAExperimentation.DataStructures.SinglyLinkedList.SinglyLinkedListNode`1",
    ];

    private static readonly SpecialType[] Scalars =
    [
        SpecialType.System_Int32,
        SpecialType.System_Int64,
        SpecialType.System_UInt32,
        SpecialType.System_Double,
        SpecialType.System_Boolean,
        SpecialType.System_String,
        SpecialType.System_Char,
    ];

    private static readonly SymbolDisplayFormat NameWithoutArguments = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

    public static bool IsReadable(ITypeSymbol type) => type switch
    {
        _ when Scalars.Contains(type.SpecialType) => true,
        IArrayTypeSymbol array when array.Rank <= GridRank => IsReadable(array.ElementType),
        INamedTypeSymbol { IsTupleType: true } tuple =>
            tuple.TupleElements.Length <= MostTupleItems && tuple.TupleElements.All(item => IsReadable(item.Type)),
        INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } maybe => IsReadable(maybe.TypeArguments[0]),
        INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } generic => IsReadableGeneric(generic),
        INamedTypeSymbol { IsValueType: true } valueType when RecordValuesOf(valueType) is { } values => values.All(IsReadable),
        _ => false,
    };

    private static bool IsReadableGeneric(INamedTypeSymbol type)
    {
        var definition = MetadataNameOf(type.OriginalDefinition);
        var argument = type.TypeArguments[0];

        if (IntegerNodes.Contains(definition))
        {
            return argument.SpecialType == SpecialType.System_Int32;
        }

        return Sequences.Contains(definition) && IsReadable(argument);
    }

    private static string MetadataNameOf(INamedTypeSymbol type) =>
        $"{type.ContainingNamespace.ToDisplayString()}.{type.MetadataName}";

    // The values a positional record of value type is built from, in order: the parameters of its one public
    // constructor, beside the parameterless one every struct has, each kept as the property of the same
    // name - a role type such as SourceText(string Text), or Rectangle(X1, Y1, X2, Y2). Null for any other.
    private static List<ITypeSymbol>? RecordValuesOf(INamedTypeSymbol type)
    {
        var constructors = type.InstanceConstructors.Where(constructor => constructor is { IsImplicitlyDeclared: false, DeclaredAccessibility: Accessibility.Public }).ToList();

        if (constructors.Count != 1 || constructors[0].Parameters.Length == 0)
        {
            return null;
        }

        var parameters = constructors[0].Parameters;

        if (!parameters.All(parameter => IsKeptAsProperty(type, parameter)))
        {
            return null;
        }

        return [.. parameters.Select(parameter => parameter.Type)];
    }

    private static bool IsKeptAsProperty(INamedTypeSymbol type, IParameterSymbol parameter) =>
        type.GetMembers(parameter.Name).OfType<IPropertySymbol>().Any(property => SymbolEqualityComparer.Default.Equals(property.Type, parameter.Type));

    // How many consecutive case inputs a parameter of this type can stand for, as NotationReader.WidthOf
    // counts them: a tuple's or a record's values, when it has several, and one for any other type.
    public static int WidthOf(ITypeSymbol type) => type switch
    {
        INamedTypeSymbol { IsTupleType: true } tuple => tuple.TupleElements.Length,
        INamedTypeSymbol { IsValueType: true } valueType when RecordValuesOf(valueType) is { } values => values.Count,
        _ => 1,
    };

    // An array of a rendered type, nested depth times, written as C# reads it: rank specifiers run outermost
    // first, so the new brackets go before the type's own trailing ones, and an array of int[,] is int[][,],
    // where int[,][] would be a grid of int[]. Brackets inside a tuple or a type argument are not trailing,
    // and stay where they are.
    public static string ArrayOf(string element, int depth)
    {
        var trailing = element.Length;

        while (trailing > 0 && element[trailing - 1] == ArrayClose)
        {
            trailing = element.LastIndexOf(ArrayOpen, trailing - 1);
        }

        var ranks = Enumerable.Repeat(OneRank, depth);
        var brackets = string.Concat(ranks);

        return element.Substring(0, trailing) + brackets + element.Substring(trailing);
    }

    public static string Render(ITypeSymbol type) => type switch
    {
        IArrayTypeSymbol array => $"{Render(array.ElementType)}[{new string(RankSeparator, array.Rank - 1)}]",
        INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } maybe => Render(maybe.TypeArguments[0]) + MaybeMark,
        INamedTypeSymbol { IsTupleType: true } tuple =>
            TupleOpen + string.Join(ItemSeparator, tuple.TupleElements.Select(item => Render(item.Type))) + TupleClose,
        INamedTypeSymbol { IsGenericType: true } generic =>
            $"{generic.ToDisplayString(NameWithoutArguments)}<{string.Join(ItemSeparator, generic.TypeArguments.Select(Render))}>",
        _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
    };
}
