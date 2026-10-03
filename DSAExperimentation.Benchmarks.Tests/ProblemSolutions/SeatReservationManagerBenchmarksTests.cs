using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SeatReservationManagerBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins every seat the script hands out, from the script alone.
//
// The first pass reserves seats 1..OperationCount in order. Unreserving every third of them frees
// 3, 6, ..., and LC 1845 reserves the smallest unreserved seat, so the second pass takes those
// freed seats back in ascending order before it reaches the never-issued seats past
// OperationCount.
public sealed partial class SeatReservationManagerBenchmarksTests
{
    private const int SmallestOperationCount = 200;
    private const int UnreserveStride = 3;

    [Fact]
    public void LinearScanArray_ReserveReleaseReserveScript_HandsOutTheSmallestFreeSeatEachTime() =>
        Assert.Equal(ExpectedSeats(), BuildHarness().LinearScanArray());

    [Fact]
    public void ReleasedSeatHeap_ReserveReleaseReserveScript_HandsOutTheSmallestFreeSeatEachTime() =>
        Assert.Equal(ExpectedSeats(), BuildHarness().ReleasedSeatHeap());

    private static int[] ExpectedSeats()
    {
        var freed = SmallestOperationCount / UnreserveStride;
        var firstPass = Enumerable.Range(1, SmallestOperationCount);
        var freedSeats = Enumerable.Range(1, freed).Select(i => i * UnreserveStride);
        var freshSeats = Enumerable.Range(SmallestOperationCount + 1, SmallestOperationCount - freed);

        return [.. firstPass, .. freedSeats, .. freshSeats];
    }

    private static SeatReservationManagerBenchmarks BuildHarness()
    {
        var harness = new SeatReservationManagerBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
