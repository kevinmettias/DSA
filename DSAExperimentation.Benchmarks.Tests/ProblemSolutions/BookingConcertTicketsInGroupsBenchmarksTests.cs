using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BookingConcertTicketsInGroupsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - scanning rows for a run of free seats against a
// segment tree searched for the first row with the needed space - so a harness whose arms disagree
// is timing two different problems. Each arm returns a checksum folded over every gather and scatter
// answer, and each arm builds its own BookMyShow inside the call, so one arm's bookings cannot leak
// into the other's: the same script must replay to the same checksum. Setup draws that script from
// one fixed seed, so the same RowCount must rebuild the same script.
public sealed partial class BookingConcertTicketsInGroupsBenchmarksTests
{
    private const int SmallestRowCount = 200;

    [Fact]
    public void Setup_SameRowCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RowScan(), BuildHarness().RowScan());

    [Fact]
    public void RowScan_ThreeHundredGatherAndScatterCalls_AgreesWithSegmentTreeBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SegmentTreeBinarySearch(), harness.RowScan());
    }

    [Fact]
    public void SegmentTreeBinarySearch_ThreeHundredGatherAndScatterCalls_AgreesWithRowScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RowScan(), harness.SegmentTreeBinarySearch());
    }

    private static BookingConcertTicketsInGroupsBenchmarks BuildHarness()
    {
        var harness = new BookingConcertTicketsInGroupsBenchmarks { RowCount = SmallestRowCount };
        harness.Setup();

        return harness;
    }
}
