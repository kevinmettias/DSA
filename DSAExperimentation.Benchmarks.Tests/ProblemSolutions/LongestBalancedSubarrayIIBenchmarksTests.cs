using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestBalancedSubarrayIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) window scan against the O(n log^2 n)
// prefix-balance segment tree - so a harness whose arms disagree is timing two different problems.
// Both arms return the longest balanced length, a scalar compared directly. Setup alternates 2i
// and 2i + 1, so every value is distinct and each parity contributes Length / 2 distinct counts:
// the whole array is balanced and the answer is exactly its length, which is the decisive value
// both arms must reach and the same Length must rebuild.
public sealed partial class LongestBalancedSubarrayIIBenchmarksTests
{
    private const int SmallestLength = 100;

    // Setup's alternating values are all distinct, so half the array counts as distinct evens and
    // half as distinct odds - the full length is the longest balanced subarray.
    private const int ExpectedLongestBalancedLength = SmallestLength;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedLongestBalancedLength, BuildHarness().PrefixBalanceSegmentTree());
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());
    }

    [Fact]
    public void BruteForce_SmallestLength_AgreesWithPrefixBalanceSegmentTree()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestBalancedLength, harness.BruteForce());
        Assert.Equal(harness.PrefixBalanceSegmentTree(), harness.BruteForce());
    }

    [Fact]
    public void PrefixBalanceSegmentTree_SmallestLength_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLongestBalancedLength, harness.PrefixBalanceSegmentTree());
        Assert.Equal(harness.BruteForce(), harness.PrefixBalanceSegmentTree());
    }

    private static LongestBalancedSubarrayIIBenchmarks BuildHarness()
    {
        var harness = new LongestBalancedSubarrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
