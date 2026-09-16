using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestBinarySubsequenceLessThanOrEqualToKBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - exhaustive subset enumeration
// against the O(n) right-to-left greedy scan - so a harness whose arms disagree is timing two
// different problems. Both arms return the subsequence length, a scalar compared directly. Setup
// draws the bits from a fixed seed, so the same Length must rebuild the same string; that length
// is not derivable from the documented shape, so the arms are compared against each other alone.
public sealed partial class LongestBinarySubsequenceLessThanOrEqualToKBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().GreedyRightToLeftScan(),
            BuildHarness().GreedyRightToLeftScan());

    [Fact]
    public void BruteForceSubsetEnumeration_SmallestLength_AgreesWithGreedyRightToLeftScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedyRightToLeftScan(), harness.BruteForceSubsetEnumeration());
    }

    [Fact]
    public void GreedyRightToLeftScan_SmallestLength_AgreesWithBruteForceSubsetEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceSubsetEnumeration(), harness.GreedyRightToLeftScan());
    }

    private static LongestBinarySubsequenceLessThanOrEqualToKBenchmarks BuildHarness()
    {
        var harness = new LongestBinarySubsequenceLessThanOrEqualToKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
