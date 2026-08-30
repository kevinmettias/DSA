using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PacificAtlanticWaterFlow;

// LeetCode 417. Pacific Atlantic Water Flow: multi-source reverse-flow flood fill,
// the same border-flood-fill shape SurroundedRegionsTests already uses - this
// repo's own DepthFirstSearch.Traverse walking "uphill or flat" outward from every
// Pacific-adjacent (top row/left col) and Atlantic-adjacent (bottom row/right col)
// border cell, deduped into two Set<(int,int)> reachability sets instead of
// SurroundedRegions' in-place board mutation (heights must stay untouched here).
// A cell reachable from BOTH oceans' border sweeps can flow to both in the forward
// direction, since "can flow downhill from X to a border cell" is exactly the
// reverse of "can walk uphill-or-flat from that border cell to X".
public sealed partial class PacificAtlanticWaterFlowTests
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Fact]
    public void PacificAtlantic_ClassicExample_ReturnsCellsThatReachBothOceans()
    {
        int[][] heights =
        [
            [1, 2, 2, 3, 5],
            [3, 2, 3, 4, 4],
            [2, 4, 5, 3, 1],
            [6, 7, 1, 4, 5],
            [5, 1, 1, 2, 4],
        ];

        var result = PacificAtlantic(heights);

        (int Row, int Col)[] expected = [(0, 4), (1, 3), (1, 4), (2, 2), (3, 0), (3, 1), (4, 0)];
        Assert.Equal(expected.OrderBy(p => p).ToArray(), result.OrderBy(p => p).ToArray());
    }

    [Fact]
    public void PacificAtlantic_SingleCell_ReachesBothOceans()
    {
        int[][] heights = [[5]];

        var result = PacificAtlantic(heights);

        Assert.Equal([(0, 0)], result);
    }

    [Fact]
    public void PacificAtlantic_FlatGrid_EveryCellReachesBothOceans()
    {
        int[][] heights = [[3, 3], [3, 3]];

        var result = PacificAtlantic(heights);

        (int Row, int Col)[] expected = [(0, 0), (0, 1), (1, 0), (1, 1)];
        Assert.Equal(expected.OrderBy(p => p).ToArray(), result.OrderBy(p => p).ToArray());
    }

    private static List<(int Row, int Col)> PacificAtlantic(int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;
        var pacific = new Set<(int Row, int Col)>();
        var atlantic = new Set<(int Row, int Col)>();

        for (var r = 0; r < rows; r++)
        {
            FloodFrom((r, 0), pacific);
            FloodFrom((r, cols - 1), atlantic);
        }

        for (var c = 0; c < cols; c++)
        {
            FloodFrom((0, c), pacific);
            FloodFrom((rows - 1, c), atlantic);
        }

        var result = new List<(int Row, int Col)>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (pacific.Has((r, c)) && atlantic.Has((r, c)))
                {
                    result.Add((r, c));
                }
            }
        }

        return result;

        void FloodFrom((int Row, int Col) start, Set<(int Row, int Col)> reached)
        {
            if (reached.Has(start))
            {
                return;
            }

            foreach (var node in DepthFirstSearch.Traverse(start, Neighbors))
            {
                reached.TryAdd(node);
            }
        }

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (heights[nextRow][nextCol] < heights[p.Row][p.Col])
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }
}
