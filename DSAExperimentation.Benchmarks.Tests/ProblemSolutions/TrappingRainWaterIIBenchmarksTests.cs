using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TrappingRainWaterIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the relaxation sweep against the boundary
// heap flood fill - so a harness whose arms disagree is timing two different problems. Both arms
// return the trapped volume as an int, so they are compared directly. Setup draws the height map
// from a fixed seed, so the same Size must rebuild the same map.
public sealed partial class TrappingRainWaterIIBenchmarksTests
{
    private const int SmallestSize = 15;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameHeightMap() =>
        Assert.Equal(BuildHarness().RelaxationSweep(), BuildHarness().RelaxationSweep());

    [Fact]
    public void RelaxationSweep_SmallestSize_AgreesWithHeapFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HeapFloodFill(), harness.RelaxationSweep());
    }

    [Fact]
    public void HeapFloodFill_SmallestSize_AgreesWithRelaxationSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RelaxationSweep(), harness.HeapFloodFill());
    }

    private static TrappingRainWaterIIBenchmarks BuildHarness()
    {
        var harness = new TrappingRainWaterIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
