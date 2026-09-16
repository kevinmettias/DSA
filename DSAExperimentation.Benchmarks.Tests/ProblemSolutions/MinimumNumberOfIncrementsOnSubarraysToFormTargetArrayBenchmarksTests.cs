using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarks
// (ARCHITECTURE 17.9): both arms are
// MinimumNumberOfIncrementsOnSubarraysToFormTargetArraySolution's, the same methods
// MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayTests proves correct, and both return
// the fewest subarray increments that build the target array. The literal layer-by-layer
// simulation and the single rising-difference pass are competing strategies for that one
// number, so arms that disagree are timing two different problems.
public sealed partial class MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarksTests
{
    // The smallest declared [Params] value: the simulation costs O(length * max height), so a
    // shorter target is the cheaper way to reach the same comparison.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().LayerByLayerSimulation(),
            BuildHarness().LayerByLayerSimulation());

    [Fact]
    public void LayerByLayerSimulation_AgreesWithRunningDiffScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningDiffScan(), harness.LayerByLayerSimulation());
    }

    [Fact]
    public void RunningDiffScan_AgreesWithLayerByLayerSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LayerByLayerSimulation(), harness.RunningDiffScan());
    }

    private static MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
