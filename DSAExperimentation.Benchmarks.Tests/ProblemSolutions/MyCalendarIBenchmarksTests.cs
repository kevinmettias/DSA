using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MyCalendarIBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for the
// same question - the linear overlap scan against the interval set's binary search - so a harness whose arms
// disagree is timing two different problems. Each arm builds its own mutable calendar inside the call and only
// reads the hoisted event array, so one harness instance is safe to call twice in either order. Setup derives the
// non-overlapping event sequence from Length alone, so the same Length must rebuild the same bookings; otherwise
// two published numbers were never comparable in the first place.
//
// WEAK AGREEMENT, by construction. Both arms return one int - how many of the bookings were accepted - and the
// fixture's Stride > EventWidth spacing makes every booking disjoint, so the only answer the workload can carry
// is the full Length. Agreement therefore witnesses "both accepted the same number of bookings", not "both kept
// the same set of intervals": an arm that stored a different-but-equally-permissive calendar would pass. That is
// the count the benchmark chose to measure, so the honest assertion is used rather than a stronger one the return
// type cannot support.
public sealed partial class MyCalendarIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBookingSequence() =>
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());

    [Fact]
    public void LinearScan_DisjointBookingSequence_AgreesWithIntervalSetBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetBinarySearch(), harness.LinearScan());
    }

    [Fact]
    public void IntervalSetBinarySearch_DisjointBookingSequence_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.IntervalSetBinarySearch());
    }

    private static MyCalendarIBenchmarks BuildHarness()
    {
        var harness = new MyCalendarIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
