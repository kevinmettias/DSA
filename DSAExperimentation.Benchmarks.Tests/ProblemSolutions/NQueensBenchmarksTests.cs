using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NQueensBenchmarks (ARCHITECTURE 17.9): its arms are competing strategies for the same
// question - the plain recursive DFS over a row's columns, the backtracking engine, and Algorithm X over the exact
// cover - so a harness whose arms disagree is timing different problems. Every arm builds the answer from the Size
// alone, holds no state between calls and shares nothing, so one harness instance is safe to call twice in any order,
// and each renders the same LeetCode answer shape: the list of boards, each board a list of row strings, which the
// problem pins row by row. The two row-by-row arms list the boards in one order; the exact cover's order is its own,
// which LC 51 allows, so it is compared as a set of boards.
public sealed partial class NQueensBenchmarksTests
{
    private const int EightQueens = 8;

    [Fact]
    public void RecursiveDfs_EightQueens_AgreesWithBacktrackEngine()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.Of(harness.BacktrackEngine()),
            AnswerGraphText.Of(harness.RecursiveDfs()));
    }

    [Fact]
    public void ExactCover_EightQueens_AgreesWithRecursiveDfsAsASet()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerGraphText.OfUnordered(harness.RecursiveDfs()),
            AnswerGraphText.OfUnordered(harness.ExactCover()));
    }

    private static NQueensBenchmarks BuildHarness() => new() { Size = EightQueens };
}
