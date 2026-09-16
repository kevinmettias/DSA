using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PopulatingNextRightPointersInEachNodeBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for one question - the node-to-next-right-neighbor map of a perfect
// tree - so a harness whose arms disagree is timing two different problems. NodeCount is the only
// [Params] axis and Setup builds the tree from it, so the same NodeCount must rebuild the same tree.
//
// Both arms return only .Count of that map, because BinaryTreeNode<int> is internal and a public
// [Benchmark] method cannot return a type built over it. That is a proxy: two arms that linked the
// nodes differently but visited the same number of them would still agree. The count is at least the
// one thing the fixture pins - BinaryTrees.Balanced(n) allocates exactly n nodes - so each arm is
// also asserted against that independently derived total, which an arm that dropped a node fails.
public sealed partial class PopulatingNextRightPointersInEachNodeBenchmarksTests
{
    private const int SmallestNodeCount = 63;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameTree() =>
        Assert.Equal(BuildHarness().ManualQueueBfs(), BuildHarness().ManualQueueBfs());

    [Fact]
    public void ManualQueueBfs_AgreesWithLevelGroupedTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LevelGroupedTraversal(), harness.ManualQueueBfs());
    }

    [Fact]
    public void LevelGroupedTraversal_AgreesWithManualQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ManualQueueBfs(), harness.LevelGroupedTraversal());
    }

    [Fact]
    public void ManualQueueBfs_PerfectTree_LinksEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().ManualQueueBfs());

    [Fact]
    public void LevelGroupedTraversal_PerfectTree_LinksEveryNode() =>
        Assert.Equal(SmallestNodeCount, BuildHarness().LevelGroupedTraversal());

    private static PopulatingNextRightPointersInEachNodeBenchmarks BuildHarness()
    {
        var harness = new PopulatingNextRightPointersInEachNodeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
