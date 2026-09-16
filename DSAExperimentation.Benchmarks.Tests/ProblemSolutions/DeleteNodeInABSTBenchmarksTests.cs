using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodeInABSTBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - collecting the surviving keys and rebuilding a tree against the
// binary search tree's own in-place delete - so a harness whose arms disagree is timing two different
// problems. [GlobalSetup] shuffles 0..NodeCount-1 from one fixed seed, so the tree stays near-balanced,
// and targets the value NodeCount/2, which the shuffled run contains exactly once: deleting one node of
// a tree holding distinct keys leaves NodeCount-1 of them, which is the reading's documented shape. Each
// arm builds its own tree from that same shuffled order, so a delete never leaks into the next call.
public sealed partial class DeleteNodeInABSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    private const int ExpectedRemainingNodeCount = SmallestNodeCount - 1;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedRemainingNodeCount, BuildHarness().CollectFilterRebuild());
        Assert.Equal(BuildHarness().CollectFilterRebuild(), BuildHarness().CollectFilterRebuild());
    }

    [Fact]
    public void CollectFilterRebuild_FiveHundredShuffledKeys_AgreesWithBinarySearchTreeTryDelete()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRemainingNodeCount, harness.CollectFilterRebuild());
        Assert.Equal(harness.BinarySearchTreeTryDelete(), harness.CollectFilterRebuild());
    }

    [Fact]
    public void BinarySearchTreeTryDelete_FiveHundredShuffledKeys_AgreesWithCollectFilterRebuild()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedRemainingNodeCount, harness.BinarySearchTreeTryDelete());
        Assert.Equal(harness.CollectFilterRebuild(), harness.BinarySearchTreeTryDelete());
    }

    private static DeleteNodeInABSTBenchmarks BuildHarness()
    {
        var harness = new DeleteNodeInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
