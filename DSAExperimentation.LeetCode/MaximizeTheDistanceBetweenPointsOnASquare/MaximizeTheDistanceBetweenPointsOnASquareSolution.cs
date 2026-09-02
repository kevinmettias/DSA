using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximizeTheDistanceBetweenPointsOnASquare;

// LeetCode 3464. Maximize the Distance Between Points on a Square: choose k of the
// given boundary points so the minimum Manhattan distance between any two chosen
// points is as large as possible.
//
// Every point on the square's boundary maps to a single offset in [0, 4*side)
// walking the perimeter clockwise. k is always at least 4, which pins the answer's
// upper bound at `side` (two of any four boundary points must share a side or
// meet at a corner) - and for any two offsets within `side` of each other, that
// offset difference is exactly their real Manhattan distance, not merely a proxy
// for it. Since the search below never asks about a gap larger than `side`,
// "maximize the minimum pairwise Manhattan distance" reduces to "maximize the
// minimum circular gap" on the sorted offsets: a textbook binary search on the
// answer, trying every point as the unwrapped start of the walk so the closing
// wrap-around gap is checked without ever doubling the array.
internal static class MaximizeTheDistanceBetweenPointsOnASquareSolution
{
    private const int MinimumGap = 1;

    // LeetCode's own shape: raw points, mapped and sorted by
    // ToSortedPerimeterPositions before handing off to the hoisted overload below
    // - the step a benchmark charges to [GlobalSetup] instead of the search itself.
    public static int MaxDistanceByLinearScan(int side, int[][] points, int k) =>
        MaxDistanceByLinearScan(side, ToSortedPerimeterPositions(side, points), k);

    // The textbook arm: the same binary search on the answer, but the inner "find
    // the next point at least `gap` away" step is a plain forward scan - what
    // you'd write without this repo's BinarySearch.
    public static int MaxDistanceByLinearScan(int side, ArraySequence<long> positions, int k) =>
        LargestFeasibleGap(side, gap => IsFeasibleByLinearScan(positions, side, k, gap));

    public static int MaxDistanceBySortedGreedy(int side, int[][] points, int k) =>
        MaxDistanceBySortedGreedy(side, ToSortedPerimeterPositions(side, points), k);

    // The composed arm: the same binary search on the answer, but each hop to
    // "the next point at least `gap` away" is this repo's own
    // BinarySearch.LowerBound over the sorted offsets instead of a linear scan.
    public static int MaxDistanceBySortedGreedy(int side, ArraySequence<long> positions, int k) =>
        LargestFeasibleGap(side, gap => IsFeasibleByBinarySearch(positions, side, k, gap));

    private static int LargestFeasibleGap(int side, Func<int, bool> isFeasible)
    {
        var low = MinimumGap;
        var high = side;

        while (low < high)
        {
            var mid = low + ((high - low + 1) / 2);

            if (isFeasible(mid))
            {
                low = mid;
            }
            else
            {
                high = mid - 1;
            }
        }

        return low;
    }

    private static bool IsFeasibleByLinearScan(ArraySequence<long> positions, int side, int k, int gap)
    {
        var perimeter = 4L * side;
        var n = positions.Length;

        for (var startIndex = 0; startIndex < n; startIndex++)
        {
            var start = positions.Get(startIndex);
            var end = start + perimeter - gap;
            var current = start;
            var selected = 1;

            for (var j = startIndex + 1; j < n && selected < k; j++)
            {
                var candidate = positions.Get(j);

                if (candidate < current + gap)
                {
                    continue;
                }

                if (candidate > end)
                {
                    break;
                }

                current = candidate;
                selected++;
            }

            if (selected >= k)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFeasibleByBinarySearch(ArraySequence<long> positions, int side, int k, int gap)
    {
        var perimeter = 4L * side;
        var n = positions.Length;

        for (var startIndex = 0; startIndex < n; startIndex++)
        {
            var start = positions.Get(startIndex);
            var end = start + perimeter - gap;
            var current = start;
            var selected = 1;

            while (selected < k)
            {
                var nextIndex = BinarySearch.LowerBound(positions, current + gap);

                if (nextIndex >= n || positions.Get(nextIndex) > end)
                {
                    break;
                }

                current = positions.Get(nextIndex);
                selected++;
            }

            if (selected >= k)
            {
                return true;
            }
        }

        return false;
    }

    public static ArraySequence<long> ToSortedPerimeterPositions(int side, int[][] points)
    {
        var positions = new long[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            positions[i] = ToPerimeterPosition(side, points[i][0], points[i][1]);
        }

        Array.Sort(positions);
        return new ArraySequence<long>(positions);
    }

    private static long ToPerimeterPosition(int side, int x, int y)
    {
        if (x == 0)
        {
            return y;
        }

        if (y == side)
        {
            return side + x;
        }

        if (x == side)
        {
            return 3L * side - y;
        }

        return 4L * side - x;
    }
}
