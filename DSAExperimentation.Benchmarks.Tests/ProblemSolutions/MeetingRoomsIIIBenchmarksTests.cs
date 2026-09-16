using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MeetingRoomsIIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - scanning a freeAt[] array once for the lowest free room and again
// for the soonest-to-free room against a free-room heap plus a busy-room heap - so a harness whose arms
// disagree is timing two different problems. Both arms return the busiest room index as an int, so they
// are compared directly, and both simulate on their own state built from the meeting array, so one
// harness is safe to read twice in either order. Setup generates the meetings from one fixed seed, so
// the same RoomCount must rebuild the same arrivals and with them the same busiest room.
public sealed partial class MeetingRoomsIIIBenchmarksTests
{
    private const int SmallestRoomCount = 20;

    [Fact]
    public void Setup_SameRoomCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScanFreeAt(), BuildHarness().LinearScanFreeAt());

    [Fact]
    public void LinearScanFreeAt_SeededMeetings_AgreesWithTwoHeapPool()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TwoHeapPool(), harness.LinearScanFreeAt());
    }

    [Fact]
    public void TwoHeapPool_SeededMeetings_AgreesWithLinearScanFreeAt()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanFreeAt(), harness.TwoHeapPool());
    }

    private static MeetingRoomsIIIBenchmarks BuildHarness()
    {
        var harness = new MeetingRoomsIIIBenchmarks { RoomCount = SmallestRoomCount };
        harness.Setup();

        return harness;
    }
}
