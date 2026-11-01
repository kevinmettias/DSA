using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordSearchBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to
// agreeing with each other, which a shared wrong verdict would pass. The board and word come from
// WordSearchWorkloads, whose word ends in a letter the board never holds - a construction its own
// tests pin - so the word cannot be traced, and that decisive false is pinned on the baseline.
public sealed partial class WordSearchBenchmarksTests
{
    private const int SmallestSide = 3;

    [Fact]
    public void CanTraceWordByBruteForceDfs_LastLetterOffTheBoard_CannotTraceTheWord() =>
        Assert.False(BuildHarness().CanTraceWordByBruteForceDfs());

    private static WordSearchBenchmarks BuildHarness()
    {
        var harness = new WordSearchBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
