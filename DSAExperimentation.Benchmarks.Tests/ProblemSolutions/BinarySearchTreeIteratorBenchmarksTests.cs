using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BinarySearchTreeIteratorBenchmarks (ARCHITECTURE 17.9): the class has a
// single arm, so there is no second strategy to reconcile it against and the assertion has to come
// from the workload's construction instead - [Benchmark] drains a fresh iterator end to end and
// returns every value it saw. Setup labels each node of the complete tree with its in-order rank,
// so the tree is a binary search tree over 0..n-1 and an in-order drain must report exactly those
// values in ascending order - every node once, none out of place.
public sealed partial class BinarySearchTreeIteratorBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void DrainInOrder_CompleteSearchTreeWithTwoHundredNodes_ReportsEveryValueInAscendingOrder() =>
        Assert.Equal(Enumerable.Range(0, SmallestNodeCount), BuildHarness().DrainInOrder());

    private static BinarySearchTreeIteratorBenchmarks BuildHarness()
    {
        var harness = new BinarySearchTreeIteratorBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
