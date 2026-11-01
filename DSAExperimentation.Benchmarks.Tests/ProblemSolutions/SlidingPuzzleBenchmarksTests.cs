using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SlidingPuzzleBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the answer the workload is built to have. The smallest
// StartState is LeetCode's own one-move example, so the baseline is checked against the minimum
// the problem itself fixes for it: a start state one slide away from "123450" has exactly that one
// slide as its shortest solution, and no arm may report zero moves (already solved) or any number
// above one for it.
public sealed partial class SlidingPuzzleBenchmarksTests
{
    private const string OneMoveStartState = "123405";

    // LeetCode 773's first example board is one slide from the solved board, so one move is the
    // whole of what the minimum can be.
    private const int ExpectedMinMoves = 1;

    [Fact]
    public void MutationQueueBfs_OneMoveStartState_SolvesInOneMove() =>
        Assert.Equal(ExpectedMinMoves, BuildHarness().MutationQueueBfs());

    private static SlidingPuzzleBenchmarks BuildHarness()
    {
        var harness = new SlidingPuzzleBenchmarks { StartState = OneMoveStartState };
        harness.Setup();

        return harness;
    }
}
