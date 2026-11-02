using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for KthSmallestElementInABSTBenchmarks (ARCHITECTURE 17.9). Arm agreement,
// rebuild determinism and repeat calls on one harness are BenchmarkArmsTests' job; this pins what
// the generic check cannot know.
//
// The expected value is decisive and independent of either walk: Setup inserts the seeded shuffle
// of 1..NodeCount (all distinct) into a BinarySearchTree, so the in-order sequence is exactly
// 1..NodeCount and the targetRank = NodeCount / 2 th smallest value is NodeCount / 2 itself.
public sealed partial class KthSmallestElementInABSTBenchmarksTests
{
    private const int SmallestNodeCount = 500;
    private const int ExpectedRankValue = SmallestNodeCount / AlgorithmConstants.HalvingFactor;

    [Fact]
    public void RecursiveInOrderWalk_ShuffledTree_ReportsTheMedianRankValue() =>
        Assert.Equal(ExpectedRankValue, BuildHarness().RecursiveInOrderWalk());

    private static KthSmallestElementInABSTBenchmarks BuildHarness()
    {
        var harness = new KthSmallestElementInABSTBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
