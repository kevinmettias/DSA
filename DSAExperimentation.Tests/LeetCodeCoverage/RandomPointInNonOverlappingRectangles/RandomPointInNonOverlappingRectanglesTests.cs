using DSAExperimentation.LeetCode.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPointInNonOverlappingRectangles;

// Harness only. Both strategies are
// RandomPointInNonOverlappingRectanglesSolution's - this file just pins them to
// LeetCode's published examples: every drawn point must land inside one of the
// given rectangles, and picks must be weighted by rectangle area rather than
// uniform over rectangles.
public sealed class RandomPointInNonOverlappingRectanglesTests
{
    private const int DrawsPerBoundsCheck = 100;
    private const int DrawsPerAreaWeightingCheck = 500;
    private const int MinLargeRectangleHitsOutOf500 = 480;

    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1, 3, 3]], 1 },
            { [[-2, -2, -1, -1], [1, 0, 3, 0]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PickByLinearScan_LeetCodeExamples_AlwaysLandsInsideSomeRectangle(int[][] rects, int seed) =>
        AssertEveryPickLandsInsideARectangle(
            rects, seed, RandomPointInNonOverlappingRectanglesSolution.PickByLinearScan);

    [Theory]
    [MemberData(nameof(Examples))]
    public void PickByBinarySearchUpperBound_LeetCodeExamples_AlwaysLandsInsideSomeRectangle(
        int[][] rects, int seed) =>
        AssertEveryPickLandsInsideARectangle(
            rects, seed, RandomPointInNonOverlappingRectanglesSolution.PickByBinarySearchUpperBound);

    [Fact]
    public void PickByLinearScan_OneRectangleFarLargerByArea_LandsThereFarMoreOften() =>
        AssertLargeRectangleDominates(RandomPointInNonOverlappingRectanglesSolution.PickByLinearScan);

    [Fact]
    public void PickByBinarySearchUpperBound_OneRectangleFarLargerByArea_LandsThereFarMoreOften() =>
        AssertLargeRectangleDominates(RandomPointInNonOverlappingRectanglesSolution.PickByBinarySearchUpperBound);

    private static void AssertEveryPickLandsInsideARectangle(
        int[][] rects, int seed, Func<int[][], Random, int[]> pick)
    {
        var random = new Random(seed);

        for (var i = 0; i < DrawsPerBoundsCheck; i++)
        {
            var point = pick(rects, random);
            Assert.Contains(rects, rect => IsInside(point, rect));
        }
    }

    private static void AssertLargeRectangleDominates(Func<int[][], Random, int[]> pick)
    {
        int[][] rects = [[0, 0, 0, 0], [0, 0, 100, 100]];
        var random = new Random(3);

        var largeRectangleHits = 0;
        for (var i = 0; i < DrawsPerAreaWeightingCheck; i++)
        {
            var point = pick(rects, random);
            if (point[0] != 0 || point[1] != 0)
            {
                largeRectangleHits++;
            }
        }

        Assert.True(largeRectangleHits > MinLargeRectangleHitsOutOf500);
    }

    private static bool IsInside(int[] point, int[] rect) =>
        point[0] >= rect[0] && point[0] <= rect[2] && point[1] >= rect[1] && point[1] <= rect[3];
}
