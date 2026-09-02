using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountNumberOfRectanglesContainingEachPoint;

// LeetCode 2250. Count Number of Rectangles Containing Each Point: group rectangle
// lengths by height in this repo's own HashMap<TKey,TValue>, sort each height's
// length list and the distinct-height list with this repo's own MergeSort, then
// answer each point with two nested BinarySearch.LowerBound calls (first to skip
// straight to the smallest height >= y, then within each surviving height bucket to
// skip straight to the smallest length >= x) - the same LowerBound-over-a-sorted-
// ArraySequence composition QueriesOnNumberOfPointsInsideACircleTests already
// establishes, just applied twice instead of once.
public sealed partial class CountNumberOfRectanglesContainingEachPointTests
{
    [Fact]
    public void CountContaining_ClassicExampleOne_ReturnsExpectedCounts()
    {
        int[][] rectangles = [[1, 2], [2, 3], [2, 5]];
        int[][] points = [[2, 1], [1, 4]];

        var counts = CountContainingRectangles(rectangles, points);

        Assert.Equal([2, 1], counts);
    }

    [Fact]
    public void CountContaining_ClassicExampleTwo_ReturnsExpectedCounts()
    {
        int[][] rectangles = [[1, 1], [2, 2], [3, 3]];
        int[][] points = [[1, 3], [1, 1]];

        var counts = CountContainingRectangles(rectangles, points);

        Assert.Equal([1, 3], counts);
    }

    private static int[] CountContainingRectangles(int[][] rectangles, int[][] points)
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

    private static (int[] Heights, HashMap<int, int[]> LengthsByHeight) GroupSortedLengthsByHeight(int[][] rectangles)
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

        return (heights, lengthsByHeight);
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
