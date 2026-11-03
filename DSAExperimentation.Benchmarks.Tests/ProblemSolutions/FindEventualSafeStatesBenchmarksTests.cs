using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindEventualSafeStatesBenchmarks (ARCHITECTURE 17.9): the generic arms check
// only compares the arms with each other, so this pins the baseline's safe ids, ascending as the
// problem fixes them, to the set Setup's layout determines independently of any arm.
//
// Setup splits the nodes in half: [0, half) is a forward DAG whose every node reaches the terminal
// node half - 1 (its own fan-out shrinks to nothing there), and [half, NodeCount) is one ring cycle
// whose every node loops forever. Every node in the first half is therefore safe and every node in
// the second is not, which makes the answer decisive rather than a happy accident of the seed.
public sealed partial class FindEventualSafeStatesBenchmarksTests
{
    // The smaller of Setup's [Params(50, 1_000)] node counts.
    private const int SmallestNodeCount = 50;

    [Fact]
    public void DfsThreeColoring_SafeForwardHalfAndRingCycle_ReturnsTheForwardHalfAscending() =>
        Assert.Equal(AnswerGraphText.Of(ExpectedSafeNodes()), AnswerGraphText.Of(BuildHarness().DfsThreeColoring()));

    private static FindEventualSafeStatesBenchmarks BuildHarness()
    {
        var harness = new FindEventualSafeStatesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // Setup's first half, read off its own layout: node i points only at higher ids inside
    // [0, half), so the walk always terminates at the node whose fan-out shrinks to zero. The
    // second half is a single ring and never peels, so no id at or above half can be safe.
    private static int[] ExpectedSafeNodes() =>
        Enumerable.Range(0, SmallestNodeCount / AlgorithmConstants.HalvingFactor).ToArray();
}
