using DSAExperimentation.LeetCode.NumberOfValidMoveCombinationsOnChessboard;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfValidMoveCombinationsOnChessboard;

// Harness only. Both strategies are
// NumberOfValidMoveCombinationsOnChessboardSolution's - this file pins them to
// LeetCode's published examples in LeetCode's own (pieces, positions) input shape:
// each piece alone in a corner, LeetCode's own off-corner bishop, and the two-piece
// cases where the simultaneous-movement collision rule is what removes
// combinations from the product.
public sealed partial class NumberOfValidMoveCombinationsOnChessboardTests
{
    public static TheoryData<string[], int[][], int> Examples =>
        new()
        {
            { ["rook"], [[1, 1]], 15 },
            { ["queen"], [[1, 1]], 22 },
            { ["bishop"], [[4, 3]], 12 },
            { ["bishop"], [[1, 1]], 8 },
            { ["rook", "rook"], [[1, 1], [8, 8]], 223 },
            { ["rook", "bishop"], [[1, 1], [8, 8]], 119 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByPrunedBacktracking_LeetCodeExamples_ReturnsCollisionFreeCombinationCount(
        string[] pieces, int[][] positions, int expected)
    {
        var actual = NumberOfValidMoveCombinationsOnChessboardSolution.CountCombinationsByPrunedBacktracking(
            pieces, positions);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCombinationsByCartesianProduct_LeetCodeExamples_ReturnsCollisionFreeCombinationCount(
        string[] pieces, int[][] positions, int expected)
    {
        var actual = NumberOfValidMoveCombinationsOnChessboardSolution.CountCombinationsByCartesianProduct(
            pieces, positions);

        Assert.Equal(expected, actual);
    }
}
