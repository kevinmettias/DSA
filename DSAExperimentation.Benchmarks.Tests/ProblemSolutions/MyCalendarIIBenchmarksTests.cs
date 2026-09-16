using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MyCalendarIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for the
// same question - the two-list scan against the two-interval-set scan - so a harness whose arms disagree is
// timing two different problems. Each arm builds its own mutable calendar inside the call and only reads the
// hoisted event array, so one harness instance is safe to call twice in either order. Setup derives the
// sliding-window event sequence from Length alone, so the same Length must rebuild the same bookings; otherwise
// two published numbers were never comparable in the first place.
//
// WEAK AGREEMENT, by construction. Both arms return one int - how many of the bookings were accepted - and the
// fixture's Stride < EventWidth spacing makes every event double-book with its immediate predecessor and nothing
// else, so that count is a summary of the acceptance decisions rather than of the double-booked intervals the two
// arms maintain to reach them. Agreement witnesses "both accepted exactly the same bookings", which does catch a
// permissiveness disagreement, but it cannot see a difference inside the stored double-booked ranges. The return
// type is the benchmark's to change, not this harness's.
public sealed partial class MyCalendarIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBookingSequence() =>
        Assert.Equal(BuildHarness().TwoListScan(), BuildHarness().TwoListScan());

    [Fact]
    public void TwoListScan_OverlappingBookingSequence_AgreesWithTwoIntervalSetScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoIntervalSetScan(), harness.TwoListScan());
    }

    [Fact]
    public void TwoIntervalSetScan_OverlappingBookingSequence_AgreesWithTwoListScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoListScan(), harness.TwoIntervalSetScan());
    }

    private static MyCalendarIIBenchmarks BuildHarness()
    {
        var harness = new MyCalendarIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
