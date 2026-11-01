using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestBalancedSubarrayIIBenchmarks (ARCHITECTURE 17.9): Setup alternates
// 2(i + 1) and 2(i + 1) + 1, so every value is distinct and each parity contributes Length / 2
// distinct counts: the whole array is balanced and the answer is exactly its length - an expected
// value derived from the workload's construction rather than from either arm, which the generic
// agreement check cannot supply. The baseline returns the longest balanced length, a scalar compared
// directly.
public sealed partial class LongestBalancedSubarrayIIBenchmarksTests
{
    private const int SmallestLength = 100;

    // Setup's alternating values are all distinct, so half the array counts as distinct evens and
    // half as distinct odds - the full length is the longest balanced subarray.
    private const int ExpectedLongestBalancedLength = SmallestLength;

    [Fact]
    public void BruteForce_SmallestLength_SpansTheWholeArray() =>
        Assert.Equal(ExpectedLongestBalancedLength, BuildHarness().BruteForce());

    private static LongestBalancedSubarrayIIBenchmarks BuildHarness()
    {
        var harness = new LongestBalancedSubarrayIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
