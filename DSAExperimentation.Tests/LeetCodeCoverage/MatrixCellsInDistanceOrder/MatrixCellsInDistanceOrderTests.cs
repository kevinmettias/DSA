using DSAExperimentation.LeetCode.MatrixCellsInDistanceOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatrixCellsInDistanceOrder;

// Harness only. Both strategies are MatrixCellsInDistanceOrderSolution's - this file
// pins them to LeetCode's published examples. LC 1030 accepts any ordering among
// equidistant cells, so an example states the distance sequence the answer must
// produce rather than one exact permutation; together with "every cell exactly once"
// that pins the answer as tightly as LeetCode itself does, and exactly on the
// tie-free single-row case.
public sealed class MatrixCellsInDistanceOrderTests
{
    // rows, cols, rCenter, cCenter, and the Manhattan distances the returned cells
    // must have, in order.
    public static TheoryData<int, int, int, int, int[]> Examples =>
        new()
        {
            { 1, 1, 0, 0, new[] { 0 } },
            { 1, 2, 0, 0, new[] { 0, 1 } },
            { 2, 2, 0, 1, new[] { 0, 1, 1, 2 } },
            { 2, 3, 1, 2, new[] { 0, 1, 1, 2, 2, 3 } },
            { 3, 3, 1, 1, new[] { 0, 1, 1, 1, 1, 2, 2, 2, 2 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllCellsDistOrderByManhattanFormula_LeetCodeExamples_ReturnsEveryCellInDistanceOrder(
        int rows, int cols, int rCenter, int cCenter, int[] expectedDistances) =>
        AssertDistanceOrder(
            MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByManhattanFormula(rows, cols, rCenter, cCenter),
            rows, cols, rCenter, cCenter, expectedDistances);

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllCellsDistOrderByGridBfs_LeetCodeExamples_ReturnsEveryCellInDistanceOrder(
        int rows, int cols, int rCenter, int cCenter, int[] expectedDistances) =>
        AssertDistanceOrder(
            MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByGridBfs(rows, cols, rCenter, cCenter),
            rows, cols, rCenter, cCenter, expectedDistances);

    private static void AssertDistanceOrder(
        int[][] result, int rows, int cols, int rCenter, int cCenter, int[] expectedDistances)
    {
        Assert.Equal(rows * cols, result.Length);
        Assert.Equal(AllCells(rows, cols), result.OrderBy(cell => cell[0]).ThenBy(cell => cell[1]));
        Assert.Equal(
            expectedDistances,
            result.Select(cell => Math.Abs(cell[0] - rCenter) + Math.Abs(cell[1] - cCenter)));
    }

    private static int[][] AllCells(int rows, int cols) =>
        Enumerable.Range(0, rows)
            .SelectMany(row => Enumerable.Range(0, cols).Select(col => new[] { row, col }))
            .ToArray();
}
