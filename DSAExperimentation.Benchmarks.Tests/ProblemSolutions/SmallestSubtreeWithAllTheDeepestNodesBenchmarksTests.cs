using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestSubtreeWithAllTheDeepestNodesBenchmarks (ARCHITECTURE 17.9). Arm
// agreement is BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the
// workload's shape. The arms return the answer node itself, and Setup's tree gives node i the
// value i, so the node's value names which node an arm picked. The tree is complete, which makes
// the expected node derivable from the fixture's own layout: see the oracle below, which is what
// the baseline is checked against.
public sealed partial class SmallestSubtreeWithAllTheDeepestNodesBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void HandRolledRecursion_FiftyNodeCompleteTree_PicksTheDeepestLevelsAncestor() =>
        Assert.Equal(ExpectedDeepestSubtreeValue(), BuildHarness().HandRolledRecursion()?.Value);

    // The smallest subtree holding every deepest node is the lowest common ancestor of the
    // deepest level's two endpoints, since a complete tree fills that level contiguously. Setup
    // gives node i the value i, so the value that ancestor reports is its own heap index, which
    // this walks out from the fixture's documented layout alone - the leftmost path down to the
    // deepest level for one endpoint, the last index for the other, then both lifted together.
    private static int ExpectedDeepestSubtreeValue()
    {
        var leftEndpoint = DeepestLevelFirstIndex();
        var rightEndpoint = SmallestNodeCount - 1;

        while (leftEndpoint != rightEndpoint)
        {
            leftEndpoint = ParentOf(leftEndpoint);
            rightEndpoint = ParentOf(rightEndpoint);
        }

        return leftEndpoint;
    }

    private static int DeepestLevelFirstIndex()
    {
        var index = 0;
        var leftChild = (AlgorithmConstants.BranchingFactor * index) + 1;

        while (leftChild < SmallestNodeCount)
        {
            index = leftChild;
            leftChild = (AlgorithmConstants.BranchingFactor * index) + 1;
        }

        return index;
    }

    private static int ParentOf(int index) => (index - 1) / AlgorithmConstants.BranchingFactor;

    private static SmallestSubtreeWithAllTheDeepestNodesBenchmarks BuildHarness()
    {
        var harness = new SmallestSubtreeWithAllTheDeepestNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
