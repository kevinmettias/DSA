using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistributeRepeatingIntegersBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a hand-written choose/explore/unchoose recursion
// against this repo's Backtrack.TrySearch closed over those same steps - so a harness whose arms
// disagree is timing two different problems. The verdict is a bare bool, so the class comment's own
// guarantee is what keeps agreement from being vacuous: the stock buckets were filled with the
// shared sum of the orders, so a valid distribution always exists and every arm must answer true.
// Setup shuffles one fixed order multiset and rebuilds one fixed stock, so the same ValueCount must
// reach that same verdict. Both arms copy the prepared stock before searching it, so one harness is
// safe to read twice in either order.
public sealed partial class DistributeRepeatingIntegersBenchmarksTests
{
    private const int SmallestValueCount = 3;
    private const bool ExpectedDistributabilityForTheSharedStockWorkload = true;

    [Fact]
    public void Setup_SharedStockBuckets_PlaceEveryOrderAndRebuildTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDistributabilityForTheSharedStockWorkload, harness.CanDistributeByNaiveBacktracking());
        Assert.Equal(harness.CanDistributeByNaiveBacktracking(), BuildHarness().CanDistributeByNaiveBacktracking());
    }

    [Fact]
    public void CanDistributeByNaiveBacktracking_SharedStockBuckets_AnswersTrueAndAgreesWithGenericBacktrack()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDistributabilityForTheSharedStockWorkload, harness.CanDistributeByNaiveBacktracking());
        Assert.Equal(harness.CanDistributeByGenericBacktrack(), harness.CanDistributeByNaiveBacktracking());
    }

    [Fact]
    public void CanDistributeByGenericBacktrack_SharedStockBuckets_AnswersTrueAndAgreesWithNaiveBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDistributabilityForTheSharedStockWorkload, harness.CanDistributeByGenericBacktrack());
        Assert.Equal(harness.CanDistributeByNaiveBacktracking(), harness.CanDistributeByGenericBacktrack());
    }

    private static DistributeRepeatingIntegersBenchmarks BuildHarness()
    {
        var harness = new DistributeRepeatingIntegersBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
