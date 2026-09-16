using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinarySearchTreeIteratorBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from the arm's own declared contract instead - [Benchmark] drains a fresh iterator end to end and
// reports the last value it saw. Fixtures.BinaryTrees.Balanced makes that value decisive: it builds
// the complete tree in heap layout, node i's children at 2i + 1 and 2i + 2 with the value equal to
// the index, so an in-order drain ends on the bottom of the right spine, which is the largest index
// still inside the node count. Setup is unseeded but fully determined by NodeCount, so the same
// NodeCount must rebuild the same tree and therefore report the same final value.
public sealed partial class BinarySearchTreeIteratorBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DrainInOrder(), BuildHarness().DrainInOrder());

    [Fact]
    public void DrainInOrder_CompleteTreeWithTwoHundredNodes_EndsOnTheRightSpine()
    {
        var harness = BuildHarness();

        Assert.Equal(RightSpineTerminal(SmallestNodeCount), harness.DrainInOrder());
    }

    private static BinarySearchTreeIteratorBenchmarks BuildHarness()
    {
        var harness = new BinarySearchTreeIteratorBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }

    // In-order visits a node's left subtree, then the node, then its right subtree, so the value it
    // reports last is the one at the bottom of the right spine; in this fixture's heap layout that
    // spine is the chain 0, 2, 6, 14, ... and it ends at the last index that still holds a node.
    private static int RightSpineTerminal(int nodeCount)
    {
        var index = 0;

        while (RightChild(index) < nodeCount)
        {
            index = RightChild(index);
        }

        return index;
    }

    // The fixture places node i's children at BranchingFactor * i + 1 and the index right after
    // that, the same arithmetic Fixtures.BinaryTrees.Balanced itself writes out.
    private static int RightChild(int index)
    {
        var leftChild = (AlgorithmConstants.BranchingFactor * index) + 1;

        return leftChild + 1;
    }
}
