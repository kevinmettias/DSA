using DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BuildAMatrixWithConditions;

// Harness only. Both strategies are BuildAMatrixWithConditionsSolution's - this
// file pins them to LeetCode's published examples plus the shapes those never
// reach: no conditions at all, a cycle on the COLUMN axis rather than the row
// axis, and chains that constrain both axes fully.
//
// LC 2392 accepts *any* matrix satisfying the conditions, so an example cannot
// state one blessed matrix; it states whether the conditions are satisfiable, and
// the shared check below verifies the returned matrix really places every value
// exactly once and honours every condition on both axes. The rescan baseline is
// asserted here too, which is the point of hoisting it into the solution class:
// before this migration it existed only as a benchmark arm nothing checked.
public sealed class BuildAMatrixWithConditionsTests
{
    public static TheoryData<int, int[][], int[][], bool> Examples =>
        new()
        {
            { 3, [[1, 2], [3, 2]], [[2, 1]], true },
            { 3, [[1, 2], [2, 3], [3, 1]], [], false },
            { 2, [], [], true },
            { 2, [[1, 2]], [[1, 2], [2, 1]], false },
            { 4, [[1, 2], [2, 3], [3, 4]], [[4, 3], [3, 2], [2, 1]], true },
            { 5, [[2, 1], [3, 1]], [[1, 4], [5, 1]], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildMatrixByKahnsTopologicalSort_LeetCodeExamples_PlacesEveryValueSatisfyingBothAxes(
        int k, int[][] rowConditions, int[][] colConditions, bool satisfiable) =>
        AssertSatisfies(
            BuildAMatrixWithConditionsSolution.BuildMatrixByKahnsTopologicalSort(k, rowConditions, colConditions),
            k, rowConditions, colConditions, satisfiable);

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildMatrixByNaiveRescan_LeetCodeExamples_PlacesEveryValueSatisfyingBothAxes(
        int k, int[][] rowConditions, int[][] colConditions, bool satisfiable) =>
        AssertSatisfies(
            BuildAMatrixWithConditionsSolution.BuildMatrixByNaiveRescan(k, rowConditions, colConditions),
            k, rowConditions, colConditions, satisfiable);

    private static void AssertSatisfies(
        int[][] matrix, int k, int[][] rowConditions, int[][] colConditions, bool satisfiable)
    {
        if (!satisfiable)
        {
            Assert.Empty(matrix);
            return;
        }

        Assert.Equal(k, matrix.Length);
        Assert.All(matrix, row => Assert.Equal(k, row.Length));
        Assert.Equal(
            Enumerable.Range(1, k),
            matrix.SelectMany(row => row).Where(value => value != 0).Order());

        foreach (var condition in rowConditions)
        {
            Assert.True(FindRow(matrix, condition[0]) < FindRow(matrix, condition[1]));
        }

        foreach (var condition in colConditions)
        {
            Assert.True(FindColumn(matrix, condition[0]) < FindColumn(matrix, condition[1]));
        }
    }

    private static int FindRow(int[][] matrix, int value)
    {
        for (var row = 0; row < matrix.Length; row++)
        {
            if (Array.IndexOf(matrix[row], value) >= 0)
            {
                return row;
            }
        }

        throw new InvalidOperationException($"Value {value} not found in matrix.");
    }

    private static int FindColumn(int[][] matrix, int value)
    {
        foreach (var row in matrix)
        {
            var column = Array.IndexOf(row, value);

            if (column >= 0)
            {
                return column;
            }
        }

        throw new InvalidOperationException($"Value {value} not found in matrix.");
    }
}
