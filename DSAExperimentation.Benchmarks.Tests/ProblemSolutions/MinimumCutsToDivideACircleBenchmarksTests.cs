using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCutsToDivideACircleBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an O(n) simulation that places one cut at a time
// against the O(1) parity read-off - so a harness whose arms disagree is timing two different
// questions. The class carries no [GlobalSetup]: both arms are handed the same Slices directly, per
// the problem's own argument shape. Slices is odd, and an odd slice count can never pair opposite
// slices across the centre, so every cut must be a single radius cut and the answer is the slice
// count itself - a decisive value rather than a self-consistent one.
public sealed partial class MinimumCutsToDivideACircleBenchmarksTests
{
    private const int SmallestSlices = 101;

    [Fact]
    public void SimulateOneCutAtATime_OddSliceCount_NeedsOneCutPerSliceAndAgreesWithClosedFormParityCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestSlices, harness.SimulateOneCutAtATime());
        Assert.Equal(harness.ClosedFormParityCheck(), harness.SimulateOneCutAtATime());
    }

    [Fact]
    public void ClosedFormParityCheck_OddSliceCount_NeedsOneCutPerSliceAndAgreesWithSimulateOneCutAtATime()
    {
        var harness = BuildHarness();

        Assert.Equal(SmallestSlices, harness.ClosedFormParityCheck());
        Assert.Equal(harness.SimulateOneCutAtATime(), harness.ClosedFormParityCheck());
    }

    private static MinimumCutsToDivideACircleBenchmarks BuildHarness() =>
        new() { Slices = SmallestSlices };
}
