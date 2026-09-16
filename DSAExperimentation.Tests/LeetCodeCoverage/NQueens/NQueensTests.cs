using DSAExperimentation.LeetCode.NQueens;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NQueens;

// Harness only. Both strategies are NQueensSolution's; this file just pins them to
// LeetCode's published examples - the solution count for boardSize, plus one board
// LeetCode itself publishes as a valid placement.
public sealed class NQueensTests
{
    public static TheoryData<int, int, string[]> Examples =>
        new()
        {
            { 1, 1, ["Q"] },
            { 4, 2, [".Q..", "...Q", "Q...", "..Q."] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveByRecursiveDfs_LeetCodeExamples_ReturnsExpectedSolutions(
        int boardSize, int expectedCount, string[] sampleBoard)
    {
        var solutions = NQueensSolution.SolveByRecursiveDfs(boardSize);

        Assert.Equal(expectedCount, solutions.Count);
        Assert.Contains(solutions, board => board.SequenceEqual(sampleBoard));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SolveByBacktrackEngine_LeetCodeExamples_ReturnsExpectedSolutions(
        int boardSize, int expectedCount, string[] sampleBoard)
    {
        var solutions = NQueensSolution.SolveByBacktrackEngine(boardSize);

        Assert.Equal(expectedCount, solutions.Count);
        Assert.Contains(solutions, board => board.SequenceEqual(sampleBoard));
    }
}
