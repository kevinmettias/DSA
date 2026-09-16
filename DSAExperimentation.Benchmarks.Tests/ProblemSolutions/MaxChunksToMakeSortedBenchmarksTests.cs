using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxChunksToMakeSortedBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - testing every candidate boundary by rescanning the
// range against a single running-max prefix scan - so a harness whose arms disagree is timing two
// different problems. Both arms return the chunk count LC 769 asks for over a permutation of
// 0..Length-1. Setup shuffles that permutation from one fixed seed, so the same Length must rebuild
// the same array and the same count.
public sealed partial class MaxChunksToMakeSortedBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
        Assert.Equal(BuildHarness().RunningMaxScan(), BuildHarness().RunningMaxScan());
    }

    [Fact]
    public void BruteForce_SeededPermutation_AgreesWithRunningMaxScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningMaxScan(), harness.BruteForce());
    }

    [Fact]
    public void RunningMaxScan_SeededPermutation_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.RunningMaxScan());
    }

    private static MaxChunksToMakeSortedBenchmarks BuildHarness()
    {
        var harness = new MaxChunksToMakeSortedBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
