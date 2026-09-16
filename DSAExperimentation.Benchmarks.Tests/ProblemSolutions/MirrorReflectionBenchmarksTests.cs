using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MirrorReflectionBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - naive O(roomSide) step-by-step unfolding against the Euclidean-GCD
// closed form - so a harness whose arms disagree is timing two different problems. Both arms take two
// ints and compute from them alone, so there is no workload to prepare and no state to share: one harness
// is safe to call twice in either order, and the two arms agree or they are answering different questions.
// RoomSide is pinned to the smallest of the benchmark's own [Params] values, and its LaserHeight is
// RoomSide - 1, the harness's own coprime pair.
public sealed partial class MirrorReflectionBenchmarksTests
{
    private const int SmallestRoomSide = 50_000;

    [Fact]
    public void SimulatedUnfolding_CoprimeRoomAndLaser_AgreesWithGcdClosedForm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GcdClosedForm(), harness.SimulatedUnfolding());
    }

    [Fact]
    public void GcdClosedForm_CoprimeRoomAndLaser_AgreesWithSimulatedUnfolding()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SimulatedUnfolding(), harness.GcdClosedForm());
    }

    private static MirrorReflectionBenchmarks BuildHarness() =>
        new() { RoomSide = SmallestRoomSide };
}
