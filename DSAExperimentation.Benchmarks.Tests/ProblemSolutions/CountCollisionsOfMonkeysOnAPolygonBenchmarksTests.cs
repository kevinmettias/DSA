using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountCollisionsOfMonkeysOnAPolygonBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing at the smallest [Params] count, so this pins
// the baseline to the closed form the class comment states, (2^n - 2) mod 1e9+7, computed here
// independently of both arms. The class carries no [GlobalSetup] - the monkey count is the whole
// workload.
public sealed partial class CountCollisionsOfMonkeysOnAPolygonBenchmarksTests
{
    private const int SmallestMonkeyCount = 1_000;
    private const int WaysForOneThousandMonkeys = 688423208;

    [Fact]
    public void RepeatedMultiplication_OneThousandMonkeys_MatchesTheClosedForm() =>
        Assert.Equal(WaysForOneThousandMonkeys, BuildHarness().RepeatedMultiplication());

    private static CountCollisionsOfMonkeysOnAPolygonBenchmarks BuildHarness() =>
        new() { MonkeyCount = SmallestMonkeyCount };
}
