using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDeletionsToMakeArrayDivisibleBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a candidate scan that recounts the whole array per
// candidate against one merge sort followed by a scan - so a harness whose arms disagree is timing two
// different arrays. Setup makes every entry of numsDivide the same base divisor and draws every entry
// of nums from that divisor's own divisors, so nums is entirely valid candidates and the answer is
// decisively zero deletions: agreement here also witnesses that neither arm invented work the other
// did not do. The draw is seeded, so the same Length must rebuild the same nums.
public sealed partial class MinimumDeletionsToMakeArrayDivisibleBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every element of nums divides the reduced divisor, so the smallest valid candidate is already
    // at the front of the sorted array and nothing has to be deleted.
    private const int ExpectedDeletions = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameArrays() =>
        Assert.Equal(BuildHarness().CountValidPrecedingBruteForce(), BuildHarness().CountValidPrecedingBruteForce());

    [Fact]
    public void CountValidPrecedingBruteForce_AllElementsValid_DeletesNothingAndAgreesWithMergeSortAndScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDeletions, harness.CountValidPrecedingBruteForce());
        Assert.Equal(harness.MergeSortAndScan(), harness.CountValidPrecedingBruteForce());
    }

    [Fact]
    public void MergeSortAndScan_AllElementsValid_DeletesNothingAndAgreesWithCountValidPrecedingBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDeletions, harness.MergeSortAndScan());
        Assert.Equal(harness.CountValidPrecedingBruteForce(), harness.MergeSortAndScan());
    }

    private static MinimumDeletionsToMakeArrayDivisibleBenchmarks BuildHarness()
    {
        var harness = new MinimumDeletionsToMakeArrayDivisibleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
