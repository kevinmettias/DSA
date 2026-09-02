using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimizeManhattanDistances;

// LeetCode 3102. Minimize Manhattan Distances: remove exactly one point from
// `points` so that the maximum Manhattan distance between any two of the
// remaining points is as small as possible; return that minimized maximum.
//
// The composed strategy leans on the standard 45-degree rotation identity for
// Manhattan distance: with u = x + y and v = x - y,
// max_{i,j} |x_i-x_j| + |y_i-y_j| = max(max(u)-min(u), max(v)-min(v)). Removing one
// point only ever changes an extreme if that point WAS the (a) unique extreme, so
// sorting each of u and v once up front and, per removal, falling back from the
// top/bottom entry to the runner-up when the removed point owns it, answers every
// removal in O(1) off an O(n log n) preprocessing pass.
internal static class MinimizeManhattanDistancesSolution
{
    // The textbook answer: for each point removed, rescan every remaining pair for
    // the true maximum distance. O(n^3). Deliberately plain BCL loops - the arm the
    // transform strategy below has to justify itself against.
    public static int MinDistanceByBruteForce(int[][] points)
    {
        var best = int.MaxValue;

        for (var removed = 0; removed < points.Length; removed++)
        {
            var maxDistance = 0;

            for (var i = 0; i < points.Length; i++)
            {
                if (i == removed)
                {
                    continue;
                }

                for (var j = i + 1; j < points.Length; j++)
                {
                    if (j == removed)
                    {
                        continue;
                    }

                    maxDistance = Math.Max(maxDistance, ManhattanDistance(points[i], points[j]));
                }
            }

            best = Math.Min(best, maxDistance);
        }

        return best;
    }

    private static int ManhattanDistance(int[] a, int[] b) => Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]);

    // Sorts both transforms with this repo's own MergeSort (over an
    // ArrayIndexedSequence view of the transform arrays), keeping the O(n) "try
    // every removal" sweep as the only thing the benchmark measures once handed the
    // prepared arrays.
    public static int MinDistanceByManhattanTransform(int[][] points)
    {
        var (sortedByU, sortedByV) = BuildSortedTransforms(points);

        return MinDistanceByManhattanTransform(sortedByU, sortedByV);
    }

    public static int MinDistanceByManhattanTransform(
        (long Value, int PointIndex)[] sortedByU, (long Value, int PointIndex)[] sortedByV)
    {
        var best = long.MaxValue;

        for (var removed = 0; removed < sortedByU.Length; removed++)
        {
            var rangeU = RangeExcluding(sortedByU, removed);
            var rangeV = RangeExcluding(sortedByV, removed);
            best = Math.Min(best, Math.Max(rangeU, rangeV));
        }

        return (int)best;
    }

    // sortedAscending always holds every point (n >= 3), so falling back from the
    // global extreme to its runner-up whenever the removed point owns it always has
    // a valid, distinct point to land on.
    private static long RangeExcluding((long Value, int PointIndex)[] sortedAscending, int excludedPointIndex)
    {
        var last = sortedAscending.Length - 1;
        var max = sortedAscending[last].PointIndex != excludedPointIndex
            ? sortedAscending[last].Value
            : sortedAscending[last - 1].Value;
        var min = sortedAscending[0].PointIndex != excludedPointIndex
            ? sortedAscending[0].Value
            : sortedAscending[1].Value;

        return max - min;
    }

    public static ((long Value, int PointIndex)[] SortedByU, (long Value, int PointIndex)[] SortedByV)
        BuildSortedTransforms(int[][] points)
    {
        var byU = new (long Value, int PointIndex)[points.Length];
        var byV = new (long Value, int PointIndex)[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            byU[i] = ((long)points[i][0] + points[i][1], i);
            byV[i] = ((long)points[i][0] - points[i][1], i);
        }

        var byValue = Comparer<(long Value, int PointIndex)>.Create((a, b) => a.Value.CompareTo(b.Value));

        MergeSort.Sort<(long Value, int PointIndex), ArrayIndexedSequence<(long Value, int PointIndex)>>(
            new ArrayIndexedSequence<(long Value, int PointIndex)>(byU), byValue);
        MergeSort.Sort<(long Value, int PointIndex), ArrayIndexedSequence<(long Value, int PointIndex)>>(
            new ArrayIndexedSequence<(long Value, int PointIndex)>(byV), byValue);

        return (byU, byV);
    }
}
