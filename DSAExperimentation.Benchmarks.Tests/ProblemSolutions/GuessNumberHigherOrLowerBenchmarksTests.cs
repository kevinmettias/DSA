using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GuessNumberHigherOrLowerBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on the smallest workload, so this asserts only the answer the
// fixture fixes independently of either arm: [GlobalSetup] places the pick at a fixed fraction of
// NumberCount, which is what makes the linear scan pay close to its full O(n) worst case, and the
// number the baseline must report is that same value.
public sealed partial class GuessNumberHigherOrLowerBenchmarksTests
{
    private const int SmallestNumberCount = 1_000;

    // The fraction [GlobalSetup] places the pick at, restated so the expected number can be derived
    // from the fixture rather than read back out of an arm.
    private const double PickFraction = 0.7;

    [Fact]
    public void LinearScan_PickNearTheTopOfTheRange_ReportsThePickedNumber() =>
        Assert.Equal(ExpectedPick, BuildHarness().LinearScan());

    private static int ExpectedPick => (int)(SmallestNumberCount * PickFraction);

    private static GuessNumberHigherOrLowerBenchmarks BuildHarness()
    {
        var harness = new GuessNumberHigherOrLowerBenchmarks { NumberCount = SmallestNumberCount };
        harness.Setup();

        return harness;
    }
}
