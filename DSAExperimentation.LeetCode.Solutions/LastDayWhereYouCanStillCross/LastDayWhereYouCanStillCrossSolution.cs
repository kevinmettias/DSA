using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.LeetCode.Solutions.LastDayWhereYouCanStillCross;

// LeetCode 1970. Last Day Where You Can Still Cross: the last day on which some
// 4-directional path of land still joins the top row to the bottom row.
//
// Crossability is monotone in the day - once flooding disconnects the two rows, more
// flooding never reconnects them - so "is day d blocked" is a [false...false,
// true...true] sequence and the answer is one bisection away. Both strategies bisect
// the same predicate. The baseline runs the bisection by hand and probes each day with
// its own explicit-stack flood fill; the composed arm hands the bisection to
// MonotonePredicateSearch and probes with ConnectedComponents.ReduceEach over
// GridTopology.
internal static class LastDayWhereYouCanStillCrossSolution
{
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
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

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

            foreach (var (deltaRow, deltaCol) in GridDirections.Orthogonal)
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

    // The composed arm's probe: the day's dry cells become a Grid, every dry top-row
    // cell roots a walk unless an earlier one reached it, and the grid can be crossed
    // when some walk enters the last row. PassableNodes lists the grid row by row, so
    // the top row's dry cells are the ones it lists before any other row's.
    private static bool CanCross(FloodSchedule flooding, int day)
    {
        var dry = Grid.Where(new GridSize(flooding.Rows, flooding.Cols), new DryOnDay(flooding, day));
        var topRow = dry.PassableNodes<NodePerCell>().TakeWhile(cell => cell.Row == 0);
        var walks = ConnectedComponents.ReduceEach<
            GridNode, GridTopology<NodePerCell>, GridChildren<NodePerCell>, DepthFirstReduceOrder<GridNode>, ReachesLastRow, Crossing>(
            topRow, Crossing.Blocked);

        return walks.Contains(Crossing.Crossed);
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

    // The cells still land on the day being probed.
    private readonly struct DryOnDay(FloodSchedule flooding, int day) : IGridCellFilter
    {
        private readonly FloodSchedule _flooding = flooding;
        private readonly int _day = day;

        public bool CanEnter(int row, int col) => _flooding.IsLand(row, col, _day);
    }

    // What one walk from the top row has found: Blocked until it enters the last row.
    private enum Crossing
    {
        Blocked,
        Crossed,
    }

    // Crossed from the first cell on the grid's last row, read off the node's own Grid,
    // and from then on.
    private readonly struct ReachesLastRow : IReduceAlgebra<GridNode, Crossing>
    {
        public static Crossing Seed => Crossing.Blocked;

        public static Crossing Enter(Crossing state, GridNode node, int depth)
        {
            var isOnLastRow = node.Row == node.Grid.Rows - 1;

            return isOnLastRow ? Crossing.Crossed : state;
        }
    }
}
