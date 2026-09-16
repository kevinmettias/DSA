using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NQueensBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies for the same
// question - the plain recursive DFS over a row's columns against the backtracking engine - so a harness whose arms
// disagree is timing two different problems. Both arms build the answer from the Size alone, hold no state between
// calls and share nothing, so one harness instance is safe to call twice in either order, and both render the same
// LeetCode answer shape: the list of boards, each board a list of row strings, which the problem pins row by row.
public sealed partial class NQueensBenchmarksTests
{
    private const int EightQueens = 8;

    [Fact]
    public void RecursiveDfs_EightQueens_AgreesWithBacktrackEngine()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.BacktrackEngine()),
            AnswerText.Of(harness.RecursiveDfs()));
    }

    [Fact]
    public void BacktrackEngine_EightQueens_AgreesWithRecursiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.RecursiveDfs()),
            AnswerText.Of(harness.BacktrackEngine()));
    }

    private static NQueensBenchmarks BuildHarness() => new() { Size = EightQueens };
}
