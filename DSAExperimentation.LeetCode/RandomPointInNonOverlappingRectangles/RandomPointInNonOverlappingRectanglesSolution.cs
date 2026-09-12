using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.RandomPointInNonOverlappingRectangles;

// LeetCode 497. Random Point in Non-overlapping Rectangles: build a prefix-sum
// array of rectangle areas, draw one uniform integer over the total area, and use
// it to pick which rectangle's cumulative range the draw landed in - weighted-by-
// area random selection. A second, independent uniform draw then picks a point
// inside that chosen rectangle.
//
// The two strategies differ only in how they turn the draw into a rectangle
// index: a linear scan of the prefix sums, or this repo's own
// BinarySearch.UpperBound over an ArraySequence<int> of the same prefix sums -
// the same prefix-sum-plus-BinarySearch technique
// CountOfSmallerNumbersAfterSelfSolution uses for rank compression. Both take a
// hoisted overload of the prepared prefix-sum array, so a caller that draws many
// points is not charged rebuilding it on every Pick.
internal static class RandomPointInNonOverlappingRectanglesSolution
{
    // The textbook per-pick linear scan: walk the prefix sums until the running
    // total exceeds the draw. O(n) per Pick; the arm the binary search below has
    // to justify itself against.
    public static int[] PickByLinearScan(int[][] rects, Random random) =>
        PickByLinearScan(rects, BuildPrefixAreas(rects), random);

    public static int[] PickByLinearScan(int[][] rects, int[] prefixAreas, Random random)
    {
        var draw = random.Next(prefixAreas[^1]);
        var index = 0;

        while (prefixAreas[index] <= draw)
        {
            index++;
        }

        return PointWithin(rects[index], random);
    }

    // This repo's own BinarySearch.UpperBound over an ArraySequence<int> view of
    // the prefix sums finds the same rectangle index in O(log n) per Pick.
    public static int[] PickByBinarySearchUpperBound(int[][] rects, Random random) =>
        PickByBinarySearchUpperBound(rects, BuildPrefixAreas(rects), random);

    public static int[] PickByBinarySearchUpperBound(int[][] rects, int[] prefixAreas, Random random)
    {
        var sequence = new ArraySequence<int>(prefixAreas);
        var draw = random.Next(prefixAreas[^1]);
        var index = BinarySearch.UpperBound(sequence, draw);

        return PointWithin(rects[index], random);
    }

    private static int[] BuildPrefixAreas(int[][] rects)
    {
        var prefixAreas = new int[rects.Length];
        var running = 0;

        for (var i = 0; i < rects.Length; i++)
        {
            running += Area(rects[i]);
            prefixAreas[i] = running;
        }

        return prefixAreas;
    }

    private static int Area(int[] rect) => (rect[2] - rect[0] + 1) * (rect[3] - rect[1] + 1);

    private static int[] PointWithin(int[] rect, Random random)
    {
        var x = rect[0] + random.Next(rect[2] - rect[0] + 1);
        var y = rect[1] + random.Next(rect[3] - rect[1] + 1);
        return [x, y];
    }
}
