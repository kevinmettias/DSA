using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.StrangePrinterII;

// LeetCode 1591. Strange Printer II: the printer can only ever lay down a solid
// axis-aligned rectangle of one color, so each color's bounding rectangle must
// have been printed as a single block. Any *other* color still visible inside that
// rectangle therefore had to be printed afterward, on top of it. That "must print
// before" relation is exactly a dependency graph, and the grid is printable iff
// that graph has no cycle - the identical question LeetCode/CourseSchedule asks
// for LC 207.
//
// Both strategies decide the same cycle question over the same color graph; they
// differ only in how the next zero-in-degree color is found:
//
//   - IsPrintableByNaiveRescan rescans every remaining color from scratch on each
//     step, O(V^2 + V*E). Deliberately written with nothing but BCL collections -
//     it is the arm the composed solution has to justify itself against.
//   - IsPrintableByKahnsTopologicalSort is this repo's own TopologicalSort.TrySort,
//     Kahn's algorithm proper, O(V+E) via a queue of already-zero-in-degree colors.
//
// Each takes a second overload on the prepared color graph (§17.4) so
// StrangePrinterIIBenchmarks can charge graph construction to its [GlobalSetup];
// List<ColorNode> can never be confused with LeetCode's own int[][] grid shape.
internal static class StrangePrinterIISolution
{
    // This repo's own Kahn's algorithm. TrySort returns false exactly when the
    // dependency graph has a cycle, which is one-for-one LC 1591's "not printable".
    public static bool IsPrintableByKahnsTopologicalSort(int[][] targetGrid) =>
        IsPrintableByKahnsTopologicalSort(BuildColorGraph(targetGrid));

    public static bool IsPrintableByKahnsTopologicalSort(List<ColorNode> colors) =>
        TopologicalSort.TrySort<
            ColorNode, ColorTopology, ListChildren<ColorNode>,
            NaturalChildOrder<ColorNode, ListChildren<ColorNode>>, ListChildren<ColorNode>>(
            colors, out _);

    // The textbook answer: keep a remaining-colors list and linearly scan it for a
    // color with no unprinted prerequisites, rather than maintaining a frontier
    // queue. Printable iff every color eventually gets printed.
    public static bool IsPrintableByNaiveRescan(int[][] targetGrid) =>
        IsPrintableByNaiveRescan(BuildColorGraph(targetGrid));

    public static bool IsPrintableByNaiveRescan(List<ColorNode> colors)
    {
        var inDegree = colors.ToDictionary(color => color, _ => 0);

        foreach (var color in colors)
        {
            foreach (var dependent in color.MustPrintBefore)
            {
                inDegree[dependent]++;
            }
        }

        var remaining = new List<ColorNode>(colors);
        var printed = 0;

        while (remaining.Count > 0 && TryPrintNextReadyColor(remaining, inDegree))
        {
            printed++;
        }

        return printed == colors.Count;
    }

    private static bool TryPrintNextReadyColor(List<ColorNode> remaining, Dictionary<ColorNode, int> inDegree)
    {
        var next = remaining.FirstOrDefault(color => inDegree[color] == 0);

        if (next is null)
        {
            return false;
        }

        remaining.Remove(next);

        foreach (var dependent in next.MustPrintBefore)
        {
            inDegree[dependent]--;
        }

        return true;
    }

    private static List<ColorNode> BuildColorGraph(int[][] targetGrid)
    {
        var rectangles = ComputeColorRectangles(targetGrid);
        var nodes = rectangles.Keys.ToDictionary(color => color, color => new ColorNode(color));
        var seenEdges = new HashSet<(int Before, int After)>();

        foreach (var (color, rectangle) in rectangles)
        {
            AddCoveredColorEdges(targetGrid, color, rectangle, nodes, seenEdges);
        }

        return nodes.Values.ToList();
    }

    // The smallest axis-aligned rectangle containing every cell of each color -
    // the block that color must have been printed as.
    private static Dictionary<int, ColorRectangle> ComputeColorRectangles(int[][] targetGrid)
    {
        var rectangles = new Dictionary<int, ColorRectangle>();

        for (var row = 0; row < targetGrid.Length; row++)
        {
            for (var col = 0; col < targetGrid[row].Length; col++)
            {
                var color = targetGrid[row][col];
                rectangles[color] = rectangles.TryGetValue(color, out var existing)
                    ? existing.Expand(row, col)
                    : new ColorRectangle(row, row, col, col);
            }
        }

        return rectangles;
    }

    // A different color found inside this color's rectangle was printed on top of
    // it, so this color must print first - one dependency edge per (color, other)
    // pair, the first time that pair is seen.
    private static void AddCoveredColorEdges(
        int[][] targetGrid,
        int color,
        ColorRectangle rectangle,
        Dictionary<int, ColorNode> nodes,
        HashSet<(int Before, int After)> seenEdges)
    {
        for (var row = rectangle.MinRow; row <= rectangle.MaxRow; row++)
        {
            for (var col = rectangle.MinCol; col <= rectangle.MaxCol; col++)
            {
                var other = targetGrid[row][col];

                if (other != color && seenEdges.Add((color, other)))
                {
                    nodes[color].MustPrintBefore.Add(nodes[other]);
                }
            }
        }
    }

    private readonly record struct ColorRectangle(int MinRow, int MaxRow, int MinCol, int MaxCol)
    {
        public ColorRectangle Expand(int row, int col) => new(
            Math.Min(MinRow, row), Math.Max(MaxRow, row),
            Math.Min(MinCol, col), Math.Max(MaxCol, col));
    }
}
