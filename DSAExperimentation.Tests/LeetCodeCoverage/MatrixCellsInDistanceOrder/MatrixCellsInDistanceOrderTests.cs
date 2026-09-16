using DSAExperimentation.LeetCode.MatrixCellsInDistanceOrder;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatrixCellsInDistanceOrder;

// Harness only. Both strategies are MatrixCellsInDistanceOrderSolution's - this file
// pins them to LeetCode's published examples. LC 1030 accepts any ordering among
// equidistant cells, so an example states the distance sequence the answer must
// produce rather than one exact permutation; together with "every cell exactly once"
// that pins the answer as tightly as LeetCode itself does, and exactly on the
// tie-free single-row case.
public sealed partial class MatrixCellsInDistanceOrderTests
{
    public static TheoryData<MatrixExample> Examples =>
        new()
        {
            { new MatrixExample(Rows: 1, Cols: 1, RCenter: 0, CCenter: 0, ExpectedDistances: [0]) },
            { new MatrixExample(Rows: 1, Cols: 2, RCenter: 0, CCenter: 0, ExpectedDistances: [0, 1]) },
            { new MatrixExample(Rows: 2, Cols: 2, RCenter: 0, CCenter: 1, ExpectedDistances: [0, 1, 1, 2]) },
            { new MatrixExample(Rows: 2, Cols: 3, RCenter: 1, CCenter: 2, ExpectedDistances: [0, 1, 1, 2, 2, 3]) },
            { new MatrixExample(Rows: 3, Cols: 3, RCenter: 1, CCenter: 1, ExpectedDistances: [0, 1, 1, 1, 1, 2, 2, 2, 2]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllCellsDistOrderByManhattanFormula_LeetCodeExamples_ReturnsEveryCellInDistanceOrder(MatrixExample example)
    {
        var result = MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByManhattanFormula(
            example.Rows, example.Cols, example.RCenter, example.CCenter);

        AssertDistanceOrder(result, example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllCellsDistOrderByGridBfs_LeetCodeExamples_ReturnsEveryCellInDistanceOrder(MatrixExample example)
    {
        var result = MatrixCellsInDistanceOrderSolution.AllCellsDistOrderByGridBfs(
            example.Rows, example.Cols, example.RCenter, example.CCenter);

        AssertDistanceOrder(result, example);
    }

    private static void AssertDistanceOrder(int[][] result, MatrixExample example)
    {
        var allCells = AllCells(example.Rows, example.Cols);

        Assert.Equal(example.Rows * example.Cols, result.Length);
        Assert.Equal(allCells, result.OrderBy(cell => cell[0]).ThenBy(cell => cell[1]));
        Assert.Equal(
            example.ExpectedDistances,
            result.Select(cell => Math.Abs(cell[0] - example.RCenter) + Math.Abs(cell[1] - example.CCenter)));
    }

    private static int[][] AllCells(int rows, int cols) =>
        Enumerable.Range(0, rows)
            .SelectMany(row => Enumerable.Range(0, cols).Select(col => new[] { row, col }))
            .ToArray();

    // One LeetCode example: the grid, the center to measure from, and the Manhattan
    // distances the returned cells must have, in order. The four travel together at
    // every call site - the assertion helper needs all of them to derive the expected
    // distances - so they are one thing with a name.
    public readonly record struct MatrixExample(int Rows, int Cols, int RCenter, int CCenter, int[] ExpectedDistances);
}
