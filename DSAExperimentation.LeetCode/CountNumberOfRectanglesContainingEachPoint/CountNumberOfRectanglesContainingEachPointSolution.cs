using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.CountNumberOfRectanglesContainingEachPoint;

// LeetCode 2250. Count Number of Rectangles Containing Each Point: every rectangle
// [l, h] covers the axis-aligned box from (0, 0) to (l, h), so a point (x, y) is
// inside it exactly when l >= x and h >= y. The answer is one count per point.
//
// Both strategies return that same int[]; they differ only in how a point's count
// is obtained - checking it against every rectangle, or exploiting the problem's
// tiny height range by grouping lengths per height and bisecting twice.
internal static class CountNumberOfRectanglesContainingEachPointSolution
{
    // The textbook O(n*m) answer: for every point, walk every rectangle and tally
    // the ones that cover it. Deliberately plain BCL - it is the arm the composed
    // strategy below has to justify itself against, and at small rectangle counts
    // it wins, because it pays none of that strategy's fixed setup cost.
    public static int[] CountRectanglesByBruteForce(int[][] rectangles, int[][] points)
    {
        var counts = new int[points.Length];

        for (var i = 0; i < points.Length; i++)
        {
            counts[i] = CountCoveringRectangles(points[i], rectangles);
        }

        return counts;
    }

    private static int CountCoveringRectangles(int[] point, int[][] rectangles)
    {
        var x = point[0];
        var y = point[1];
        var count = 0;

        foreach (var rectangle in rectangles)
        {
            if (rectangle[0] >= x && rectangle[1] >= y)
            {
                count++;
            }
        }

        return count;
    }

    // Group rectangle lengths by height in this repo's own HashMap<TKey,TValue>,
    // sort each height's length list and the distinct-height list with this repo's
    // own MergeSort, then answer each point with two nested BinarySearch.LowerBound
    // calls - first to skip straight to the smallest height >= y, then within each
    // surviving height bucket to skip straight to the smallest length >= x, whose
    // count-from-there is Length - index. This is the same LowerBound-over-a-sorted
    // -ArraySequence composition QueriesOnNumberOfPointsInsideACircle establishes,
    // nested one level deeper.
    //
    // The grouping and sorting are charged to this method on purpose: they are a
    // fixed cost brute force never pays, and the crossover between the two arms is
    // exactly what the benchmark is there to show.
    public static int[] CountRectanglesByGroupedLowerBound(int[][] rectangles, int[][] points)
    {
        var (heights, lengthsByHeight) = GroupSortedLengthsByHeight(rectangles);
        var heightSequence = new ArraySequence<int>(heights);

        var counts = new int[points.Length];
        for (var i = 0; i < points.Length; i++)
        {
            counts[i] = CountForPoint(points[i], heights, heightSequence, lengthsByHeight);
        }

        return counts;
    }

    private static LengthsGroupedByHeight GroupSortedLengthsByHeight(int[][] rectangles)
    {
        var grouped = new HashMap<int, List<int>>();

        foreach (var rectangle in rectangles)
        {
            AddRectangleToGroup(rectangle, grouped);
        }

        var lengthsByHeight = new HashMap<int, int[]>();
        foreach (var height in grouped.Keys)
        {
            grouped.TryGetValue(height, out var lengths);
            var array = lengths.ToArray();
            MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(array));
            lengthsByHeight.Set(height, array);
        }

        var heights = lengthsByHeight.Keys.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(heights));

        return new(heights, lengthsByHeight);
    }

    private static void AddRectangleToGroup(int[] rectangle, HashMap<int, List<int>> grouped)
    {
        var length = rectangle[0];
        var height = rectangle[1];

        if (!grouped.TryGetValue(height, out var lengths))
        {
            lengths = [];
            grouped.Set(height, lengths);
        }

        lengths.Add(length);
    }

    private static int CountForPoint(
        int[] point, int[] heights, ArraySequence<int> heightSequence, HashMap<int, int[]> lengthsByHeight)
    {
        var x = point[0];
        var y = point[1];
        var startIndex = BinarySearch.LowerBound(heightSequence, y);
        var count = 0;

        for (var i = startIndex; i < heights.Length; i++)
        {
            lengthsByHeight.TryGetValue(heights[i], out var lengths);
            var lengthSequence = new ArraySequence<int>(lengths);
            var firstAtLeastX = BinarySearch.LowerBound(lengthSequence, x);
            count += lengths.Length - firstAtLeastX;
        }

        return count;
    }
}
