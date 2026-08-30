using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FallingSquares;

// LeetCode 699. Falling Squares: coordinate-compresses every square's [left, left+size)
// footprint onto a dense leaf-index range, then tracks the current stack height over that
// range with this repo's own LazySegmentTree<int,int?,RangeAssignMaxOperation<int>> -
// range-max Query answers "what's already stacked directly under this square" and
// range-assign UpdateRange plants the new height across that same footprint, exactly the
// algebra RangeAssignMaxOperation.cs already provides (a newer assignment always overwrites
// - see its own doc comment). The per-step answer LeetCode expects is just the running max
// of every square's landed height so far.
public sealed partial class FallingSquaresTests
{
    [Fact]
    public void FallingSquares_ClassicOverlappingExample_ReturnsRunningMaxHeights()
    {
        int[][] positions = [[1, 2], [2, 3], [6, 1]];

        Assert.Equal([2, 5, 5], FallingSquares(positions));
    }

    [Fact]
    public void FallingSquares_IdenticalFootprint_StacksDirectlyOnTopOfEachOther()
    {
        int[][] positions = [[100, 100], [100, 100]];

        Assert.Equal([100, 200], FallingSquares(positions));
    }

    [Fact]
    public void FallingSquares_NonOverlappingSquares_EachRestsOnTheGround()
    {
        int[][] positions = [[1, 2], [5, 3], [10, 1]];

        Assert.Equal([2, 3, 3], FallingSquares(positions));
    }

    private static List<int> FallingSquares(int[][] positions)
    {
        var coordinates = CompressCoordinates(positions);
        var tree = new LazySegmentTree<int, int?, RangeAssignMaxOperation<int>>(new int[coordinates.Length - 1]);

        var result = new List<int>(positions.Length);
        var overallMax = 0;

        foreach (var position in positions)
        {
            var left = Array.BinarySearch(coordinates, position[0]);
            var right = Array.BinarySearch(coordinates, position[0] + position[1]) - 1;

            var heightBelow = tree.Query(left, right);
            var newHeight = heightBelow + position[1];

            tree.UpdateRange(left, right, newHeight);
            overallMax = Math.Max(overallMax, newHeight);
            result.Add(overallMax);
        }

        return result;
    }

    // Every square's left/right edge is added to this list before sorting, so the resulting
    // array always contains an exact match for every edge Array.BinarySearch below looks up -
    // no "insertion point" (~index) handling needed.
    private static int[] CompressCoordinates(int[][] positions)
    {
        var coordinates = new List<int>(positions.Length * 2);

        foreach (var position in positions)
        {
            coordinates.Add(position[0]);
            coordinates.Add(position[0] + position[1]);
        }

        return coordinates.Distinct().OrderBy(x => x).ToArray();
    }
}
