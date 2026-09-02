using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.BuildAMatrixWithConditions.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BuildAMatrixWithConditions;

// LeetCode 2392. Build a Matrix With Conditions: this repo's own Kahn's-algorithm
// TopologicalSort.TrySort (CourseScheduleIITests' own precedent for reading back
// the actual ordering, not just whether one exists), run once over rowConditions
// and once over colConditions, gives every value's row index and column index
// directly from each ordering's position - and TrySort's own false-on-cycle
// result is exactly LeetCode's "return an empty matrix" signal, so no separate
// cycle check is needed either.
public sealed partial class BuildAMatrixWithConditionsTests
{
    [Fact]
    public void BuildMatrix_ClassicExample_PlacesEachValueSatisfyingBothOrders()
    {
        int[][] rowConditions = [[1, 2], [3, 2]];
        int[][] colConditions = [[2, 1]];

        var matrix = BuildMatrix(3, rowConditions, colConditions);

        Assert.NotEmpty(matrix);
        AssertRowBefore(matrix, 1, 2);
        AssertRowBefore(matrix, 3, 2);
        AssertColumnBefore(matrix, 2, 1);
    }

    [Fact]
    public void BuildMatrix_CyclicRowConditions_ReturnsEmptyMatrix()
    {
        int[][] rowConditions = [[1, 2], [2, 3], [3, 1]];
        int[][] colConditions = [];

        var matrix = BuildMatrix(3, rowConditions, colConditions);

        Assert.Empty(matrix);
    }

    [Fact]
    public void BuildMatrix_NoConditions_PlacesEveryValueExactlyOnce()
    {
        var matrix = BuildMatrix(2, [], []);

        Assert.NotEmpty(matrix);
        var placedValues = matrix.SelectMany(row => row).Where(value => value != 0).OrderBy(value => value);
        Assert.Equal([1, 2], placedValues);
    }

    private static void AssertRowBefore(int[][] matrix, int before, int after)
        => Assert.True(FindRow(matrix, before) < FindRow(matrix, after));

    private static void AssertColumnBefore(int[][] matrix, int before, int after)
        => Assert.True(FindColumn(matrix, before) < FindColumn(matrix, after));

    private static int FindRow(int[][] matrix, int value)
    {
        for (var r = 0; r < matrix.Length; r++)
        {
            if (Array.IndexOf(matrix[r], value) >= 0)
            {
                return r;
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

    private static int[][] BuildMatrix(int k, int[][] rowConditions, int[][] colConditions)
    {
        if (!TryOrder(k, rowConditions, out var rowOrder) || !TryOrder(k, colConditions, out var colOrder))
        {
            return [];
        }

        var rowIndex = new int[k + 1];
        for (var i = 0; i < rowOrder.Count; i++)
        {
            rowIndex[rowOrder[i]] = i;
        }

        var colIndex = new int[k + 1];
        for (var i = 0; i < colOrder.Count; i++)
        {
            colIndex[colOrder[i]] = i;
        }

        var matrix = new int[k][];
        for (var r = 0; r < k; r++)
        {
            matrix[r] = new int[k];
        }

        for (var value = 1; value <= k; value++)
        {
            matrix[rowIndex[value]][colIndex[value]] = value;
        }

        return matrix;
    }

    private static bool TryOrder(int k, int[][] conditions, out List<int> order)
    {
        var nodes = Enumerable.Range(1, k).Select(id => new ValueNode(id)).ToArray();

        foreach (var condition in conditions)
        {
            nodes[condition[0] - 1].After.Add(nodes[condition[1] - 1]);
        }

        var canOrder = TopologicalSort.TrySort<
            ValueNode, ValueTopology, ListChildren<ValueNode>,
            NaturalChildOrder<ValueNode, ListChildren<ValueNode>>, ListChildren<ValueNode>>(
            nodes, out var ordering);

        order = canOrder ? ordering.Select(n => n.Id).ToList() : [];
        return canOrder;
    }
}
