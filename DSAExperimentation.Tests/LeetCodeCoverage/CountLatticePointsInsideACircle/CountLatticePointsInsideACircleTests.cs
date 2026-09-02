using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountLatticePointsInsideACircle;

// LeetCode 2249. Count Lattice Points Inside a Circle: scan each circle's own
// bounding box for integer points inside it, and de-duplicate the union across
// circles with this repo's own Set<Element> (HashMap<Element,bool>-backed) instead
// of a hand-rolled visited array - the same "membership is the whole job" fit
// TwoSumTests' HashMap already establishes, just for a tuple key instead of int.
public sealed partial class CountLatticePointsInsideACircleTests
{
    [Fact]
    public void CountLatticePoints_SingleUnitCircle_ReturnsPlusShapedFivePoints()
    {
        int[][] circles = [[2, 2, 1]];

        var count = CountLatticePoints(circles);

        Assert.Equal(5, count);
    }

    [Fact]
    public void CountLatticePoints_TwoOverlappingCircles_DoesNotDoubleCountSharedPoints()
    {
        // Circle A (center (0,0), r=1) and circle B (center (1,0), r=1) each cover 5
        // lattice points and share exactly (0,0) and (1,0) - the union is 8, not the
        // naive 10, only if the two shared points are de-duplicated.
        int[][] circles = [[0, 0, 1], [1, 0, 1]];

        var count = CountLatticePoints(circles);

        Assert.Equal(8, count);
    }

    private static int CountLatticePoints(int[][] circles)
    {
        var points = new Set<(int X, int Y)>();

        foreach (var circle in circles)
        {
            AddCirclePoints(circle, points);
        }

        return points.Count;
    }

    private static void AddCirclePoints(int[] circle, Set<(int X, int Y)> points)
    {
        var x = circle[0];
        var y = circle[1];
        var r = circle[2];

        for (var dx = -r; dx <= r; dx++)
        {
            for (var dy = -r; dy <= r; dy++)
            {
                if ((dx * dx) + (dy * dy) <= r * r)
                {
                    points.TryAdd((x + dx, y + dy));
                }
            }
        }
    }
}
