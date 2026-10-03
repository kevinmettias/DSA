using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WordSearchBenchmarks (ARCHITECTURE 17.9): both arms are competing tracers for
// one question on one board, so a harness whose arms disagree is timing two different problems.
// Neither arm mutates the board - both carry a separate used-path grid - so one harness instance is
// safe to call twice in either order. The board and word come from WordSearchWorkloads, whose word
// ends in a letter the board never holds - a construction its own tests pin - so the word cannot be
// traced, and that decisive false pins the agreement.
public sealed partial class WordSearchBenchmarksTests
{
    private const int SmallestSide = 3;

    [Fact]
    public void Setup_SameBoard_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerGraphText.Of(BuildHarness().CanTraceWordByBruteForceDfs()),
            AnswerGraphText.Of(BuildHarness().CanTraceWordByBruteForceDfs()));

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
    public void CanTraceWordByBruteForceDfs_LastLetterOffTheBoard_CannotTraceTheWord() =>
        Assert.False(BuildHarness().CanTraceWordByBruteForceDfs());

    [Fact]
    public void CanTraceWordByBacktrack_LastLetterOffTheBoard_CannotTraceTheWord() =>
        Assert.False(BuildHarness().CanTraceWordByBacktrack());

    private static WordSearchBenchmarks BuildHarness()
    {
        var harness = new WordSearchBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
