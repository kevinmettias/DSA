using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumLengthOfRepeatedSubarrayBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumLengthOfRepeatedSubarraySolution's competing strategies for one question - the cubic
// starting-pair scan against the memoized suffix-pair DP - so a harness whose arms disagree is
// timing two different problems. Both answer with a single length, compared directly.
//
// Setup leaves both arrays at their default element value, which is the identical all-one-value
// shape the benchmark's own comment describes. Two identical length-n arrays share their entire
// run, so the longest common subarray is the whole of each and the answer is exactly the Length
// Setup drew - an oracle read off the fixture's layout, not off either arm's return.
public sealed partial class MaximumLengthOfRepeatedSubarrayBenchmarksTests
{
    private const int SmallestLength = 60;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameIdenticalArrays()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(SmallestLength, first.BruteForce());
        Assert.Equal(SmallestLength, second.MemoizedSuffixPairDp());
    }

    [Fact]
    public void BruteForce_IdenticalArrays_AgreesWithMemoizedSuffixPairDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedSuffixPairDp(), harness.BruteForce());
    }

    [Fact]
    public void MemoizedSuffixPairDp_IdenticalArrays_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.MemoizedSuffixPairDp());
    }

    private static MaximumLengthOfRepeatedSubarrayBenchmarks BuildHarness()
    {
        var harness = new MaximumLengthOfRepeatedSubarrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
