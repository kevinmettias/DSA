using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace DSAExperimentation.Tests.Architecture;

// What counts as a copy, stated as probes: a problem's constants compiled against the real library,
// beside a stand-in for the LeetCode tier's root, each planting one value a composed arm might repeat.
public sealed partial class LibraryValueCopiesTests
{
    // Revolution shares no word with AngleUnits.FullTurnDegrees, so only its value can match.
    private const string RevolutionCopy = "private const double Revolution = 360.0;";

    [Fact]
    public void In_DistinctiveValueTheLibraryStates_IsACopy()
        => Assert.Contains(
            new LibraryValueCopies.Copy(
                "DSAExperimentation.LeetCode.Solutions.Probe.ProbeSolution.Revolution", "DSAExperimentation.Domain.Mathematics.Geometry.AngleUnits.FullTurnDegrees"),
            CopiesIn(RevolutionCopy));

    [Fact]
    public void In_IntegerOfAnotherWidth_IsACopy()
        => Assert.Contains(
            new LibraryValueCopies.Copy("DSAExperimentation.LeetCode.Solutions.Probe.ProbeSolution.Modulus", "DSAExperimentation.LeetCode.Solutions.ProbeRoot.Prime"),
            CopiesIn("private const int Modulus = 1_000_000_007;"));

    // Small numbers mean many things, so a 3 with no word in common with a library 3 is not a copy.
    [Fact]
    public void In_SmallValueWithNoSharedWord_IsNoCopy()
        => Assert.Empty(CopiesIn("private const int Columns = 3;"));

    [Fact]
    public void In_SmallValueSharingASubjectWord_IsACopy()
        => Assert.Contains(
            new LibraryValueCopies.Copy("DSAExperimentation.LeetCode.Solutions.Probe.ProbeSolution.ChessBoardWidth", "DSAExperimentation.Domain.Games.Chess.ChessRules.BoardSize"),
            CopiesIn("private const int ChessBoardWidth = 8;"));

    // Size names a role, not a subject: a GridSize of 9 shares no subject with a sudoku board's 9.
    [Fact]
    public void In_SmallValueSharingOnlyARoleWord_IsNoCopy()
        => Assert.Empty(CopiesIn("private const int GridSize = 9;"));

    // Factor names a role too: a scale of 2 is not AlgorithmConstants' halving factor.
    [Fact]
    public void In_SmallValueSharingOnlyFactor_IsNoCopy()
        => Assert.Empty(CopiesIn("private const int ScaleFactor = 2;"));

    // An enum's numbers are its encoding: a local heading enum does not copy CompassHeading's.
    [Fact]
    public void In_EnumMember_IsNoCopy()
        => Assert.Empty(CopiesIn("private enum Heading { North, East, South, West }"));

    [Fact]
    public void In_ConstantABaselineReaches_IsNoCopy()
        => Assert.Empty(CopiesIn(RevolutionCopy, spared: "Revolution"));

    [Fact]
    public void In_ConstantOfTheTierRoot_IsNoCopy()
        => Assert.DoesNotContain(CopiesIn(string.Empty), copy => copy.Name.StartsWith("DSAExperimentation.LeetCode.Solutions.ProbeRoot", StringComparison.Ordinal));

    private static IReadOnlyList<LibraryValueCopies.Copy> CopiesIn(string solutionMembers, string spared = "")
    {
        var probe = ProbeCompilation(solutionMembers);
        var baselineReached = probe.GetSymbolsWithName(name => name == spared, SymbolFilter.Member).ToHashSet(SymbolEqualityComparer.Default);

        return LibraryValueCopies.In(RepositoryCompilation.Framework, probe, baselineReached);
    }

    // The probe compiled against what the solution project compiles against. A probe that does not
    // compile would find nothing, so it fails here instead.
    private static CSharpCompilation ProbeCompilation(string solutionMembers)
    {
        var probe = CSharpCompilation.Create(
            "Probe",
            [CSharpSyntaxTree.ParseText(ProbeSource(solutionMembers), new CSharpParseOptions(LanguageVersion.CSharp14))],
            RepositoryCompilation.Solutions.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));

        AssertCompiles(probe);

        return probe;
    }

    private static void AssertCompiles(Compilation probe)
    {
        var errors = probe.GetDiagnostics().Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error).ToList();

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    // A problem's solution class, and a public constant at the tier's root as LeetCodeModulus is.
    private static string ProbeSource(string solutionMembers)
        => $$"""
            namespace DSAExperimentation.LeetCode.Solutions
            {
                public static class ProbeRoot
                {
                    public const long Prime = 1_000_000_007;
                }
            }

            namespace DSAExperimentation.LeetCode.Solutions.Probe
            {
                internal static class ProbeSolution
                {
            {{solutionMembers}}
                }
            }
            """;
}
