using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumDeletionsToMakeArrayDivisibleBenchmarks (ARCHITECTURE 17.9): Setup
// makes every entry of numsDivide the same base divisor and draws every entry of nums from that
// divisor's own divisors, so nums is entirely valid candidates and the answer is decisively zero
// deletions - an expected value derived from the workload's construction rather than from either
// arm, which the generic agreement check cannot supply.
public sealed partial class MinimumDeletionsToMakeArrayDivisibleBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every element of nums divides the reduced divisor, so the smallest valid candidate is already
    // at the front of the sorted array and nothing has to be deleted.
    private const int ExpectedDeletions = 0;

    [Fact]
    public void CountValidPrecedingBruteForce_AllElementsValid_DeletesNothing() =>
        Assert.Equal(ExpectedDeletions, BuildHarness().CountValidPrecedingBruteForce());

    private static MinimumDeletionsToMakeArrayDivisibleBenchmarks BuildHarness()
    {
        var harness = new MinimumDeletionsToMakeArrayDivisibleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
