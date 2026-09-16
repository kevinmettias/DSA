using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumIceCreamBarsBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumIceCreamBarsSolution's competing strategies for one question - the quadratic repeated
// selection scan against one merge sort followed by a single greedy pass - so a harness whose arms
// disagree is timing two different problems. Both answer with the number of bars bought, so the
// two returns are compared directly.
public sealed partial class MaximumIceCreamBarsBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameCostsAndBudget()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The cost array and the coin budget are both private, so the rebuild is pinned through
        // the purchase count they produce: the same Length must draw the same seeded costs and
        // derive the same budget from them.
        Assert.Equal(first.SelectionScan(), second.SelectionScan());
        Assert.Equal(first.MergeSortGreedy(), second.MergeSortGreedy());
    }

    [Fact]
    public void SelectionScan_SeededCostsAndBudget_AgreesWithMergeSortGreedy()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortGreedy(), harness.SelectionScan());
    }

    [Fact]
    public void MergeSortGreedy_SeededCostsAndBudget_AgreesWithSelectionScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SelectionScan(), harness.MergeSortGreedy());
    }

    private static MaximumIceCreamBarsBenchmarks BuildHarness()
    {
        var harness = new MaximumIceCreamBarsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
