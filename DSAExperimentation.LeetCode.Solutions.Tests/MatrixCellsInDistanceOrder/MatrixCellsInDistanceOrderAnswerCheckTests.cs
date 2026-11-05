using DSAExperimentation.LeetCode.Solutions.MatrixCellsInDistanceOrder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MatrixCellsInDistanceOrder;

public sealed partial class MatrixCellsInDistanceOrderAnswerCheckTests
{
    public static TheoryData<int, int, int, int, int[][], int[][], bool> Answers =>
        new()
        {
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1], [1, 0]], [[0, 1], [0, 0], [1, 1], [1, 0]], true },
            { 2, 2, 0, 1, [[0, 1], [1, 1], [0, 0], [1, 0]], [[0, 1], [0, 0], [1, 1], [1, 0]], true },
            { 2, 3, 1, 2, [[1, 2], [1, 1], [0, 2], [1, 0], [0, 1], [0, 0]], [[1, 2], [0, 2], [1, 1], [0, 1], [1, 0], [0, 0]], true },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 0], [1, 1]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1], [1, 1]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1], [2, 0]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1], [1, 0, 0]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [[0, 1], [0, 0], [1, 1], [1]], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
            { 2, 2, 0, 1, [], [[0, 1], [0, 0], [1, 1], [1, 0]], false },
        };

    [Theory]
    [MemberData(nameof(Answers))]
    public void IsAccepted_Answer_HoldsForEveryCellOnceWithTheDistanceNeverFalling(int rows, int cols, int rCenter, int cCenter, int[][] answer, int[][] recorded, bool expected)
        => Assert.Equal(expected, MatrixCellsInDistanceOrderAnswerCheck.IsAccepted(rows, cols, rCenter, cCenter, answer, recorded));
}
