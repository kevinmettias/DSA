using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordSearchBenchmarks (ARCHITECTURE 17.9): both arms are competing tracers for
// one question on one board, so a harness whose arms disagree is timing two different problems.
// Neither arm mutates the board - both carry a separate used-path grid - so one harness instance is
// safe to call twice in either order. The board and word are LC 79's own example, where the word is
// traceable, which is the decisive value pinning the agreement.
public sealed partial class WordSearchBenchmarksTests
{
    [Fact]
    public void Setup_SameBoard_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanTraceWordByBruteForceDfs()),
            AnswerText.Of(BuildHarness().CanTraceWordByBruteForceDfs()));

    [Fact]
    public void CanTraceWordByBruteForceDfs_AgreesWithBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanTraceWordByBruteForceDfs(), harness.CanTraceWordByBacktrack());
    }

    [Fact]
    public void CanTraceWordByBacktrack_AgreesWithBruteForceDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanTraceWordByBacktrack(), harness.CanTraceWordByBruteForceDfs());
    }

    [Fact]
    public void CanTraceWordByBruteForceDfs_LetterCellExample_TracesTheWord() =>
        Assert.True(BuildHarness().CanTraceWordByBruteForceDfs());

    [Fact]
    public void CanTraceWordByBacktrack_LetterCellExample_TracesTheWord() =>
        Assert.True(BuildHarness().CanTraceWordByBacktrack());

    private static WordSearchBenchmarks BuildHarness()
    {
        var harness = new WordSearchBenchmarks();
        harness.Setup();

        return harness;
    }
}
