namespace DSAExperimentation.Tests.Architecture;

// What NamespaceDeclarations reads as a file's namespaces, stated as cases. The layering test
// passes a file only when this finds nothing outside the file's tier.
public sealed partial class NamespaceDeclarationsTests
{
    private const string Algorithms = "DSAExperimentation.Algorithms";

    // The container itself, a namespace inside it in either form, and a comment, which declares
    // nothing.
    public static TheoryData<string> SourcesThatStayInsideAlgorithms =>
        new()
        {
            "namespace DSAExperimentation.Algorithms;",
            "namespace DSAExperimentation.Algorithms.Searching;",
            "namespace DSAExperimentation.Algorithms.Searching\n{\n}",
            "// namespace DSAExperimentation.DataStructures.Sequence;\nnamespace DSAExperimentation.Algorithms.Searching;",
        };

    // A lower tier's namespace, the root, a sibling that merely shares the prefix, the second of two
    // declarations, and no declaration at all - each with the one namespace found outside.
    public static TheoryData<string, string> SourcesThatLeaveAlgorithms =>
        new()
        {
            { "namespace DSAExperimentation.DataStructures.Sequence;", "DSAExperimentation.DataStructures.Sequence" },
            { "namespace DSAExperimentation;", "DSAExperimentation" },
            { "namespace DSAExperimentation.AlgorithmsExtras;", "DSAExperimentation.AlgorithmsExtras" },
            {
                "namespace DSAExperimentation.Algorithms.Searching { }\nnamespace DSAExperimentation.DataStructures.Sequence { }",
                "DSAExperimentation.DataStructures.Sequence"
            },
            { "internal static class Loose { }", NamespaceDeclarations.Global },
        };

    [Theory]
    [MemberData(nameof(SourcesThatStayInsideAlgorithms))]
    public void OutsideOf_FindsNothingInTheContainerOrWithinIt(string source)
        => Assert.Empty(NamespaceDeclarations.OutsideOf(Algorithms, RepositoryFiles.CodeOf(source)));

    [Theory]
    [MemberData(nameof(SourcesThatLeaveAlgorithms))]
    public void OutsideOf_FindsTheOneDeclarationBeyondTheContainer(string source, string outside)
        => Assert.Equal(outside, Assert.Single(NamespaceDeclarations.OutsideOf(Algorithms, RepositoryFiles.CodeOf(source))));
}
