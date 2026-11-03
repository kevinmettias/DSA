using Microsoft.CodeAnalysis;

namespace DSAExperimentation.Tests.Architecture;

// Which tier of the repository a symbol is declared in, read off its namespace: the solutions and the
// LeetCode conventions at their tier's root, the benchmark harnesses and the fixtures that lay their
// input out, and the framework library. Within the
// library, ARCHITECTURE.md section 17.5 tells what a tier implements apart from what it only states:
// a baseline writes the first for itself and reads the second like every other arm.
internal static class RepositoryTiers
{
    private const string SolutionNamespace = "DSAExperimentation.LeetCode.Solutions";
    private const string ConventionsNamespace = "DSAExperimentation.LeetCode";
    private const string BenchmarkNamespace = "DSAExperimentation.Benchmarks";
    private const string FixtureNamespace = "DSAExperimentation.Benchmarks.Fixtures";

    // Every tier of the framework project: the library the trace knows not to walk into.
    private static readonly string[] LibraryNamespaces =
    [
        "DSAExperimentation.DataStructures",
        "DSAExperimentation.Algorithms",
        "DSAExperimentation.Mechanisms",
        "DSAExperimentation.Domain",
    ];

    // The tiers that implement data structures, algorithms and mechanisms, which a baseline writes
    // for itself. Domain states facts about a problem's world, which every arm reads alike.
    private static readonly string[] ImplementationNamespaces =
    [
        "DSAExperimentation.DataStructures",
        "DSAExperimentation.Algorithms",
        "DSAExperimentation.Mechanisms",
    ];

    public static bool IsBenchmark(ISymbol symbol) => IsIn(symbol, BenchmarkNamespace);

    // The LeetCode tier: its problems, and the conventions at its root that they share. The root is that
    // namespace alone, not the catalog's or the harness library's beneath it.
    public static bool IsSolution(ISymbol symbol) => IsIn(symbol, SolutionNamespace) || NamespaceOf(symbol) == ConventionsNamespace;

    public static bool IsSource(ISymbol symbol) => IsSolution(symbol) || IsBenchmark(symbol);

    public static bool IsDeclaredInRepository(ISymbol symbol) => IsSource(symbol) || IsLibrary(symbol);

    // Benchmark code that runs with the arm, as opposed to Fixtures/, which only lays input out.
    public static bool IsHarnessCode(ISymbol symbol) => IsBenchmark(symbol) && !IsIn(symbol, FixtureNamespace);

    public static bool IsLibrary(ISymbol symbol) => Array.Exists(LibraryNamespaces, library => IsIn(symbol, library));

    // A name a baseline may not use: a type or member of the implementation tiers that is neither a
    // contract nor a stated value.
    public static bool IsImplementation(ISymbol symbol)
        => Array.Exists(ImplementationNamespaces, tier => IsIn(symbol, tier)) && !IsContract(symbol) && !IsStatedValue(symbol);

    // An interface, or a member it declares: the contract a hand-written layer may implement.
    public static bool IsContract(ISymbol symbol)
        => symbol is INamedTypeSymbol { TypeKind: TypeKind.Interface } || symbol.ContainingType?.TypeKind == TypeKind.Interface;

    // A constant or an enum: a value the library names, which a baseline reads rather than copies.
    public static bool IsStatedValue(ISymbol symbol)
        => symbol is IFieldSymbol { IsConst: true } or INamedTypeSymbol { TypeKind: TypeKind.Enum }
            || symbol.ContainingType?.TypeKind == TypeKind.Enum;

    private static bool IsIn(ISymbol symbol, string container)
    {
        var name = NamespaceOf(symbol);

        return name == container || name.StartsWith($"{container}.", StringComparison.Ordinal);
    }

    private static string NamespaceOf(ISymbol symbol) => (symbol as INamespaceSymbol ?? symbol.ContainingNamespace)?.ToDisplayString() ?? string.Empty;
}
