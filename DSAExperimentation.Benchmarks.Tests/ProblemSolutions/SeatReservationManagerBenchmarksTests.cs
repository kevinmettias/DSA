using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SeatReservationManagerBenchmarks (ARCHITECTURE 17.9): both arms replay the
// identical reserve / unreserve-every-third / reserve script against their own manager, so a harness
// whose arms disagree is timing two different scripts. Each arm constructs its mutable subject inside
// the call - the scan arm its own sized array manager, the heap arm its own released-seat heap - so
// one harness instance is safe to call twice in either order, and the field holding the unreserve
// targets is only read. Setup derives those targets from OperationCount alone, so the same
// OperationCount must rebuild the same script.
public sealed partial class SeatReservationManagerBenchmarksTests
{
    private const int SmallestOperationCount = 200;

    [Fact]
    public void Setup_SameOperationCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().LinearScanArray(), BuildHarness().LinearScanArray());

    [Fact]
    public void LinearScanArray_ReserveUnreserveReserve_AgreesWithReleasedSeatHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReleasedSeatHeap(), harness.LinearScanArray());
    }

    [Fact]
    public void ReleasedSeatHeap_ReserveUnreserveReserve_AgreesWithLinearScanArray()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScanArray(), harness.ReleasedSeatHeap());
    }

    private static SeatReservationManagerBenchmarks BuildHarness()
    {
        var harness = new SeatReservationManagerBenchmarks { OperationCount = SmallestOperationCount };
        harness.Setup();

        return harness;
    }
}
