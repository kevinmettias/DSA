using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfEvenNumbersAfterQueriesBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfEvenNumbersAfterQueriesSolution's - the O(n) rescan per query against the running even-sum
// invariant - so a harness whose arms disagree is timing two different questions. Each arm returns the
// per-query answers, and the problem pins that sequence to the order its queries arrive in, so the
// outer order is part of the answer and the comparison is order-sensitive.
public sealed partial class SumOfEvenNumbersAfterQueriesBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RescanAfterEveryQuery()),
            AnswerText.Of(BuildHarness().RescanAfterEveryQuery()));

    [Fact]
    public void RescanAfterEveryQuery_SeededValuesAndQueries_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RunningEvenSum()),
            AnswerText.Of(harness.RescanAfterEveryQuery()));
    }

    [Fact]
    public void RunningEvenSum_SeededValuesAndQueries_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RescanAfterEveryQuery()),
            AnswerText.Of(harness.RunningEvenSum()));
    }

    private static SumOfEvenNumbersAfterQueriesBenchmarks BuildHarness()
    {
        var harness = new SumOfEvenNumbersAfterQueriesBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
