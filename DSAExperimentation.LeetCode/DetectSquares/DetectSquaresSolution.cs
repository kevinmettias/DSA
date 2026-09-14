using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DetectSquares;

// LeetCode 2013. Detect Squares: a stream of lattice points, and a query that asks
// how many axis-aligned squares of positive area can be formed using three stored
// points plus the queried one. Duplicate points count separately, so an answer is a
// product of occurrence counts rather than a count of distinct corner triples.
//
// This is a design problem - LeetCode's own shape is a stateful object with two
// operations, not a single return value - so the strategy choice is which
// implementation backs it, the same CreateBy<Strategy> factory shape
// AllOneDataStructureSolution uses for its own design problem.
internal static class DetectSquaresSolution
{
    // Composed: points are grouped by x-coordinate into this repo's own HashMap nested
    // inside itself (x -> y -> occurrence count), the same nested-map composition
    // EvaluateDivision uses for a different problem's adjacency structure. Count only
    // ever scans points that already share the query's x-coordinate: for each such
    // point the square's side is the gap between the two y-coordinates, which pins the
    // other two corners' x-coordinate to exactly two values, so each candidate square
    // is CHECKED with two O(1) lookups instead of searched for.
    public static IDetectSquares CreateByHashMapGroupedByX() => new HashMapGroupedByXDetectSquares();

    // The textbook answer: keep every point in a flat list and rescan the whole list
    // for each candidate corner. Deliberately written without this repo's primitives -
    // a BCL List and a predicate count - because it is the arm the composed strategy
    // has to justify itself against. O(n) per corner check, so O(n^2) per query.
    public static IDetectSquares CreateByPointListScan() => new PointListScanDetectSquares();

    // LeetCode's own two operations. Duplicate Add calls are kept, not deduplicated.
    internal interface IDetectSquares
    {
        void Add(int pointX, int pointY);

        int Count(int pointX, int pointY);
    }

    private sealed class HashMapGroupedByXDetectSquares : IDetectSquares
    {
        private readonly HashMap<int, HashMap<int, int>> _countsByX = new();

        public void Add(int pointX, int pointY)
        {
            if (!_countsByX.TryGetValue(pointX, out var countsByY))
            {
                countsByY = new HashMap<int, int>();
                _countsByX.Set(pointX, countsByY);
            }

            countsByY.TryGetValue(pointY, out var existing);
            countsByY.Set(pointY, existing + 1);
        }

        public int Count(int pointX, int pointY)
        {
            if (!_countsByX.TryGetValue(pointX, out var sameColumn))
            {
                return 0;
            }

            var total = 0;

            foreach (var otherY in sameColumn.Keys)
            {
                total += CountSquaresAcross(sameColumn, new SquareQuery(pointX, pointY, otherY));
            }

            return total;
        }

        // Every square built from the query point and the same-column point at OtherY:
        // the side length is the gap between the two y-coordinates, which pins the two
        // remaining corners to one x-coordinate on each side of the query. A second
        // corner at the query's own y would be a zero-area square, so it is skipped.
        private int CountSquaresAcross(HashMap<int, int> sameColumn, SquareQuery query)
        {
            if (query.OtherY == query.PointY)
            {
                return 0;
            }

            sameColumn.TryGetValue(query.OtherY, out var otherYCount);
            var side = Math.Abs(query.OtherY - query.PointY);

            return CountCorner(query.PointX + side, query, otherYCount)
                + CountCorner(query.PointX - side, query, otherYCount);
        }

        // The two remaining corners share an x-coordinate, so one map lookup serves
        // both; the square exists once for every combination of the three corners'
        // occurrence counts.
        private int CountCorner(int otherX, SquareQuery query, int otherYCount)
        {
            if (!_countsByX.TryGetValue(otherX, out var countsByY))
            {
                return 0;
            }

            countsByY.TryGetValue(query.PointY, out var countAtQueryY);
            countsByY.TryGetValue(query.OtherY, out var countAtOtherY);

            return countAtQueryY * countAtOtherY * otherYCount;
        }

        // One candidate square family: the queried point, plus the y-coordinate of the
        // second corner sharing its x-coordinate.
        private readonly record struct SquareQuery(int PointX, int PointY, int OtherY);
    }

    private sealed class PointListScanDetectSquares : IDetectSquares
    {
        private readonly List<(int X, int Y)> _points = [];

        public void Add(int pointX, int pointY) => _points.Add((pointX, pointY));

        public int Count(int pointX, int pointY)
        {
            var total = 0;

            foreach (var point in _points)
            {
                if (point.X != pointX || point.Y == pointY)
                {
                    continue;
                }

                var side = Math.Abs(point.Y - pointY);

                total += CountCorner(pointX + side, pointY, point.Y);
                total += CountCorner(pointX - side, pointY, point.Y);
            }

            return total;
        }

        // No occurrence-count multiplier here: the enclosing loop already visits a
        // duplicated same-column corner once per copy.
        private int CountCorner(int otherX, int queryY, int otherY)
        {
            var countAtQueryY = _points.Count(point => point.X == otherX && point.Y == queryY);
            var countAtOtherY = _points.Count(point => point.X == otherX && point.Y == otherY);

            return countAtQueryY * countAtOtherY;
        }
    }
}
