using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarraySumEqualsKBenchmarks (ARCHITECTURE 17.9): the target is
// deliberately unreachable, which makes every generated workload answer 0: that is what forces
// both arms through their full worst-case scan, and it also means the generic arm check's
// agreement alone would be satisfied by two arms that both failed to count anything. The derived
// expectation below, asserted of the baseline, is what carries the weight.
public sealed partial class SubarraySumEqualsKBenchmarksTests
{
    private const int SmallestLength = 200;

    // The target lies far outside the range any running sum can reach: values are bounded to
    // magnitude 10 and there are only SmallestLength of them, so the largest attainable
    // magnitude is a few thousand against a target of a million.
    private const int ExpectedSubarrayCount = 0;

    [Fact]
    public void BruteForce_UnreachableTarget_CountsNoSubarray() =>
        Assert.Equal(ExpectedSubarrayCount, BuildHarness().BruteForce());

    private static SubarraySumEqualsKBenchmarks BuildHarness()
    {
        var harness = new SubarraySumEqualsKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
