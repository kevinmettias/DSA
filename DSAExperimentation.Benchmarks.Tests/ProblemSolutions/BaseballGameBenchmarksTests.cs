using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BaseballGameBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each other,
// and this asserts the total is positive: Setup's script emits no "C" and every base score is at least one, so a
// zero or negative total would mean an arm never recorded a score.
public sealed partial class BaseballGameBenchmarksTests
{
    // The smaller of Setup's [Params(200, 1_000)] script lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void ManualArrayCursor_TwoHundredOperationScript_ReturnsAPositiveTotal() =>
        Assert.True(BuildHarness().ManualArrayCursor() > 0);

    private static BaseballGameBenchmarks BuildHarness()
    {
        var harness = new BaseballGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
