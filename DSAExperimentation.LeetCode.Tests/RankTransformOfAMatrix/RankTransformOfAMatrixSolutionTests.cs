using DSAExperimentation.LeetCode.RankTransformOfAMatrix;

namespace DSAExperimentation.LeetCode.Tests.RankTransformOfAMatrix;

// Harness only. Both ranking strategies are RankTransformOfAMatrixSolution's -
// this file just pins them to LeetCode's four published examples, plus a single
// cell and two hand-worked tie cases. In the first, the three 5s share a rank of
// 4 although (0, 0)'s own row and column hold nothing above rank 1: (0, 2) is
// raised by the 3 below it and (2, 0) by the 4 beside it, and the tie rule carries
// both up to (0, 0). In the second, the two 3s share no row or column, so they are
// ranked apart - 3 after the 1 and 2 in their row, 2 after the 0s in theirs.
public sealed partial class RankTransformOfAMatrixSolutionTests
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
