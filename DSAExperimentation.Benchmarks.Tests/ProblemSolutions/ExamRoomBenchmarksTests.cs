using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExamRoomBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: where the
// seats land, bounded by Setup's construction rather than by either arm. The room is stateful and rebuilt per
// invocation, so Setup fixes only the workload size: the room is padded beyond Length so the end-of-room gap is
// never the binding constraint, which means every one of the Length seats handed out has to land inside the
// padded capacity. Each arm returns every seat Seat() handed out, in order.
public sealed partial class ExamRoomBenchmarksTests
{
    private const int SmallestLength = 200;

    // The same padding the harness appends beyond Length, restated here so the seat-number contract
    // below can be stated without reaching into the harness.
    private const int RoomCapacityPadding = 1_000;
    private const int MaxSeatNumber = SmallestLength + RoomCapacityPadding - 1;

    [Fact]
    public void LinearScanList_DrainedPaddedRoom_SeatsEveryStudentInsideTheRoom() =>
        AssertSeatsEveryStudentInsideTheRoom(BuildHarness().LinearScanList());

    [Fact]
    public void BinarySearchDynamicArray_DrainedPaddedRoom_SeatsEveryStudentInsideTheRoom() =>
        AssertSeatsEveryStudentInsideTheRoom(BuildHarness().BinarySearchDynamicArray());

    // No two students present at once share a seat, and nobody leaves until all are seated.
    private static void AssertSeatsEveryStudentInsideTheRoom(int[] seats)
    {
        Assert.All(seats, seat => Assert.InRange(seat, 0, MaxSeatNumber));
        Assert.Equal(SmallestLength, seats.Distinct().Count());
    }

    private static ExamRoomBenchmarks BuildHarness()
    {
        var harness = new ExamRoomBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
