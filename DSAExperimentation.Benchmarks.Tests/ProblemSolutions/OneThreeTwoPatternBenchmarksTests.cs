using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for OneThreeTwoPatternBenchmarks (ARCHITECTURE 17.9): the baseline's verdict against
// the one the workload's shape fixes, which the generic check cannot supply - it holds the monotonic
// stack to the baseline. Setup makes the array strictly increasing (0..Length-1), which the class
// comment already pins as containing no 132 pattern at all, so the answer is a known false.
public sealed partial class OneThreeTwoPatternBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void HasPatternByBruteForce_IncreasingWorkload_ReportsNoPattern() =>
        Assert.False(BuildHarness().HasPatternByBruteForce());

    private static OneThreeTwoPatternBenchmarks BuildHarness()
    {
        var harness = new OneThreeTwoPatternBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
