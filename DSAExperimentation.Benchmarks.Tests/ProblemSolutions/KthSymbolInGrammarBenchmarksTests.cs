using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSymbolInGrammarBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the symbol at one position of a generated row - so a
// harness whose arms disagree is timing two different problems. Both arms answer with the bare
// symbol, and Setup fixes the index at the last position of the row, which is the deepest
// recursion the halving walk can be asked for.
//
// That last position also gives the assertion a decisive value independent of either arm: LC 779
// replaces every 0 with 01 and every 1 with 10, so the row's second half is the complement of its
// first and the last symbol flips with every row - the first row's last symbol being 0.
public sealed partial class KthSymbolInGrammarBenchmarksTests
{
    private const int SmallestRowNumber = 10;
    private const int FirstRowNumber = 1;
    private const int FirstRowLastSymbol = 0;

    [Fact]
    public void Setup_SameRowNumber_RebuildsTheSameLastIndex()
    {
        var expected = LastSymbolOfRow(SmallestRowNumber);

        Assert.Equal(expected, BuildHarness().BuildFullRow());
        Assert.Equal(expected, BuildHarness().RecursiveHalving());
    }

    [Fact]
    public void BuildFullRow_LastSymbolOfRow_AgreesWithRecursiveHalving()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveHalving(), harness.BuildFullRow());
    }

    [Fact]
    public void RecursiveHalving_LastSymbolOfRow_AgreesWithBuildFullRow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BuildFullRow(), harness.RecursiveHalving());
    }

    private static KthSymbolInGrammarBenchmarks BuildHarness()
    {
        var harness = new KthSymbolInGrammarBenchmarks { RowNumber = SmallestRowNumber };
        harness.Setup();

        return harness;
    }

    // Counting the complementation steps directly - the last symbol is in the second half of every
    // row from the second one onward, so it flips once per row - rather than asking either arm.
    private static int LastSymbolOfRow(int rowNumber) =>
        (FirstRowLastSymbol + rowNumber - FirstRowNumber) % 2;
}
