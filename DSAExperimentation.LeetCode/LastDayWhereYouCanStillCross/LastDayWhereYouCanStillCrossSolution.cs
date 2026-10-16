using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.LastDayWhereYouCanStillCross;

// LeetCode 1970. Last Day Where You Can Still Cross: the last day on which some
// 4-directional path of land still joins the top row to the bottom row.
//
// Crossability is monotone in the day - once flooding disconnects the two rows, more
// flooding never reconnects them - so "is day d blocked" is a [false...false,
// true...true] sequence and the answer is one bisection away. Both strategies bisect
// the same predicate. The baseline runs the bisection by hand and probes each day with
// its own explicit-stack flood fill; the composed arm hands the bisection to
// MonotonePredicateSearch and probes with one DepthFirstSearch.Traverse.
internal static class LastDayWhereYouCanStillCrossSolution
{
    // The divisor a range is split by, restated rather than read from AlgorithmConstants: a baseline
    // reaches no library type, not even a constant (ARCHITECTURE 17.5).
    private const int HalvingFactor = 2;

    // A virtual node above the grid, wired to every still-dry top-row cell, so one
    // Traverse call replaces one search per dry top-row column.
    private const int AboveGrid = -1;

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook baseline: a hand-rolled lo/hi bisection over the same predicate,
    // each probe a Stack-driven flood fill. Deliberately written without this repo's
    // BinarySearch or DepthFirstSearch - it is the arm the sequence strategy has to
    // justify itself against (ARCHITECTURE 17.5).
    public static int LatestDayToCrossByManualBisection(int row, int col, int[][] cells)
    {
        var flooding = FloodSchedule.Build(row, col, cells);

        return LatestDayToCrossByManualBisection(flooding);
    }

    public static int LatestDayToCrossByManualBisection(FloodSchedule flooding)
    {
        var low = 0;
        var high = flooding.LastDay;

        while (low < high)
        {
            var mid = low + ((high - low) / HalvingFactor);

            if (CanCrossByStackWalk(flooding, mid))
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        return low - 1;
    }

    // This repo's own bisection: MonotonePredicateSearch.LastTrue over the days, the
    // "binary search on the answer" Koko Eating Bananas and Split Array Largest Sum
    // already use, asked for the other end. The question is the last day the rule
    // holds, so the rule is stated as crossability itself rather than as "blocked"
    // with the answer recovered one day back.
    public static int LatestDayToCrossByPredicateSearch(int row, int col, int[][] cells)
    {
        var flooding = FloodSchedule.Build(row, col, cells);

        return LatestDayToCrossByPredicateSearch(flooding);
    }

    public static int LatestDayToCrossByPredicateSearch(FloodSchedule flooding) =>
        MonotonePredicateSearch.LastTrue(0, flooding.LastDay, new CrossableOnDay(flooding));

    // The baseline's probe: a depth-first flood fill from every top-row cell still dry
    // on `day`, over a BCL Stack and a bool[,] visited map, true the moment it reaches
    // the bottom row.
    private static bool CanCrossByStackWalk(FloodSchedule flooding, int day)
    {
        var (visited, pending) = DryTopRowFrontier(flooding, day);

        return CanReachBottomRow(flooding, day, visited, pending);
    }

    // Every top-row cell still dry on `day`, marked visited and pushed as a start.
    private static (bool[,] Visited, Stack<(int Row, int Col)> Pending) DryTopRowFrontier(
        FloodSchedule flooding, int day)
    {
        var visited = new bool[flooding.Rows, flooding.Cols];
        var pending = new Stack<(int Row, int Col)>();

        for (var col = 0; col < flooding.Cols; col++)
        {
            if (flooding.IsLand(0, col, day))
            {
                visited[0, col] = true;
                pending.Push((0, col));
            }
        }

        return (visited, pending);
    }

    private static bool CanReachBottomRow(
        FloodSchedule flooding, int day, bool[,] visited, Stack<(int Row, int Col)> pending)
    {
        while (pending.TryPop(out var cell))
        {
            if (cell.Row == flooding.Rows - 1)
            {
                return true;
            }

            foreach (var (deltaRow, deltaCol) in Directions)
            {
                var next = (Row: cell.Row + deltaRow, Col: cell.Col + deltaCol);

                if (IsUnvisitedLand(flooding, day, visited, next))
                {
                    visited[next.Row, next.Col] = true;
                    pending.Push(next);
                }
            }
        }

        return false;
    }

    private static bool IsUnvisitedLand(FloodSchedule flooding, int day, bool[,] visited, (int Row, int Col) cell)
        => flooding.IsInBounds(cell.Row, cell.Col)
            && flooding.IsLand(cell.Row, cell.Col, day)
            && !visited[cell.Row, cell.Col];

    private static bool CanCross(FloodSchedule flooding, int day)
        => DepthFirstSearch.Traverse(
                (Row: AboveGrid, Col: AboveGrid), cell => Neighbors(cell, flooding, day))
            .Any(cell => cell.Row == flooding.Rows - 1);

    private static IEnumerable<(int Row, int Col)> Neighbors(
        (int Row, int Col) cell, FloodSchedule flooding, int day)
    {
        if (cell.Row == AboveGrid)
        {
            for (var col = 0; col < flooding.Cols; col++)
            {
                if (flooding.IsLand(0, col, day))
                {
                    yield return (0, col);
                }
            }

            yield break;
        }

        foreach (var (deltaRow, deltaCol) in Directions)
        {
            var next = (Row: cell.Row + deltaRow, Col: cell.Col + deltaCol);

            if (flooding.IsInBounds(next.Row, next.Col) && flooding.IsLand(next.Row, next.Col, day))
            {
                yield return next;
            }
        }
    }

    // IsSatisfiedBy(day) answers "can the grid still be crossed on this day" - true up to the
    // answer and false from there on, the monotonicity LastTrue assumes but never
    // checks. The range starts at day 0 because nothing flooded yet is a legitimate
    // day to ask about, and LastTrue's "none holds" answer of day -1 is never reached
    // for a grid with a dry day 0.
    private readonly struct CrossableOnDay(FloodSchedule flooding) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int day) => CanCross(flooding, day);
    }
}
