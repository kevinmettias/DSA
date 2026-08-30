using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPointInNonOverlappingRectangles;

// LeetCode 497. Random Point in Non-overlapping Rectangles: build a prefix-sum
// array of rectangle areas, draw one uniform integer over the total area, and use
// this repo's own BinarySearch.UpperBound over an ArraySequence<int> to find
// which rectangle's cumulative range the draw landed in - weighted-by-area random
// selection in O(log n) per Pick, the same prefix-sum-plus-BinarySearch technique
// CountOfSmallerNumbersAfterSelfTests uses for rank compression. A second,
// independent uniform draw then picks a point inside that chosen rectangle.
public sealed partial class RandomPointInNonOverlappingRectanglesTests
{
    [Fact]
    public void Pick_SingleRectangle_AlwaysLandsInsideItsBounds()
    {
        var solution = new Solution([[1, 1, 3, 3]], seed: 1);

        for (var i = 0; i < 100; i++)
        {
            var point = solution.Pick();
            Assert.InRange(point[0], 1, 3);
            Assert.InRange(point[1], 1, 3);
        }
    }

    [Fact]
    public void Pick_MultipleRectangles_AlwaysLandsInsideSomeRectangle()
    {
        int[][] rects = [[-2, -2, -1, -1], [1, 0, 3, 0]];
        var solution = new Solution(rects, seed: 2);

        for (var i = 0; i < 200; i++)
        {
            var point = solution.Pick();
            var insideFirst = point[0] is >= -2 and <= -1 && point[1] is >= -2 and <= -1;
            var insideSecond = point[0] is >= 1 and <= 3 && point[1] == 0;

            Assert.True(insideFirst || insideSecond);
        }
    }

    [Fact]
    public void Pick_OneRectangleFarLargerByArea_LandsThereFarMoreOften()
    {
        int[][] rects = [[0, 0, 0, 0], [0, 0, 100, 100]];
        var solution = new Solution(rects, seed: 3);

        var largeRectangleHits = 0;
        for (var i = 0; i < 500; i++)
        {
            var point = solution.Pick();
            if (point[0] != 0 || point[1] != 0)
            {
                largeRectangleHits++;
            }
        }

        Assert.True(largeRectangleHits > 480);
    }

    private sealed class Solution
    {
        private readonly int[][] _rects;
        private readonly int[] _prefixAreas;
        private readonly Random _random;

        public Solution(int[][] rects, int seed)
        {
            _rects = rects;
            _random = new Random(seed);
            _prefixAreas = new int[rects.Length];

            var running = 0;
            for (var i = 0; i < rects.Length; i++)
            {
                running += Area(rects[i]);
                _prefixAreas[i] = running;
            }
        }

        public int[] Pick()
        {
            var draw = _random.Next(_prefixAreas[^1]);
            var sequence = new ArraySequence<int>(_prefixAreas);
            var index = BinarySearch.UpperBound(sequence, draw);
            var rect = _rects[index];

            var x = rect[0] + _random.Next(rect[2] - rect[0] + 1);
            var y = rect[1] + _random.Next(rect[3] - rect[1] + 1);
            return [x, y];
        }

        private static int Area(int[] rect) => (rect[2] - rect[0] + 1) * (rect[3] - rect[1] + 1);
    }
}
