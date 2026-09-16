using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ZumaGameBenchmarks (ARCHITECTURE 17.9): both arms are competing search orders
// over one (board, hand) state space, so a harness whose arms disagree is timing two different
// problems. The board repeats a unit with no run long enough to collapse on its own, and the
// baseline is asserted against the same decisive minimum the class comment names for both sizes -
// an escape hatch that reported "no solution" would otherwise agree with a correct arm only by
// accident.
public sealed partial class ZumaGameBenchmarksTests
{
    private const int SmallestBoardRepeats = 3;
    private const int ExpectedMinimumSteps = 3;

    [Fact]
    public void Setup_SameBoardRepeats_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForceDfs()),
            AnswerText.Of(BuildHarness().BruteForceDfs()));

    [Fact]
    public void BruteForceDfs_AgreesWithQueueBfsDedup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDfs(), harness.QueueBfsDedup());
    }

    [Fact]
    public void QueueBfsDedup_AgreesWithBruteForceDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueBfsDedup(), harness.BruteForceDfs());
    }

    [Fact]
    public void BruteForceDfs_OddBoardRepeats_NeedsTheDocumentedMinimumSteps() =>
        Assert.Equal(ExpectedMinimumSteps, BuildHarness().BruteForceDfs());

    [Fact]
    public void QueueBfsDedup_OddBoardRepeats_NeedsTheDocumentedMinimumSteps() =>
        Assert.Equal(ExpectedMinimumSteps, BuildHarness().QueueBfsDedup());

    private static ZumaGameBenchmarks BuildHarness()
    {
        var harness = new ZumaGameBenchmarks { BoardRepeats = SmallestBoardRepeats };
        harness.Setup();

        return harness;
    }
}
