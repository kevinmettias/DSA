using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KClosestPointsToOrigin;

// LeetCode 973. K Closest Points to Origin: an O(n log k) size-k max-heap over
// (SquaredDistance, X, Y) - this repo's own Heap<Element,MaxHeapOrder<Element>>
// (KthLargestElementTests precedent), discarding its farthest root whenever the
// heap grows past k so only the k nearest points survive. ValueTuple<int,int,int>
// already implements IComparable<(int,int,int)> lexicographically, so MaxHeapOrder
// orders correctly by distance first with no extra comparer plumbing needed.
public sealed partial class KClosestPointsToOriginTests
{
    [Fact]
    public void KClosest_ClassicExample_ReturnsTheSingleNearestPoint()
    {
        int[][] points = [[1, 3], [-2, 2]];

        var result = KClosest(points, k: 1);

        Assert.Equal(new HashSet<(int, int)> { (-2, 2) }, ToSet(result));
    }

    [Fact]
    public void KClosest_SecondExample_ReturnsTheTwoNearestPoints()
    {
        int[][] points = [[3, 3], [5, -1], [-2, 4]];

        var result = KClosest(points, k: 2);

        Assert.Equal(new HashSet<(int, int)> { (3, 3), (-2, 4) }, ToSet(result));
    }

    private static HashSet<(int, int)> ToSet(int[][] points) => [.. points.Select(p => (p[0], p[1]))];

    private static int[][] KClosest(int[][] points, int k)
    {
        var heap = new Heap<(int Distance, int X, int Y), MaxHeapOrder<(int, int, int)>>();

        foreach (var point in points)
        {
            var distance = (point[0] * point[0]) + (point[1] * point[1]);
            heap.Push((distance, point[0], point[1]));

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
}
