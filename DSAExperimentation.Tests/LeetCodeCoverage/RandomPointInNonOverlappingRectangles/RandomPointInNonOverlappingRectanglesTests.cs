using DSAExperimentation.LeetCode.RandomPointInNonOverlappingRectangles;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPointInNonOverlappingRectangles;

// Harness only. Both strategies are
// RandomPointInNonOverlappingRectanglesSolution's - this file just pins them to
// LeetCode's published examples: every drawn point must land inside one of the
// given rectangles, and picks must be weighted by rectangle area rather than
// uniform over rectangles.
public sealed partial class RandomPointInNonOverlappingRectanglesTests
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
        AssertEveryPickLandsInsideARectangle(rects, seed, new LinearScanPicker());

    [Theory]
    [MemberData(nameof(Examples))]
    public void PickByBinarySearchUpperBound_LeetCodeExamples_AlwaysLandsInsideSomeRectangle(
        int[][] rects, int seed) =>
        AssertEveryPickLandsInsideARectangle(rects, seed, new BinarySearchPicker());

    [Fact]
    public void PickByLinearScan_OneRectangleFarLargerByArea_LandsThereFarMoreOften() =>
        AssertLargeRectangleDominates(new LinearScanPicker());

    [Fact]
    public void PickByBinarySearchUpperBound_OneRectangleFarLargerByArea_LandsThereFarMoreOften() =>
        AssertLargeRectangleDominates(new BinarySearchPicker());

    private static void AssertEveryPickLandsInsideARectangle(
        int[][] rects, int seed, IRectanglePicker picker)
    {
        var random = new Random(seed);

        for (var i = 0; i < DrawsPerBoundsCheck; i++)
        {
            var point = picker.Pick(rects, random);
            Assert.Contains(rects, rect => IsInside(point, rect));
        }
    }

    private static void AssertLargeRectangleDominates(IRectanglePicker picker)
    {
        int[][] rects = [[0, 0, 0, 0], [0, 0, 100, 100]];
        var random = new Random(3);

        var largeRectangleHits = 0;
        for (var i = 0; i < DrawsPerAreaWeightingCheck; i++)
        {
            var point = picker.Pick(rects, random);
            if (point[0] != 0 || point[1] != 0)
            {
                largeRectangleHits++;
            }
        }

        Assert.True(largeRectangleHits > MinLargeRectangleHitsOutOf500);
    }

    private static bool IsInside(int[] point, int[] rect) =>
        point[0] >= rect[0] && point[0] <= rect[2] && point[1] >= rect[1] && point[1] <= rect[3];

    // One drawing strategy: given the rectangles and a source of randomness, return one
    // point that lies inside one of them. The two shared assertion helpers used to take
    // the strategy as a bare delegate, which named neither the decision nor its inputs;
    // this type states in one named method what a picker is and is the place the contract
    // - always inside a rectangle, weighted by area - is written down. Nested because both
    // it and its two implementations are only ever used inside this test class.
    private interface IRectanglePicker
    {
        int[] Pick(int[][] rects, Random random);
    }

    // Uniform-free prefix-area scan: the arm that reaches for the solution's own name.
    private sealed class LinearScanPicker : IRectanglePicker
    {
        public int[] Pick(int[][] rects, Random random) =>
            RandomPointInNonOverlappingRectanglesSolution.PickByLinearScan(rects, random);
    }

    // The strategy the solution offers as its efficient arm, behind the same name.
    private sealed class BinarySearchPicker : IRectanglePicker
    {
        public int[] Pick(int[][] rects, Random random) =>
            RandomPointInNonOverlappingRectanglesSolution.PickByBinarySearchUpperBound(rects, random);
    }
}
