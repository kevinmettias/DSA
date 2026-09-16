using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumProfitFromValidTopologicalOrderInDagBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the backtracking search over valid
// topological orders against the bitmask memoization over predecessor masks - so a harness whose arms
// disagree is timing two different problems. Setup builds both the seeded DAG and the PrecedenceMasks
// it feeds the memoized arm, so the same NodeCount must rebuild the same workload; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class MaximumProfitFromValidTopologicalOrderInDagBenchmarksTests
{
    private const int SmallestNodeCount = 6;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Backtracking(), BuildHarness().Backtracking());

    [Fact]
    public void Backtracking_SixNodeDag_AgreesWithBitmaskMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemoization(), harness.Backtracking());
    }

    [Fact]
    public void BitmaskMemoization_SixNodeDag_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Backtracking(), harness.BitmaskMemoization());
    }

    private static MaximumProfitFromValidTopologicalOrderInDagBenchmarks BuildHarness()
    {
        var harness = new MaximumProfitFromValidTopologicalOrderInDagBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
