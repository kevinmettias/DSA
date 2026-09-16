using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SmallestSubtreeWithAllTheDeepestNodesBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - a hand-rolled (depth, node)
// recursion against this repo's own TreeFold engine closed over the deepest-subtree algebra -
// so a harness whose arms disagree is folding two different trees. Setup builds the tree from
// a closed form with no draw from any stream, so the same NodeCount is the whole of what pins
// it.
//
// Both arms report the answer node's own value rather than the node, and Setup's tree gives
// node i the value i, so the two ints agreeing means the two arms picked the same node. The
// tree is complete, which makes the expected node derivable from the fixture's own layout:
// see the oracle below, which is what both arms are additionally checked against.
public sealed partial class SmallestSubtreeWithAllTheDeepestNodesBenchmarksTests
{
    private const int SmallestNodeCount = 2_000;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree() =>
        Assert.Equal(BuildHarness().HandRolledRecursion(), BuildHarness().HandRolledRecursion());

    [Fact]
    public void HandRolledRecursion_TwoThousandNodeCompleteTree_AgreesWithTreeFoldWithAlgebra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TreeFoldWithAlgebra(), harness.HandRolledRecursion());
        Assert.Equal(ExpectedDeepestSubtreeValue(), harness.TreeFoldWithAlgebra());
    }

    [Fact]
    public void TreeFoldWithAlgebra_TwoThousandNodeCompleteTree_AgreesWithHandRolledRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HandRolledRecursion(), harness.TreeFoldWithAlgebra());
        Assert.Equal(ExpectedDeepestSubtreeValue(), harness.HandRolledRecursion());
    }

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
