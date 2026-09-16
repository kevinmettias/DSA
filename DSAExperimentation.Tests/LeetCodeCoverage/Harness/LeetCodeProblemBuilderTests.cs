using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Harness;

// Direct tests of the harness plumbing itself, built over a throwaway problem
// rather than a real registration. The single theory in LeetCodeProblemTests
// cannot cover this ground: every registered case is expected to PASS, so nothing
// there would notice if RunCase reported success unconditionally, or ran the
// wrong strategy, or silently dropped a case.
public sealed partial class LeetCodeProblemBuilderTests
{
    private const string Slug = "harness-fixture";

    [Fact]
    public void RunCase_WhenTheStrategyProducesTheExpectedAnswer_Matches()
    {
        var outcome = Doubling().RunCase(new StrategyName("Correct"), new CaseName("two"));

        Assert.True(outcome.Matched);
    }

    // The negative control for the whole harness: if this passes as "matched",
    // every green run above is worthless.
    [Fact]
    public void RunCase_WhenTheStrategyProducesTheWrongAnswer_DoesNotMatch()
    {
        var outcome = Doubling().RunCase(new StrategyName("OffByOne"), new CaseName("two"));

        Assert.False(outcome.Matched);
        Assert.Equal("4", outcome.Expected);
        Assert.Equal("5", outcome.Actual);
    }

    [Fact]
    public void RunCase_RunsTheNamedStrategy_NotTheFirstOne()
    {
        Assert.True(Doubling().RunCase(new StrategyName("Correct"), new CaseName("three")).Matched);
        Assert.False(Doubling().RunCase(new StrategyName("OffByOne"), new CaseName("three")).Matched);
    }

    [Fact]
    public void RunCase_OnAnUnknownName_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Doubling().RunCase(new StrategyName("NoSuchStrategy"), new CaseName("two")));
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Doubling().RunCase(new StrategyName("Correct"), new CaseName("no-such-case")));
    }

    [Fact]
    public void Build_WithoutCases_Throws()
    {
        var builder = LeetCodeProblem.For<int, int>(Slug)
            .Strategy("Correct", value => value * 2)
            .MatchingAnswersWith(LeetCodeAnswers.IsExactlyEqual);

        Assert.Throws<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_WithoutAStatedAnswerComparison_Throws()
    {
        var builder = LeetCodeProblem.For<int, int>(Slug)
            .Strategy("Correct", value => value * 2)
            .Case("two", 2, 4);

        Assert.Throws<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void Build_WithoutStrategies_Throws()
    {
        var builder = LeetCodeProblem.For<int, int>(Slug)
            .Case("two", 2, 4)
            .MatchingAnswersWith(LeetCodeAnswers.IsExactlyEqual);

        Assert.Throws<InvalidOperationException>(builder.Build);
    }

    [Fact]
    public void WorkloadArms_WhenAWorkloadNamesOneStrategy_ExcludesTheOthers()
    {
        var problem = Doubling();

        Assert.Equal(
            [new LeetCodeArm(Slug, "Correct", "big")],
            problem.WorkloadArms);
    }

    [Fact]
    public void BindWorkload_ReturnsTheMeasuredCallOverThePreparedInput() => Assert.Equal(
        2_000, Doubling().BindWorkload(new StrategyName("Correct"), new WorkloadName("big")).Run());

    private static LeetCodeProblem Doubling()
        => LeetCodeProblem.For<int, int>(Slug)
            .Strategy("Correct", value => value * 2)
            .Strategy("OffByOne", value => (value * 2) + 1)
            .MatchingAnswersWith(LeetCodeAnswers.IsExactlyEqual)
            .Case("two", 2, 4)
            .Case("three", 3, 6)
            .Workload("big", 1_000, "Correct")
            .Build();
}
