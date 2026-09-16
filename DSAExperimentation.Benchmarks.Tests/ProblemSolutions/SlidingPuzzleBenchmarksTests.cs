using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SlidingPuzzleBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a BFS that copies and mutates the board per state against
// one BFS over the prepared 720-node board-permutation graph - so a harness whose arms
// disagree is solving two different puzzles. Setup builds the permutation graph once, and
// StartState is the only other input either arm takes.
//
// The smallest StartState is LeetCode's own one-move example, so both arms are additionally
// checked against the minimum the problem itself fixes for it: a start state one slide away
// from "123450" has exactly that one slide as its shortest solution, and no arm may report
// zero moves (already solved) or any number above one for it.
public sealed partial class SlidingPuzzleBenchmarksTests
{
    private const string OneMoveStartState = "123405";

    // LeetCode 773's first example board is one slide from the solved board, so one move is the
    // whole of what the minimum can be.
    private const int ExpectedMinMoves = 1;

    [Fact]
    public void Setup_SameStartState_RebuildsTheSamePuzzleGraph() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_OneMoveStartState_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
        Assert.Equal(ExpectedMinMoves, harness.MutationQueueBfs());
    }

    [Fact]
    public void ReduceGraphBfs_OneMoveStartState_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
        Assert.Equal(ExpectedMinMoves, harness.ReduceGraphBfs());
    }

    private static SlidingPuzzleBenchmarks BuildHarness()
    {
        var harness = new SlidingPuzzleBenchmarks { StartState = OneMoveStartState };
        harness.Setup();

        return harness;
    }
}
