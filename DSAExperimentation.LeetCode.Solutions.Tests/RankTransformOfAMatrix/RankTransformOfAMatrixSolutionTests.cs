using DSAExperimentation.LeetCode.Solutions.RankTransformOfAMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RankTransformOfAMatrix;

// Harness only. Both ranking strategies are RankTransformOfAMatrixSolution's -
// this file just pins them to a single
// cell and two hand-worked tie cases. In the first, the three 5s share a rank of
// 4 although (0, 0)'s own row and column hold nothing above rank 1: (0, 2) is
// raised by the 3 below it and (2, 0) by the 4 beside it, and the tie rule carries
// both up to (0, 0). In the second, the two 3s share no row or column, so they are
// ranked apart - 3 after the 1 and 2 in their row, 2 after the 0s in theirs.
public sealed partial class RankTransformOfAMatrixSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            {
                [
                    [7, 3, 6],
                    [1, 4, 5],
                    [9, 8, 2],
                ],
                [
                    [5, 1, 4],
                    [1, 2, 3],
                    [6, 3, 1],
                ]
            },
            {
                [[42]],
                [[1]]
            },
            {
                [
                    [5, 1, 5],
                    [1, 2, 3],
                    [5, 4, 9],
                ],
                [
                    [4, 1, 4],
                    [1, 2, 3],
                    [4, 3, 5],
                ]
            },
            {
                [
                    [1, 2, 3],
                    [3, 0, 0],
                ],
                [
                    [1, 2, 3],
                    [2, 1, 1],
                ]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MatrixRankTransformByIterativeRelaxation_Cases_RanksRowsAndColumnsConsistently(
        int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, RankTransformOfAMatrixSolution.MatrixRankTransformByIterativeRelaxation(matrix));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MatrixRankTransformByDisjointSetRanking_Cases_RanksRowsAndColumnsConsistently(
        int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, RankTransformOfAMatrixSolution.MatrixRankTransformByDisjointSetRanking(matrix));
}
