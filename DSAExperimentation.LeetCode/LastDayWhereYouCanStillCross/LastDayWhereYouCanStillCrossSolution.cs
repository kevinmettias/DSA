using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Sequence;

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

    // This repo's own bisection: BinarySearch.LowerBound over a sequence whose
    // elements are computed on demand, so "binary search on the answer" is the same
    // call as binary search over an array - the composition Koko Eating Bananas and
    // Split Array Largest Sum already use. The first blocked day minus one is the
    // last crossable day.
    public static int LatestDayToCrossBySequenceLowerBound(int row, int col, int[][] cells)
    {
        var flooding = FloodSchedule.Build(row, col, cells);

        return LatestDayToCrossBySequenceLowerBound(flooding);
    }

    public static int LatestDayToCrossBySequenceLowerBound(FloodSchedule flooding)
    {
        var blocked = new IsBlockedSequence(flooding);

        return BinarySearch.LowerBound(blocked, true) - 1;
    }

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

            if (flooding.Contains(next.Row, next.Col) && flooding.IsLand(next.Row, next.Col, day))
            {
                yield return next;
            }
        }
    }

    // Day d, indexed directly: Get(d) answers "is the grid blocked on day d", the
    // monotone predicate LowerBound bisects. Length runs one past the last flood day
    // because day 0 (nothing flooded yet) is a legitimate answer to ask about.
    private readonly struct IsBlockedSequence(FloodSchedule flooding) : IRandomAccessSequence<bool>
    {
        public int Length => flooding.LastDay + 1;

        public bool Get(int index) => !CanCross(flooding, index);
    }
}
