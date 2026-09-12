using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.LeetCode.FallingSquares;

// LeetCode 699. Falling Squares: each square in turn lands on top of whatever is
// already stacked under its [left, left+size) footprint; report the running max
// stack height after each square lands.
internal static class FallingSquaresSolution
{
    private const int CoordinatesPerPosition = 2;

    // The textbook O(n^2) approach: for each square, scan every earlier square and
    // take the max height of the ones whose footprint overlaps it. Deliberately
    // written without this repo's primitives - it is the arm the composed solution
    // below has to justify itself against.
    public static List<int> HeightsByBruteForceOverlapScan(int[][] positions)
    {
        var heights = new int[positions.Length];
        var result = new List<int>(positions.Length);
        var overallMax = 0;

        for (var i = 0; i < positions.Length; i++)
        {
            overallMax = StackSquareAndTrackMax(positions, i, heights, overallMax);
            result.Add(overallMax);
        }

        return result;
    }

    private static int StackSquareAndTrackMax(int[][] positions, int i, int[] heights, int overallMax)
    {
        var left = positions[i][0];
        var right = left + positions[i][1];
        var heightBelow = 0;

        for (var j = 0; j < i; j++)
        {
            var otherLeft = positions[j][0];
            var otherRight = otherLeft + positions[j][1];

            if (left < otherRight && otherLeft < right && heights[j] > heightBelow)
            {
                heightBelow = heights[j];
            }
        }

        heights[i] = heightBelow + positions[i][1];
        return Math.Max(overallMax, heights[i]);
    }

    // Coordinate-compresses every square's footprint onto a dense leaf-index range,
    // then tracks the current stack height over that range with this repo's own
    // LazySegmentTree<int,int?,RangeAssignMaxOperation<int>> - range-max Query
    // answers "what's already stacked directly under this square" and range-assign
    // UpdateRange plants the new height across that same footprint, exactly the
    // algebra RangeAssignMaxOperation.cs already provides (a newer assignment
    // always overwrites - see its own doc comment).
    public static List<int> HeightsByLazySegmentTree(int[][] positions)
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

    // Every square's left/right edge is added to this list before sorting, so the
    // resulting array always contains an exact match for every edge
    // Array.BinarySearch above looks up - no "insertion point" (~index) handling
    // needed.
    private static int[] CompressCoordinates(int[][] positions)
    {
        var coordinates = new List<int>(positions.Length * CoordinatesPerPosition);

        foreach (var position in positions)
        {
            coordinates.Add(position[0]);
            coordinates.Add(position[0] + position[1]);
        }

        return coordinates.Distinct().OrderBy(x => x).ToArray();
    }
}
