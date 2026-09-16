using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignAFoodRatingSystemBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning every food on each highest-rated query against a
// per-cuisine heap that discards superseded ratings lazily - so a harness whose arms disagree is timing
// two different problems. Setup builds the constructor's foods/cuisines/ratings, one superseding rating
// per food and the query cuisines, from one fixed seed. Each arm builds its own subject inside the call
// and replays the same script over it, so a single harness is safe to call in either order. The reading
// is the summed length of every reported food name - the harness's own aggregate, chosen so the replay
// cannot be eliminated as dead code - which is bounded by one reported name per query rather than being
// the whole answer; the same Count must rebuild the same script and with it the same sum.
public sealed partial class DesignAFoodRatingSystemBenchmarksTests
{
    private const int SmallestCount = 200;

    // The documented shape of a reported name: "food{i}" over a Count below four digits.
    private const int MaxReportedNameLength = 8;

    // A cuisine with nothing rated reports no name at all.
    private const int MinimumReportedNameLengthSum = 0;
    private const int MaximumReportedNameLengthSum = SmallestCount * MaxReportedNameLength;

    [Fact]
    public void Setup_SameCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().LinearScan(),
            MinimumReportedNameLengthSum,
            MaximumReportedNameLengthSum);
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_TwoHundredSeededFoods_AgreesWithLazyDeletionHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LazyDeletionHeap(), harness.LinearScan());
    }

    [Fact]
    public void LazyDeletionHeap_TwoHundredSeededFoods_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.LazyDeletionHeap());
    }

    private static DesignAFoodRatingSystemBenchmarks BuildHarness()
    {
        var harness = new DesignAFoodRatingSystemBenchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
