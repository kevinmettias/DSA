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
        var rows = grid.Length;
        var cols = grid[0].Length;

        var nodes = new Dictionary<int, ColorNode>();
        var minRow = new Dictionary<int, int>();
        var maxRow = new Dictionary<int, int>();
        var minCol = new Dictionary<int, int>();
        var maxCol = new Dictionary<int, int>();

        ColorNode NodeFor(int color)
        {
            if (!nodes.TryGetValue(color, out var node))
            {
                node = new ColorNode(color);
                nodes[color] = node;
            }

            return node;
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var color = grid[r][c];
                NodeFor(color);
                minRow[color] = minRow.TryGetValue(color, out var mr) ? Math.Min(mr, r) : r;
                maxRow[color] = maxRow.TryGetValue(color, out var xr) ? Math.Max(xr, r) : r;
                minCol[color] = minCol.TryGetValue(color, out var mc) ? Math.Min(mc, c) : c;
                maxCol[color] = maxCol.TryGetValue(color, out var xc) ? Math.Max(xc, c) : c;
            }
        }

        var seenEdges = new HashSet<(int Before, int After)>();

        foreach (var color in nodes.Keys)
        {
            for (var r = minRow[color]; r <= maxRow[color]; r++)
            {
                for (var c = minCol[color]; c <= maxCol[color]; c++)
                {
                    var other = grid[r][c];

                    if (other != color && seenEdges.Add((color, other)))
                    {
                        NodeFor(color).MustPrintBefore.Add(NodeFor(other));
                    }
                }
            }
        }

        return TopologicalSort.TrySort<
            ColorNode, ColorTopology, ListChildren<ColorNode>,
            NaturalChildOrder<ColorNode, ListChildren<ColorNode>>, ListChildren<ColorNode>>(
            nodes.Values, out _);
    }
}
