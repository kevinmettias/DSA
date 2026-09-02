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

        var canEscape = IsEscapePossible(blocked, [0, 0], [0, 2]);

        Assert.False(canEscape);
    }

    [Fact]
    public void IsEscapePossible_NoBlockedCells_ReturnsTrue()
    {
        var canEscape = IsEscapePossible([], [0, 0], [999_999, 999_999]);

        Assert.True(canEscape);
    }

    [Fact]
    public void IsEscapePossible_PartialWallWithGap_ReturnsTrue()
    {
        // A three-cell wall one row below the source, with a gap at column 5, so
        // the source can slip through to the open board rather than being sealed in.
        int[][] blocked = [[1, 3], [1, 4], [1, 6]];

        var canEscape = IsEscapePossible(blocked, [0, 5], [50, 50]);

        Assert.True(canEscape);
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

    // Mutable per-search counter: Traverse calls the successors func exactly once per
    // newly visited node, so this tracks visited count across calls without needing
    // access to Traverse's own internals. A plain captured local can't do this once
    // Successors moves out to a real method instead of a closure, hence this holder.
    private sealed class VisitBudget
    {
        public int VisitedCount;
    }

    private static bool CanEscapeOrReach(
        (int Row, int Col) start, (int Row, int Col) other, Set<(int Row, int Col)> blocked, int threshold)
    {
        var budget = new VisitBudget();
        var reached = DepthFirstSearch.Traverse(start, cell => Successors(cell, blocked, threshold, budget));

        return reached.Count > threshold || reached.Contains(other);
    }

    private static IEnumerable<(int Row, int Col)> Successors(
        (int Row, int Col) cell, Set<(int Row, int Col)> blocked, int threshold, VisitBudget budget)
    {
        budget.VisitedCount++;

        if (budget.VisitedCount > threshold)
        {
            yield break;
        }

        foreach (var direction in Directions)
        {
            if (TryGetOpenNeighbor(cell, direction, blocked, out var next))
            {
                yield return next;
            }
        }
    }

    private static bool TryGetOpenNeighbor(
        (int Row, int Col) cell, (int DRow, int DCol) direction, Set<(int Row, int Col)> blocked, out (int Row, int Col) next)
    {
        next = (Row: cell.Row + direction.DRow, Col: cell.Col + direction.DCol);

        if (next.Row < 0 || next.Row > MaxCoordinate || next.Col < 0 || next.Col > MaxCoordinate)
        {
            return false;
        }

        return !blocked.Has(next);
    }
}
