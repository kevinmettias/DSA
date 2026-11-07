using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.Solutions.RandomPointInNonOverlappingRectangles;

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
// CountOfSmallerNumbersAfterSelfSolution uses for rank compression.
//
// The strategies are the CreateBy factories: each takes rects and makes LeetCode's
// Solution object, which builds the prefix sums once and which the catalog harness
// replays call scripts on (ARCHITECTURE 17.13). Each strategy's algorithm - one pick
// from the prepared prefix sums with a given Random - is Pick on the nested class
// named for the strategy (LinearScan, BinarySearchUpperBound): the object's pick()
// calls it and the hand-written benchmark measures it, so each algorithm is written
// once and a caller that draws many points is not charged rebuilding the prefix sums
// on every pick. LeetCode's points are random, so
// RandomPointInNonOverlappingRectanglesAnswerCheck judges them rather than the
// recorded draw.
[LeetCodeQuestion("random-point-in-non-overlapping-rectangles", LeetCodeAnswerOrder.AsWritten, AnswerCheck = typeof(RandomPointInNonOverlappingRectanglesAnswerCheck))]
internal static class RandomPointInNonOverlappingRectanglesSolution
{
    // Fixed, so a replay of one script draws the same points every time.
    private const int Seed = 1;

    // LeetCode's Solution(rects), finding each pick's rectangle by a linear scan.
    public static IRectanglePointPicker CreateByLinearScan(int[][] rects)
    {
        var prefixAreas = BuildPrefixAreas(rects);

        return new LinearScanPicker(rects, prefixAreas, Seed);
    }

    // LeetCode's Solution(rects), finding each pick's rectangle by a binary search.
    public static IRectanglePointPicker CreateByBinarySearchUpperBound(int[][] rects)
    {
        var prefixAreas = BuildPrefixAreas(rects);

        return new BinarySearchUpperBoundPicker(rects, prefixAreas, Seed);
    }

    // Both strategies' second draw: a uniform integer point inside the rectangle the
    // first draw chose.
    private static int[] PointWithin(int[] rect, Random random)
    {
        var x = rect[0] + random.Next(rect[2] - rect[0] + 1);
        var y = rect[1] + random.Next(rect[3] - rect[1] + 1);
        return [x, y];
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

    // LeetCode's Solution class as its call scripts use it: pick(), one integer point
    // inside one of the rectangles it was made from.
    internal interface IRectanglePointPicker
    {
        int[] Pick();
    }

    // The textbook per-pick linear scan: walk the prefix sums until the running
    // total exceeds the draw. O(n) per Pick; the arm the binary search below has
    // to justify itself against.
    internal static class LinearScan
    {
        public static int[] Pick(int[][] rects, int[] prefixAreas, Random random)
        {
            var draw = random.Next(prefixAreas[^1]);
            var index = 0;

            while (prefixAreas[index] <= draw)
            {
                index++;
            }

            return PointWithin(rects[index], random);
        }
    }

    // This repo's own BinarySearch.UpperBound over an ArraySequence<int> view of
    // the prefix sums finds the same rectangle index in O(log n) per Pick.
    internal static class BinarySearchUpperBound
    {
        public static int[] Pick(int[][] rects, int[] prefixAreas, Random random)
        {
            var draw = random.Next(prefixAreas[^1]);
            var index = BinarySearch.UpperBound(prefixAreas, draw);

            return PointWithin(rects[index], random);
        }
    }

    // Holds the prefix sums its factory built once, and draws each pick from them.
    private sealed class LinearScanPicker(int[][] rects, int[] prefixAreas, int seed) : IRectanglePointPicker
    {
        private readonly Random _random = new(seed);

        public int[] Pick() => LinearScan.Pick(rects, prefixAreas, _random);
    }

    // The same object over the binary search.
    private sealed class BinarySearchUpperBoundPicker(int[][] rects, int[] prefixAreas, int seed) : IRectanglePointPicker
    {
        private readonly Random _random = new(seed);

        public int[] Pick() => BinarySearchUpperBound.Pick(rects, prefixAreas, _random);
    }
}
