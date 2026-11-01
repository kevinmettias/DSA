using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInABinarySearchTreeBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the workload's shape.
// Setup builds the tree from a shuffled insertion order and aims at the highest value - and since
// the tree holds every value 1..NodeCount, that highest value is present and the node holding it is
// the answer.
public sealed partial class SearchInABinarySearchTreeBenchmarksTests
{
    private const int SmallestNodeCount = 500;
    private const int HighestValue = SmallestNodeCount;

    [Fact]
    public void LinearScan_SeededTree_FindsTheHighestValue() =>
        Assert.Equal(HighestValue, BuildHarness().LinearScan()?.Value);

    private static SearchInABinarySearchTreeBenchmarks BuildHarness()
    {
        var harness = new SearchInABinarySearchTreeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
