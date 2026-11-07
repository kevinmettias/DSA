using DSAExperimentation.LeetCode.Solutions.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RandomPointInNonOverlappingRectangles;

// Harness only. Both strategies are
// RandomPointInNonOverlappingRectanglesSolution's, each held here as LeetCode's
// Solution object its CreateBy factory makes from rects - the object whose pick()
// calls the strategy's one-pick algorithm, Pick on the nested class named for the
// strategy - and pinned to LeetCode's published example and to
// added rectangle sets: every drawn point must land inside one of the given
// rectangles, and picks must be weighted by rectangle area rather than uniform over
// rectangles.
public sealed partial class RandomPointInNonOverlappingRectanglesSolutionTests
{
    private const int DrawsPerBoundsCheck = 100;
    private const int DrawsPerAreaWeightingCheck = 500;
    private const int MinLargeRectangleHitsOutOf500 = 480;

    // LeetCode's example 1: Solution(rects) followed by five pick() calls, published as
    // [1, -2], [1, -1], [-1, -2], [-2, -2], [0, 0]. Any integer point inside a rectangle
    // is a correct pick, so those five are one valid draw rather than the draw: a row
    // asserts that each published point passes the same inside-a-rectangle check the
    // strategy's own picks must pass, one pick() per published point.
    public static TheoryData<int[][], int[][]> PublishedExamples =>
        new()
        {
            { [[-2, -2, 1, 1], [2, 2, 4, 6]], [[1, -2], [1, -1], [-1, -2], [-2, -2], [0, 0]] },
        };

    public static TheoryData<int[][]> Examples =>
        new()
        {
            { [[1, 1, 3, 3]] },
            { [[-2, -2, -1, -1], [1, 0, 3, 0]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByLinearScan_LeetCodeExamples_AlwaysLandsInsideSomeRectangle(int[][] rects)
    {
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByLinearScan(rects);

        AssertEveryPickLandsInsideARectangle(rects, picker);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByBinarySearchUpperBound_LeetCodeExamples_AlwaysLandsInsideSomeRectangle(int[][] rects)
    {
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByBinarySearchUpperBound(rects);

        AssertEveryPickLandsInsideARectangle(rects, picker);
    }

    [Theory]
    [MemberData(nameof(PublishedExamples))]
    public void CreateByLinearScan_PublishedExamples_EachPickLandsInsideARectangleLikeThePublishedPicks(
        int[][] rects, int[][] publishedPicks)
    {
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByLinearScan(rects);

        AssertEachScriptedPickLandsInsideARectangle(rects, publishedPicks, picker);
    }

    [Theory]
    [MemberData(nameof(PublishedExamples))]
    public void CreateByBinarySearchUpperBound_PublishedExamples_EachPickLandsInsideARectangleLikeThePublishedPicks(
        int[][] rects, int[][] publishedPicks)
    {
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByBinarySearchUpperBound(rects);

        AssertEachScriptedPickLandsInsideARectangle(rects, publishedPicks, picker);
    }

    [Fact]
    public void CreateByLinearScan_OneRectangleFarLargerByArea_LandsThereFarMoreOften()
    {
        int[][] rects = [[0, 0, 0, 0], [0, 0, 100, 100]];
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByLinearScan(rects);

        AssertLargeRectangleDominates(picker);
    }

    [Fact]
    public void CreateByBinarySearchUpperBound_OneRectangleFarLargerByArea_LandsThereFarMoreOften()
    {
        int[][] rects = [[0, 0, 0, 0], [0, 0, 100, 100]];
        var picker = RandomPointInNonOverlappingRectanglesSolution.CreateByBinarySearchUpperBound(rects);

        AssertLargeRectangleDominates(picker);
    }

    private static void AssertEveryPickLandsInsideARectangle(
        int[][] rects, RandomPointInNonOverlappingRectanglesSolution.IRectanglePointPicker picker)
    {
        for (var i = 0; i < DrawsPerBoundsCheck; i++)
        {
            var point = picker.Pick();
            Assert.Contains(rects, rect => IsInside(point, rect));
        }
    }

    // One pick per pick() call in the published script, each checked the way the
    // published point for that call is checked.
    private static void AssertEachScriptedPickLandsInsideARectangle(
        int[][] rects, int[][] publishedPicks, RandomPointInNonOverlappingRectanglesSolution.IRectanglePointPicker picker)
    {
        foreach (var publishedPick in publishedPicks)
        {
            Assert.Contains(rects, rect => IsInside(publishedPick, rect));

            var point = picker.Pick();
            Assert.Contains(rects, rect => IsInside(point, rect));
        }
    }

    // The picker was made from a single-point rectangle at the origin and a 101 x 101
    // one from it, so about two picks in 10,202 land on the origin.
    private static void AssertLargeRectangleDominates(RandomPointInNonOverlappingRectanglesSolution.IRectanglePointPicker picker)
    {
        var largeRectangleHits = 0;
        for (var i = 0; i < DrawsPerAreaWeightingCheck; i++)
        {
            var point = picker.Pick();
            if (point[0] != 0 || point[1] != 0)
            {
                largeRectangleHits++;
            }
        }

        Assert.True(largeRectangleHits > MinLargeRectangleHitsOutOf500);
    }

    private static bool IsInside(int[] point, int[] rect)
    {
        var isWithinColumns = point[0] >= rect[0] && point[0] <= rect[2];
        var isWithinRows = point[1] >= rect[1] && point[1] <= rect[3];

        return isWithinColumns && isWithinRows;
    }
}
