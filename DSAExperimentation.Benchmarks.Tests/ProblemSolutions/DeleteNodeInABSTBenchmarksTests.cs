using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteNodeInABSTBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin:
// which keys the tree holds after the delete, known from Setup's construction rather than from either arm.
// [GlobalSetup] shuffles 0..NodeCount-1 from one fixed seed and targets the value NodeCount/2, which the shuffled
// run contains exactly once, so the answer holds every other key and only those. The arms legitimately answer with
// differently shaped trees - one rebuilds, one deletes in place - so ArmAgreement leaves their shapes uncompared,
// and this key set is what holds them to the same answer. Each arm returns its tree as object? (the tree type is
// internal, CS0050).
public sealed partial class DeleteNodeInABSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;

    private const int ExpectedRemainingNodeCount = SmallestNodeCount - 1;

    private const int DeletedKey = SmallestNodeCount / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void CollectFilterRebuild_FiveHundredShuffledKeys_HoldsEveryKeyButTheTarget() =>
        AssertHoldsEveryKeyButTheTarget(BuildHarness().CollectFilterRebuild());

    [Fact]
    public void BinarySearchTreeTryDelete_FiveHundredShuffledKeys_HoldsEveryKeyButTheTarget() =>
        AssertHoldsEveryKeyButTheTarget(BuildHarness().BinarySearchTreeTryDelete());

    private static void AssertHoldsEveryKeyButTheTarget(object? answer)
    {
        var tree = Assert.IsType<BinarySearchTree<int>>(answer);

        Assert.Equal(ExpectedRemainingNodeCount, tree.Count);
        Assert.False(tree.Has(DeletedKey));
        Assert.All(Enumerable.Range(0, SmallestNodeCount).Where(key => key != DeletedKey), key => Assert.True(tree.Has(key)));
    }

    private static DeleteNodeInABSTBenchmarks BuildHarness()
    {
        var harness = new DeleteNodeInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
