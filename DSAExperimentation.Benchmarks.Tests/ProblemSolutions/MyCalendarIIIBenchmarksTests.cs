using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MyCalendarIIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for the
// same question - rescanning every start point seen so far against the HashMap<int,int> delta sweep re-sorted by
// MergeSort - so a harness whose arms disagree is timing two different problems. Each arm builds its own mutable
// calendar inside the call and only reads the hoisted booking array, so one harness instance is safe to call twice
// in either order. Setup draws those bookings from one fixed seed, so the same Length must rebuild the same
// sequence; otherwise two published numbers were never comparable in the first place.
//
// Agreement here is on the running maximum the final Book call returns. Bookings are never removed, so that last
// value is also the maximum over the whole run - the two arms are asked one comparable int, though it is a summary
// of the sequence rather than the per-call sequence itself.
public sealed partial class MyCalendarIIIBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameBookingSequence() =>
        Assert.Equal(BuildHarness().BruteForceEventRescan(), BuildHarness().BruteForceEventRescan());

    [Fact]
    public void BruteForceEventRescan_RandomOverlappingBookings_AgreesWithHashMapMergeSortSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapMergeSortSweep(), harness.BruteForceEventRescan());
    }

    [Fact]
    public void HashMapMergeSortSweep_RandomOverlappingBookings_AgreesWithBruteForceEventRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceEventRescan(), harness.HashMapMergeSortSweep());
    }

    private static MyCalendarIIIBenchmarks BuildHarness()
    {
        var harness = new MyCalendarIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
