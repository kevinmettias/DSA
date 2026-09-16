using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

// LeetCode 2392. Build a Matrix With Conditions: place 1..k in a k x k matrix so
// that every rowCondition [a, b] puts a in a strictly higher row than b and every
// colCondition [a, b] puts a in a strictly earlier column than b.
//
// The two axes never interact: a value's row is decided by rowConditions alone and
// its column by colConditions alone, so the problem is two independent topological
// sorts over the same 1..k vertex set, and a value's position on each axis is just
// its index in that axis's ordering. A cycle on either axis makes the constraints
// unsatisfiable, which is LeetCode's "return an empty matrix" case - and it is
// exactly the false result Kahn's algorithm already reports, so no separate cycle
// check is needed.
//
// The naive baseline rescans every unplaced value for one with nothing left ahead
// of it on each step, O(V^2 + V*E); TopologicalSort.TrySort is Kahn's algorithm
// proper, O(V+E) via a queue of already-zero-in-degree values.
internal static class BuildAMatrixWithConditionsSolution
{
    // This repo's own Kahn's algorithm, once per axis. TrySort's false-on-cycle
    // result is LeetCode's empty-matrix answer one-for-one.
    public static int[][] BuildMatrixByKahnsTopologicalSort(
        int valueCount, int[][] rowConditions, int[][] colConditions)
    {
        var rowValues = BuildValues(valueCount, rowConditions);
        var colValues = BuildValues(valueCount, colConditions);

        return BuildMatrixByKahnsTopologicalSort(rowValues, colValues);
    }

    public static int[][] BuildMatrixByKahnsTopologicalSort(
        List<ValueNode> rowValues, List<ValueNode> colValues) =>
        TryKahnsOrder(rowValues, out var rowOrder) && TryKahnsOrder(colValues, out var colOrder)
            ? Place(rowOrder, colOrder)
            : Array.Empty<int[]>();

    private static bool TryKahnsOrder(List<ValueNode> values, out List<int> order)
    {
        var ordered = TopologicalSort.TrySort<
            ValueNode, ValueTopology, ListChildren<ValueNode>,
            NaturalChildOrder<ValueNode, ListChildren<ValueNode>>, ListChildren<ValueNode>>(
            values, out var ordering);

        order = ordered ? Ids(ordering) : new List<int>();
        return ordered;
    }

    // The ids of an ordering, in order.
    private static List<int> Ids(List<ValueNode> values) => values.Select(value => value.Id).ToList();

    // The textbook answer: rescan the values still unplaced from scratch on every
    // step looking for the next one with nothing left ahead of it, rather than
    // tracking a frontier queue. Deliberately written without this repo's
    // TopologicalSort - it is the arm the composed strategy above has to justify
    // itself against.
    public static int[][] BuildMatrixByNaiveRescan(int valueCount, int[][] rowConditions, int[][] colConditions)
    {
        var rowValues = BuildValues(valueCount, rowConditions);
        var colValues = BuildValues(valueCount, colConditions);

        return BuildMatrixByNaiveRescan(rowValues, colValues);
    }

    public static int[][] BuildMatrixByNaiveRescan(List<ValueNode> rowValues, List<ValueNode> colValues) =>
        TryNaiveRescanOrder(rowValues, out var rowOrder) && TryNaiveRescanOrder(colValues, out var colOrder)
            ? Place(rowOrder, colOrder)
            : Array.Empty<int[]>();

    private static bool TryNaiveRescanOrder(List<ValueNode> values, out List<int> order)
    {
        var inDegree = values.ToDictionary(value => value, _ => 0);
        foreach (var value in values)
        {
            foreach (var next in value.After)
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<ValueNode>(values);
        var placed = new List<ValueNode>(values.Count);

        while (remaining.Count > 0 && TryAdvanceNaiveRescan(remaining, inDegree, placed))
        {
        }

        order = placed.Count == values.Count ? Ids(placed) : new List<int>();
        return placed.Count == values.Count;
    }

    private static bool TryAdvanceNaiveRescan(
        List<ValueNode> remaining, Dictionary<ValueNode, int> inDegree, List<ValueNode> placed)
    {
        var next = remaining.FirstOrDefault(value => inDegree[value] == 0);

        if (next is null)
        {
            return false;
        }

        placed.Add(next);
        remaining.Remove(next);

        foreach (var dependent in next.After)
        {
            inDegree[dependent]--;
        }

        return true;
    }

    // Each ordering is a permutation of 1..k, so a value's position in the row
    // ordering IS its row and its position in the column ordering IS its column -
    // and because both index maps are injective, no two values ever land in the
    // same cell. Everything left over stays 0, LeetCode's "any value" filler.
    private static int[][] Place(List<int> rowOrder, List<int> colOrder)
    {
        var k = rowOrder.Count;
        var rowIndex = new int[k + 1];
        var colIndex = new int[k + 1];

        for (var i = 0; i < k; i++)
        {
            rowIndex[rowOrder[i]] = i;
            colIndex[colOrder[i]] = i;
        }

        var matrix = new int[k][];
        for (var row = 0; row < k; row++)
        {
            matrix[row] = new int[k];
        }

        for (var value = 1; value <= k; value++)
        {
            matrix[rowIndex[value]][colIndex[value]] = value;
        }

        return matrix;
    }

    // LeetCode's own input shape: conditions[i] = [a, b] means a must be placed
    // before b on this axis, i.e. an edge from a to b.
    private static List<ValueNode> BuildValues(int valueCount, int[][] conditions)
    {
        var values = new List<ValueNode>(valueCount);

        for (var id = 1; id <= valueCount; id++)
        {
            values.Add(new ValueNode(id));
        }

        foreach (var condition in conditions)
        {
            values[condition[0] - 1].After.Add(values[condition[1] - 1]);
        }

        return values;
    }
}
