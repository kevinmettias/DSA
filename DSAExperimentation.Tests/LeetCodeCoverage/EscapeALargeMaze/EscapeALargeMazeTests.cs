using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EscapeALargeMaze;

// LeetCode 1036. Escape a Large Maze: the 10^6 x 10^6 board is never materialized -
// this repo's DepthFirstSearch.Traverse already models an implicit graph via a bare
// Func<TNode,IEnumerable<TNode>> (its own doc comment names exactly this "board too
// large to store" shape), and Set<(int,int)> holds the >= 200 blocked cells for O(1)
// membership. The successor closure caps itself at the standard
// blocked.Length*(blocked.Length-1)/2 bound - the largest area <= 200 blocked cells
// can wall off against a corner - by counting its own invocations (Traverse calls
// successors exactly once per newly visited node, so the closure's own counter
// tracks the visited count without needing access to Traverse's internals) and
// returning no further neighbors past that bound. Reaching the bound means the
// start cell escaped into open space rather than a sealed pocket; running that
// capped search from both source and target (and checking whether either search
// reaches the other) is the full accepted algorithm.
public sealed partial class EscapeALargeMazeTests
{
    [Fact]
    public void IsEscapePossible_SourceFullyWalledIntoCorner_ReturnsFalse()
    {
        int[][] blocked = [[0, 1], [1, 0]];

        Assert.False(IsEscapePossible(blocked, [0, 0], [0, 2]));
    }

    [Fact]
    public void IsEscapePossible_NoBlockedCells_ReturnsTrue()
    {
        Assert.True(IsEscapePossible([], [0, 0], [999_999, 999_999]));
    }

    [Fact]
    public void IsEscapePossible_PartialWallWithGap_ReturnsTrue()
    {
        // A three-cell wall one row below the source, with a gap at column 5, so
        // the source can slip through to the open board rather than being sealed in.
        int[][] blocked = [[1, 3], [1, 4], [1, 6]];

        Assert.True(IsEscapePossible(blocked, [0, 5], [50, 50]));
    }

    private const int MaxCoordinate = 999_999;
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private static bool IsEscapePossible(int[][] blockedCells, int[] source, int[] target)
    {
        var blocked = new Set<(int Row, int Col)>();

        foreach (var cell in blockedCells)
        {
            blocked.TryAdd((cell[0], cell[1]));
        }

        var threshold = blockedCells.Length * (blockedCells.Length - 1) / 2;
        var from = (source[0], source[1]);
        var to = (target[0], target[1]);

        return CanEscapeOrReach(from, to, blocked, threshold) && CanEscapeOrReach(to, from, blocked, threshold);
    }

    private static bool CanEscapeOrReach(
        (int Row, int Col) start, (int Row, int Col) other, Set<(int Row, int Col)> blocked, int threshold)
    {
        var visitedCount = 0;

        var reached = DepthFirstSearch.Traverse(start, Successors);

        return reached.Count > threshold || reached.Contains(other);

        IEnumerable<(int Row, int Col)> Successors((int Row, int Col) cell)
        {
            visitedCount++;

            if (visitedCount > threshold)
            {
                yield break;
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

                if (next.Row < 0 || next.Row > MaxCoordinate || next.Col < 0 || next.Col > MaxCoordinate)
                {
                    continue;
                }

                if (blocked.Has(next))
                {
                    continue;
                }

                yield return next;
            }
        }
    }
}
