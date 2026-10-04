using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.LastDayWhereYouCanStillCross;

// LeetCode 1970. Last Day Where You Can Still Cross: the last day on which some
// 4-directional path of land still joins the top row to the bottom row.
//
// Crossability is monotone in the day - once flooding disconnects the two rows, more
// flooding never reconnects them - so "is day d blocked" is a [false...false,
// true...true] sequence and the answer is one bisection away. Both strategies bisect
// the same predicate and each probe is the same single DepthFirstSearch.Traverse; they
// differ only in who runs the bisection, which is the whole comparison.
internal static class LastDayWhereYouCanStillCrossSolution
{

    // A virtual node above the grid, wired to every still-dry top-row cell, so one
    // Traverse call replaces one search per dry top-row column.
    private const int AboveGrid = -1;

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook baseline: a hand-rolled lo/hi bisection over the same predicate.
    // Deliberately written without this repo's BinarySearch - it is the arm the
    // sequence strategy has to justify itself against.
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
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (CanCross(flooding, mid))
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

    // Holds(day) answers "can the grid still be crossed on this day" - true up to the
    // answer and false from there on, the monotonicity LastTrue assumes but never
    // checks. The range starts at day 0 because nothing flooded yet is a legitimate
    // day to ask about, and LastTrue's "none holds" answer of day -1 is never reached
    // for a grid with a dry day 0.
    private readonly struct CrossableOnDay(FloodSchedule flooding) : IMonotonePredicate<int>
    {
        public bool Holds(int day) => CanCross(flooding, day);
    }
}
