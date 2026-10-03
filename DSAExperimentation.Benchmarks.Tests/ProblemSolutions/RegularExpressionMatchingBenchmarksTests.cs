using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RegularExpressionMatchingBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the verdict the workload is built to have. Setup pairs
// `a^Repetitions` + "b" with `(a*)^Repetitions` + "c": the pattern's final literal must consume one
// character, and the only character no `a*` unit can take is the text's trailing "b", which is not
// "c" - so no division of the a's among the units matches, and both arms must answer false.
public sealed partial class RegularExpressionMatchingBenchmarksTests
{
    private const int SmallestRepetitions = 8;

    [Fact]
    public void IsMatchByRecursion_StarUnitsEndingInAnAbsentLiteral_ReportsNoMatch() =>
        Assert.False(BuildHarness().IsMatchByRecursion());

    [Fact]
    public void IsMatchByMemoization_StarUnitsEndingInAnAbsentLiteral_ReportsNoMatch() =>
        Assert.False(BuildHarness().IsMatchByMemoization());

    private static RegularExpressionMatchingBenchmarks BuildHarness()
    {
        var harness = new RegularExpressionMatchingBenchmarks { Repetitions = SmallestRepetitions };
        harness.Setup();

        return harness;
    }
}
