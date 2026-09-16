using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumCompatibilityScoreSumBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the best total compatibility over every student/mentor
// assignment - so a harness whose arms disagree is timing two different problems. Setup draws the
// answer sheets from the fixture's fixed seed and folds them into the prepared score matrix the arms
// are handed, so the same group size must rebuild the same matrix; neither arm mutates it.
public sealed partial class MaximumCompatibilityScoreSumBenchmarksTests
{
    private const int SmallestGroupSize = 4;

    [Fact]
    public void Setup_SameGroupSize_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecursion(), BuildHarness().BruteForceRecursion());

    [Fact]
    public void BruteForceRecursion_AgreesWithMemoizedBitmask()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecursion(), harness.MemoizedBitmask());
    }

    [Fact]
    public void MemoizedBitmask_AgreesWithBruteForceRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedBitmask(), harness.BruteForceRecursion());
    }

    private static MaximumCompatibilityScoreSumBenchmarks BuildHarness()
    {
        var harness = new MaximumCompatibilityScoreSumBenchmarks { GroupSize = SmallestGroupSize };
        harness.Setup();

        return harness;
    }
}
