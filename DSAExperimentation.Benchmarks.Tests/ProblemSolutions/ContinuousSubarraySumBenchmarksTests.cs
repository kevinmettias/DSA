using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContinuousSubarraySumBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the start/end pair scan against the hashmap of
// first-seen prefix remainders - so a harness whose arms disagree is timing two different problems.
// Both arms return a bool, so they are compared directly. Setup draws from one fixed seed, so the
// same Length must rebuild the same array, and its documented shape is that the divisor exceeds any
// possible total sum of the generated values: no subarray can be a multiple of it, which is what
// forces both strategies through their full worst-case scan.
public sealed partial class ContinuousSubarraySumBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameUnqualifyingWorkload()
    {
        Assert.Equal(
            BuildHarness().HasSubarraySumMultipleOfKByBruteForce(),
            BuildHarness().HasSubarraySumMultipleOfKByBruteForce());

        Assert.False(BuildHarness().HasSubarraySumMultipleOfKByHashMapPrefixRemainder());
    }

    [Fact]
    public void HasSubarraySumMultipleOfKByBruteForce_ValuesBelowTheDivisor_AgreesWithHashMapPrefixRemainder()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasSubarraySumMultipleOfKByHashMapPrefixRemainder(),
            harness.HasSubarraySumMultipleOfKByBruteForce());
    }

    [Fact]
    public void HasSubarraySumMultipleOfKByHashMapPrefixRemainder_ValuesBelowTheDivisor_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.HasSubarraySumMultipleOfKByBruteForce(),
            harness.HasSubarraySumMultipleOfKByHashMapPrefixRemainder());
    }

    private static ContinuousSubarraySumBenchmarks BuildHarness()
    {
        var harness = new ContinuousSubarraySumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
