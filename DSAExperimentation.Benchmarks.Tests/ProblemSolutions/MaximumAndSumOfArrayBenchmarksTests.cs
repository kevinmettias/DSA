using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumAndSumOfArrayBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the largest achievable sum after distributing the array over the
// numbered slots - so a harness whose arms disagree is timing two different problems. Setup draws
// the values from a fixed seed, so the same slot count must rebuild the same workload; neither arm
// mutates it.
public sealed partial class MaximumAndSumOfArrayBenchmarksTests
{
    private const int SmallestSlotCount = 2;

    [Fact]
    public void Setup_SameSlotCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.BruteForceRecursion());
    }

    private static MaximumAndSumOfArrayBenchmarks BuildHarness()
    {
        var harness = new MaximumAndSumOfArrayBenchmarks { SlotCount = SmallestSlotCount };
        harness.Setup();

        return harness;
    }
}
