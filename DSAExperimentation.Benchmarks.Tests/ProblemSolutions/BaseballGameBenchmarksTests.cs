using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BaseballGameBenchmarks (ARCHITECTURE 17.9): its two arms are BaseballGameSolution's
// competing strategies for the same question - a manual array cursor against a stack replay - so a harness whose
// arms disagree has scored two different operation scripts. Both return the single total, so they are compared
// directly, and the total is asserted positive as well: Setup's script emits no "C" and every base score is at
// least one, so a zero or negative total would mean an arm never recorded a score. Setup builds the script from
// the length alone with no random draw, so the same Length must rebuild the same operations.
public sealed partial class BaseballGameBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] script lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ManualArrayCursor(), BuildHarness().ManualArrayCursor());

    [Fact]
    public void ManualArrayCursor_TwoHundredOperationScript_AgreesWithStackReplay()
    {
        var harness = BuildHarness();

        Assert.True(harness.ManualArrayCursor() > 0);
        Assert.Equal(harness.StackReplay(), harness.ManualArrayCursor());
    }

    [Fact]
    public void StackReplay_TwoHundredOperationScript_AgreesWithManualArrayCursor()
    {
        var harness = BuildHarness();

        Assert.True(harness.StackReplay() > 0);
        Assert.Equal(harness.ManualArrayCursor(), harness.StackReplay());
    }

    private static BaseballGameBenchmarks BuildHarness()
    {
        var harness = new BaseballGameBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
