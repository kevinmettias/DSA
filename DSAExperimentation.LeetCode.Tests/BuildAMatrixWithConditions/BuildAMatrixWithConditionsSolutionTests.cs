using DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

namespace DSAExperimentation.LeetCode.Tests.BuildAMatrixWithConditions;

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
public sealed partial class BuildAMatrixWithConditionsSolutionTests
{
    // LeetCode example 1, and the output LeetCode published for it - one of the several
    // matrices it accepts, so the Examples row asserts satisfiability rather than this matrix.
    private static readonly MatrixExample LeetCodeExample1 =
        new(K: 3, RowConditions: [[1, 2], [3, 2]], ColConditions: [[2, 1], [3, 2]], Satisfiable: true);

    private static readonly int[][] LeetCodeExample1PublishedOutput = [[3, 0, 0], [0, 0, 1], [0, 2, 0]];

    public static TheoryData<MatrixExample> Examples =>
        new()
        {
            // LeetCode example 1, stated above beside its published output.
            { LeetCodeExample1 },

            // LeetCode example 2: 1 above 2 above 3 above 1 is a cycle, so the output is [].
            {
                new MatrixExample(
                    K: 3, RowConditions: [[1, 2], [2, 3], [3, 1], [2, 3]], ColConditions: [[2, 1]], Satisfiable: false)
            },

            // Example 1 with its column condition [3, 2] dropped, and example 2 with its
            // repeated [2, 3] and its column conditions dropped.
            { new MatrixExample(K: 3, RowConditions: [[1, 2], [3, 2]], ColConditions: [[2, 1]], Satisfiable: true) },
            { new MatrixExample(K: 3, RowConditions: [[1, 2], [2, 3], [3, 1]], ColConditions: [], Satisfiable: false) },
            { new MatrixExample(K: 2, RowConditions: [], ColConditions: [], Satisfiable: true) },
            { new MatrixExample(K: 2, RowConditions: [[1, 2]], ColConditions: [[1, 2], [2, 1]], Satisfiable: false) },
            {
                new MatrixExample(
                    K: 4, RowConditions: [[1, 2], [2, 3], [3, 4]],
                    ColConditions: [[4, 3], [3, 2], [2, 1]], Satisfiable: true)
            },
            { new MatrixExample(K: 5, RowConditions: [[2, 1], [3, 1]], ColConditions: [[1, 4], [5, 1]], Satisfiable: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildMatrixByKahnsTopologicalSort_LeetCodeExamples_PlacesEveryValueSatisfyingBothAxes(
        MatrixExample example)
    {
        var matrix = BuildAMatrixWithConditionsSolution.BuildMatrixByKahnsTopologicalSort(
            example.K, example.RowConditions, example.ColConditions);

        AssertSatisfies(matrix, example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildMatrixByNaiveRescan_LeetCodeExamples_PlacesEveryValueSatisfyingBothAxes(
        MatrixExample example)
    {
        var matrix = BuildAMatrixWithConditionsSolution.BuildMatrixByNaiveRescan(
            example.K, example.RowConditions, example.ColConditions);

        AssertSatisfies(matrix, example);
    }

    // The validity check is the whole assertion for a satisfiable row, so it must accept
    // what LeetCode itself published as a correct answer, not only what these strategies build.
    [Fact]
    public void AssertSatisfies_LeetCodeExample1PublishedOutput_IsAccepted() =>
        AssertSatisfies(LeetCodeExample1PublishedOutput, LeetCodeExample1);

    private static void AssertSatisfies(int[][] matrix, MatrixExample example)
    {
        if (!example.Satisfiable)
        {
            Assert.Empty(matrix);
            return;
        }

        AssertPlacesEveryValueExactlyOnce(matrix, example.K);

        foreach (var condition in example.RowConditions)
        {
            Assert.True(FindRow(matrix, condition[0]) < FindRow(matrix, condition[1]));
        }

        foreach (var condition in example.ColConditions)
        {
            Assert.True(FindColumn(matrix, condition[0]) < FindColumn(matrix, condition[1]));
        }
    }

    private static void AssertPlacesEveryValueExactlyOnce(int[][] matrix, int valueCount)
    {
        var expectedValues = Enumerable.Range(1, valueCount);
        var placedValues = matrix.SelectMany(row => row).Where(value => value != 0).Order();

        Assert.Equal(valueCount, matrix.Length);
        Assert.All(matrix, row => Assert.Equal(valueCount, row.Length));
        Assert.Equal(expectedValues, placedValues);
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

    // One LeetCode example: the bound on the values, the conditions each axis must honour,
    // and whether any matrix satisfies them all. The four travel together into every
    // assertion, so each is named rather than left as a position in a row of four literals.
    public readonly record struct MatrixExample(
        int K,
        int[][] RowConditions,
        int[][] ColConditions,
        bool Satisfiable);
}
