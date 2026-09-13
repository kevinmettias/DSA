using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.KClosestPointsToOrigin;

// LeetCode 973. K Closest Points to Origin: return the k points nearest the origin,
// in any order. Comparing squared distances avoids the square root entirely, and the
// constraints (|x|, |y| <= 10^4) keep x^2 + y^2 inside an int.
//
// The two strategies differ only in how much of the input they are willing to order:
//
//   FullSort is the textbook answer - measure every point, sort all n of them by
//   distance, take the first k. O(n log n), and it orders n - k points nobody asked
//   about.
//
//   SizeKMaxHeap keeps this repo's own Heap<Element, MaxHeapOrder<Element>> at size k
//   (the KthLargestElement precedent), evicting its farthest root as soon as a
//   (k + 1)-th candidate arrives, so only k points are ever ordered. O(n log k).
internal static class KClosestPointsToOriginSolution
{
    // LeetCode hands each point in as a two-element array.
    private const int X = 0;
    private const int Y = 1;

    // The textbook arm, deliberately all-BCL: pair every point with its squared
    // distance, Array.Sort the whole thing, take the k smallest.
    public static int[][] KClosestByFullSort(int[][] points, int k)
    {
        var byDistance = new (int Distance, int[] Point)[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            byDistance[i] = (SquaredDistance(points[i]), points[i]);
        }

        Array.Sort(byDistance, (a, b) => a.Distance.CompareTo(b.Distance));

        var take = Math.Min(k, points.Length);
        var result = new int[take][];

        for (var i = 0; i < take; i++)
        {
            result[i] = byDistance[i].Point;
        }

        return result;
    }

    // The composed arm: a size-k max-heap over (SquaredDistance, X, Y). ValueTuple
    // already compares lexicographically, so MaxHeapOrder orders by distance first
    // with no comparer plumbing; whenever the heap outgrows k its root is the
    // farthest point held, and that is exactly the one to drop.
    public static int[][] KClosestBySizeKMaxHeap(int[][] points, int k)
    {
        var heap = new Heap<(int Distance, int X, int Y), MaxHeapOrder<(int, int, int)>>();

        foreach (var point in points)
        {
            heap.Push((SquaredDistance(point), point[X], point[Y]));

            if (heap.Count > k)
            {
                heap.TryPop(out _);
            }
        }

        var result = new int[heap.Count][];

        for (var i = 0; i < result.Length; i++)
        {
            heap.TryPop(out var top);
            result[i] = [top.X, top.Y];
        }

        return result;
    }

    private static int SquaredDistance(int[] point) => (point[X] * point[X]) + (point[Y] * point[Y]);
}
