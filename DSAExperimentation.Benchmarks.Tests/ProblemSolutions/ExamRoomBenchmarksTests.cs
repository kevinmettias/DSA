using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ExamRoomBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - LinearScanList's O(n) List<T>.Remove against BinarySearchDynamicArray's
// O(log n) lower-bound locate - so a harness whose arms disagree is running two different rooms. The
// room is stateful and rebuilt per invocation, so Setup fixes only the workload size: the room is
// padded beyond Length so the end-of-room gap is never the binding constraint, which means every one
// of the Length seats handed out has to land inside the padded capacity.
public sealed partial class ExamRoomBenchmarksTests
{
    private const int SmallestLength = 200;

    // The same padding the harness appends beyond Length, restated here so the seat-number contract
    // below can be stated without reaching into the harness.
    private const int RoomCapacityPadding = 1_000;
    private const int MaxSeatNumber = SmallestLength + RoomCapacityPadding - 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameSeatScript()
    {
        Assert.InRange(BuildHarness().LinearScanList(), 0, MaxSeatNumber);

        Assert.Equal(BuildHarness().LinearScanList(), BuildHarness().LinearScanList());
    }

    [Fact]
    public void LinearScanList_DrainedPaddedRoom_AgreesWithBinarySearchDynamicArray()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchDynamicArray(), harness.LinearScanList());
    }

    [Fact]
    public void BinarySearchDynamicArray_DrainedPaddedRoom_AgreesWithLinearScanList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanList(), harness.BinarySearchDynamicArray());
    }

    private static ExamRoomBenchmarks BuildHarness()
    {
        var harness = new ExamRoomBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
