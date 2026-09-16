using DSAExperimentation.LeetCode.NQueensII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NQueensII;

// Harness only. Both strategies live in NQueensIISolution: a hand-rolled
// array-recursion baseline and this repo's own Backtrack.Search composition,
// asserted against the same board sizes including the boards with no
// solution at all.
public sealed partial class NQueensIITests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 1 },
            { 2, 0 },
            { 3, 0 },
            { 4, 2 },
            { 5, 10 },
            { 8, 92 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalNQueensByArrayRecursion_LeetCodeExamples_ReturnsSolutionCount(int boardSize, int expected) =>
        Assert.Equal(expected, NQueensIISolution.TotalNQueensByArrayRecursion(boardSize));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalNQueensByBacktrackSearch_LeetCodeExamples_ReturnsSolutionCount(int boardSize, int expected) =>
        Assert.Equal(expected, NQueensIISolution.TotalNQueensByBacktrackSearch(boardSize));
}
