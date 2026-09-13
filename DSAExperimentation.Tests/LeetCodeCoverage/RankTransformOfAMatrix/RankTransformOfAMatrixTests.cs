using DSAExperimentation.LeetCode.RankTransformOfAMatrix;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RankTransformOfAMatrix;

// Harness only. Both ranking strategies are RankTransformOfAMatrixSolution's -
// this file just pins them to LeetCode's four published examples, plus an
// all-equal matrix (every cell shares one row or column, so one rank) and a
// single cell.
public sealed class RankTransformOfAMatrixTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            {
                [
                    [1, 2],
                    [3, 4],
                ],
                [
                    [1, 2],
                    [2, 3],
                ]
            },
            {
                [
                    [7, 7],
                    [7, 7],
                ],
                [
                    [1, 1],
                    [1, 1],
                ]
            },
            {
                [
                    [20, -21, 14],
                    [-19, 4, 19],
                    [22, -47, 24],
                    [-19, 4, 19],
                ],
                [
                    [4, 2, 3],
                    [1, 3, 4],
                    [5, 1, 6],
                    [1, 3, 4],
                ]
            },
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
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MatrixRankTransformByIterativeRelaxation_LeetCodeExamples_RanksRowsAndColumnsConsistently(
        int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, RankTransformOfAMatrixSolution.MatrixRankTransformByIterativeRelaxation(matrix));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MatrixRankTransformByDisjointSetRanking_LeetCodeExamples_RanksRowsAndColumnsConsistently(
        int[][] matrix, int[][] expected) =>
        Assert.Equal(expected, RankTransformOfAMatrixSolution.MatrixRankTransformByDisjointSetRanking(matrix));
}
