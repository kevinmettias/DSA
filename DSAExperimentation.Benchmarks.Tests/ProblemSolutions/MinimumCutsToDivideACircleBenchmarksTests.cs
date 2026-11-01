using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumCutsToDivideACircleBenchmarks (ARCHITECTURE 17.9): the class carries
// no [GlobalSetup]: the arms are handed Slices directly, per the problem's own argument shape. Slices
// is odd, and an odd slice count can never pair opposite slices across the centre, so every cut must
// be a single radius cut and the answer is the slice count itself - a decisive value rather than a
// self-consistent one, which the generic agreement check cannot supply.
public sealed partial class MinimumCutsToDivideACircleBenchmarksTests
{
    private const int SmallestSlices = 11;

    [Fact]
    public void SimulateOneCutAtATime_OddSliceCount_NeedsOneCutPerSlice() =>
        Assert.Equal(SmallestSlices, BuildHarness().SimulateOneCutAtATime());

    private static MinimumCutsToDivideACircleBenchmarks BuildHarness() =>
        new() { Slices = SmallestSlices };
}
