using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WildcardMatchingBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which two arms sharing a wrong answer would pass. Setup's
// pattern "*a*b" against a run of 'a's ending in one 'b' matches by construction, so the baseline's
// verdict is pinned to that decisive value.
public sealed partial class WildcardMatchingBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void IsMatchByGreedyTwoPointer_StarThenLiterals_MatchesTheText() =>
        Assert.True(BuildHarness().IsMatchByGreedyTwoPointer());

    private static WildcardMatchingBenchmarks BuildHarness()
    {
        var harness = new WildcardMatchingBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
