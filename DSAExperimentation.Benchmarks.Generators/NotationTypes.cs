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

    private const string TupleOpen = "(";
    private const string TupleClose = ")";
    private const string ItemSeparator = ", ";
    private const char RankSeparator = ',';

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
        IArrayTypeSymbol { Rank: 1 } array => IsReadable(array.ElementType),
        INamedTypeSymbol { IsTupleType: true } tuple =>
            tuple.TupleElements.Length <= MostTupleItems && tuple.TupleElements.All(item => IsReadable(item.Type)),
        INamedTypeSymbol { IsGenericType: true, TypeArguments.Length: 1 } generic => IsReadableGeneric(generic),
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

    public static string Render(ITypeSymbol type) => type switch
    {
        IArrayTypeSymbol array => $"{Render(array.ElementType)}[{new string(RankSeparator, array.Rank - 1)}]",
        INamedTypeSymbol { IsTupleType: true } tuple =>
            TupleOpen + string.Join(ItemSeparator, tuple.TupleElements.Select(item => Render(item.Type))) + TupleClose,
        INamedTypeSymbol { IsGenericType: true } generic =>
            $"{generic.ToDisplayString(NameWithoutArguments)}<{string.Join(ItemSeparator, generic.TypeArguments.Select(Render))}>",
        _ => type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
    };
}
