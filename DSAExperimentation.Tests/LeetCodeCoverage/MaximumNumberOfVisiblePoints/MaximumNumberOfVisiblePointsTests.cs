using DSAExperimentation.LeetCode.MaximumNumberOfVisiblePoints;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfVisiblePoints;

// Harness only. Both strategies are MaximumNumberOfVisiblePointsSolution's - this
// file pins them to LeetCode's three published examples plus the three shapes a
// windowing bug hides in: a window that straddles the 360/0-degree seam, a field of
// view narrow enough that a window centered on a point would overcount, and points
// standing exactly on `location`.
public sealed class MaximumNumberOfVisiblePointsTests
{
    public static TheoryData<int[][], int, int[], int> Examples =>
        new()
        {
            { [[2, 1], [2, 2], [3, 3]], 90, [1, 1], 3 },
            { [[2, 1], [2, 2], [3, 4], [1, 1]], 90, [1, 1], 4 },
            { [[1, 0], [2, 1]], 13, [1, 1], 1 },
            // Angle -90 deg (point (0,-1)) and angle 180 deg (point (-1,0)) are only
            // 90 degrees apart going the short way around (180 -> 270 == -90), but
            // 270 degrees apart in a plain ascending sort - only wrapping finds the
            // window that groups them.
            { [[0, -1], [-1, 0]], 90, [0, 0], 2 },
            // Angles -45, 0 and 45 with a 60-degree field of view: no window of width
            // 60 holds all three, but one *centered* on 0 degrees would span 120 and
            // wrongly report 3.
            { [[1, 0], [1, 1], [1, -1]], 60, [0, 0], 2 },
            // Every point stands on `location`, so all are visible facing any
            // direction and no angle exists to window at all.
            { [[1, 1], [1, 1], [1, 1]], 0, [1, 1], 3 },
            // A zero-degree field of view still sees every point sharing one exact
            // bearing.
            { [[1, 0], [2, 0], [3, 0]], 0, [0, 0], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void VisiblePointsByPairwiseBruteForce_LeetCodeExamples_ReturnsMaximumVisibleCount(
        int[][] points, int angle, int[] location, int expected)
    {
        var actual = MaximumNumberOfVisiblePointsSolution.VisiblePointsByPairwiseBruteForce(points, angle, location);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void VisiblePointsBySortAndSlideWindow_LeetCodeExamples_ReturnsMaximumVisibleCount(
        int[][] points, int angle, int[] location, int expected)
    {
        var actual = MaximumNumberOfVisiblePointsSolution.VisiblePointsBySortAndSlideWindow(points, angle, location);

        Assert.Equal(expected, actual);
    }
}
