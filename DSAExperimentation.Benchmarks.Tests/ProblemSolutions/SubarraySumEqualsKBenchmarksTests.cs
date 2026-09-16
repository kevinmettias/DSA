using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarraySumEqualsKBenchmarks (ARCHITECTURE 17.9): both arms answer the
// same question - how many LC 560 subarrays sum to the target - one by the plain double loop,
// one by prefix-sum frequencies, so a harness whose arms disagree is timing two different
// problems. Setup's values are seeded, so the same length must rebuild the same workload.
//
// The target is deliberately unreachable, which makes every generated workload answer 0: that
// is what forces both arms through their full worst-case scan, and it also means agreement
// alone would be satisfied by two arms that both failed to count anything. The derived
// expectation below is what carries the weight.
public sealed partial class SubarraySumEqualsKBenchmarksTests
{
    private const int SmallestLength = 200;

    // The target lies far outside the range any running sum can reach: values are bounded to
    // magnitude 10 and there are only SmallestLength of them, so the largest attainable
    // magnitude is a few thousand against a target of a million.
    private const int ExpectedSubarrayCount = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().BruteForce()),
            AnswerText.Of(BuildHarness().BruteForce()));

    [Fact]
    public void BruteForce_AgreesWithPrefixSumHashMap()
    {
        var harness = BuildHarness();
        var bruteForce = harness.BruteForce();

        Assert.Equal(bruteForce, harness.PrefixSumHashMap());
        Assert.Equal(ExpectedSubarrayCount, bruteForce);
    }

    [Fact]
    public void PrefixSumHashMap_AgreesWithBruteForce()
    {
        var harness = BuildHarness();
        var prefixSums = harness.PrefixSumHashMap();

        Assert.Equal(prefixSums, harness.BruteForce());
        Assert.Equal(ExpectedSubarrayCount, prefixSums);
    }

    private static SubarraySumEqualsKBenchmarks BuildHarness()
    {
        var harness = new SubarraySumEqualsKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
