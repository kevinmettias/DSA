using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumOperationsToMakeArrayEqualToTargetBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the fewest operations that turn nums into
// target - so a harness whose arms disagree is timing two different problems. Both arms read the one
// (nums, target) pair [GlobalSetup] built, so the comparison also pins that the simulation arm's
// exhaustive walk and the difference-scan arm were handed the same two arrays. Setup draws them from
// one fixed seed, so the same Length must rebuild the same pair.
public sealed partial class MinimumOperationsToMakeArrayEqualToTargetBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DifferenceScan(), BuildHarness().DifferenceScan());

    [Fact]
    public void BruteForceSimulation_SameArrayPair_AgreesWithDifferenceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DifferenceScan(), harness.BruteForceSimulation());
    }

    [Fact]
    public void DifferenceScan_SameArrayPair_AgreesWithBruteForceSimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSimulation(), harness.DifferenceScan());
    }

    private static MinimumOperationsToMakeArrayEqualToTargetBenchmarks BuildHarness()
    {
        var harness = new MinimumOperationsToMakeArrayEqualToTargetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
