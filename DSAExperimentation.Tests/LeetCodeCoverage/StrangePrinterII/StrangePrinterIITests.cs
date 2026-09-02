using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.LeetCodeCoverage.StrangePrinterII.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StrangePrinterII;

// LeetCode 1591. Strange Printer II: each color's bounding rectangle must have
// printed as one solid block, so any other color found inside that rectangle had
// to be printed afterward, on top of it. That "must print before" relation is
// exactly a dependency graph - the grid is printable iff that graph has no cycle,
// which TopologicalSort.TrySort already answers for LeetCode 207/210's identical
// shape (CourseScheduleTests.cs).
public sealed partial class StrangePrinterIITests
{
    [Fact]
    public void IsPrintable_NestedRectangles_ReturnsTrue()
    {
        int[][] grid =
        [
            [1, 1, 1, 1],
            [1, 2, 2, 1],
            [1, 2, 2, 1],
            [1, 1, 1, 1],
        ];

        Assert.True(IsPrintable(grid));
    }

    [Fact]
    public void IsPrintable_TwoColorsMutuallyOverlapping_ReturnsFalse()
    {
        int[][] grid =
        [
            [1, 2, 1],
            [2, 1, 2],
        ];

        Assert.False(IsPrintable(grid));
    }

    private static bool IsPrintable(int[][] grid)
    {
        var nodes = new Dictionary<int, ColorNode>();
        var bounds = new ColorBounds(
            new Dictionary<int, int>(), new Dictionary<int, int>(),
            new Dictionary<int, int>(), new Dictionary<int, int>());

        ComputeColorBounds(grid, bounds, nodes);

        var context = new PrintOrderContext(grid, new HashSet<(int Before, int After)>(), nodes);
        BuildPrintOrderEdges(context, bounds);

        return TopologicalSort.TrySort<
            ColorNode, ColorTopology, ListChildren<ColorNode>,
            NaturalChildOrder<ColorNode, ListChildren<ColorNode>>, ListChildren<ColorNode>>(
            nodes.Values, out _);
    }

    private static void ComputeColorBounds(int[][] grid, ColorBounds bounds, Dictionary<int, ColorNode> nodes)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                RecordColorBounds(grid, (r, c), bounds, nodes);
            }
        }
    }

    // Ensures grid[cell] color has a node, then widens that color's bounding
    // rectangle to include this cell.
    private static void RecordColorBounds(int[][] grid, (int Row, int Col) cell, ColorBounds bounds, Dictionary<int, ColorNode> nodes)
    {
        var color = grid[cell.Row][cell.Col];
        GetOrAddNode(nodes, color);
        ExpandBound(bounds.MinRow, color, cell.Row, Math.Min);
        ExpandBound(bounds.MaxRow, color, cell.Row, Math.Max);
        ExpandBound(bounds.MinCol, color, cell.Col, Math.Min);
        ExpandBound(bounds.MaxCol, color, cell.Col, Math.Max);
    }

    private static void ExpandBound(Dictionary<int, int> bounds, int color, int value, Func<int, int, int> combine)
        => bounds[color] = bounds.TryGetValue(color, out var existing) ? combine(existing, value) : value;

    private static void BuildPrintOrderEdges(PrintOrderContext context, ColorBounds bounds)
    {
        foreach (var color in context.Nodes.Keys)
        {
            for (var r = bounds.MinRow[color]; r <= bounds.MaxRow[color]; r++)
            {
                for (var c = bounds.MinCol[color]; c <= bounds.MaxCol[color]; c++)
                {
                    RecordPrintOrderEdge(context, color, (r, c));
                }
            }
        }
    }

    // A color found strictly inside another color's bounding rectangle had to be
    // printed on top of it, so it must print before that color - one topological
    // dependency edge per (color, other) pair the first time it's seen.
    private static void RecordPrintOrderEdge(PrintOrderContext context, int color, (int Row, int Col) cell)
    {
        var other = context.Grid[cell.Row][cell.Col];

        if (other != color && context.SeenEdges.Add((color, other)))
        {
            var beforeNode = GetOrAddNode(context.Nodes, color);
            var afterNode = GetOrAddNode(context.Nodes, other);
            beforeNode.MustPrintBefore.Add(afterNode);
        }
    }

    private static ColorNode GetOrAddNode(Dictionary<int, ColorNode> nodes, int color)
    {
        if (!nodes.TryGetValue(color, out var node))
        {
            node = new ColorNode(color);
            nodes[color] = node;
        }

        return node;
    }

    private readonly record struct ColorBounds(
        Dictionary<int, int> MinRow, Dictionary<int, int> MaxRow,
        Dictionary<int, int> MinCol, Dictionary<int, int> MaxCol);

    private readonly record struct PrintOrderContext(
        int[][] Grid, HashSet<(int Before, int After)> SeenEdges, Dictionary<int, ColorNode> Nodes);
}
