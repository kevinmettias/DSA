using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WildcardMatchingBenchmarks (ARCHITECTURE 17.9): both arms are competing
// matchers for one question on one text/pattern pair, so a harness whose arms disagree is timing
// two different problems. The pattern is the class comment's "*a*b" against a run of 'a's ending in
// one 'b', which matches by construction - that decisive value keeps the agreement from standing on
// two arms that merely happen to share an answer.
public sealed partial class WildcardMatchingBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().IsMatchByGreedyTwoPointer()),
            AnswerText.Of(BuildHarness().IsMatchByGreedyTwoPointer()));

    [Fact]
    public void IsMatchByGreedyTwoPointer_AgreesWithMemoizedDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsMatchByGreedyTwoPointer(), harness.IsMatchByMemoizedDp());
    }

    [Fact]
    public void IsMatchByMemoizedDp_AgreesWithGreedyTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsMatchByMemoizedDp(), harness.IsMatchByGreedyTwoPointer());
    }

    [Fact]
    public void IsMatchByGreedyTwoPointer_StarThenLiterals_MatchesTheText() =>
        Assert.True(BuildHarness().IsMatchByGreedyTwoPointer());

    [Fact]
    public void IsMatchByMemoizedDp_StarThenLiterals_MatchesTheText() =>
        Assert.True(BuildHarness().IsMatchByMemoizedDp());

    private static WildcardMatchingBenchmarks BuildHarness()
    {
        var harness = new WildcardMatchingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
