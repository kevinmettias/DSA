using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfVisiblePoints;

// LeetCode 1610. Maximum Number of Visible Points: convert every point (other than
// ones stacked exactly on location, which are visible from any direction and
// counted separately) to its polar angle in degrees from location, sort those
// angles with this repo's own Algorithms.Sorting.MergeSort (SortAnArrayTests
// precedent) over an ArrayIndexedSequence<double>, then duplicate the sorted run
// shifted by +360 degrees so a single two-pointer sweep can slide a window of
// width `angle` straight across the 360/0-degree seam instead of special-casing
// it. The widest window found, plus the always-visible same-location count, is the
// answer.
public sealed partial class MaximumNumberOfVisiblePointsTests
{
    [Fact]
    public void VisiblePoints_LeetCodeExampleOne_ReturnsThreeVisiblePoints()
    {
        int[][] points = [[2, 1], [2, 2], [3, 3]];

        var visible = VisiblePoints(points, angle: 90, location: [1, 1]);

        Assert.Equal(3, visible);
    }

    [Fact]
    public void VisiblePoints_LeetCodeExampleTwo_CountsThePointAtLocationRegardlessOfAngle()
    {
        int[][] points = [[2, 1], [2, 2], [3, 4], [1, 1]];

        var visible = VisiblePoints(points, angle: 90, location: [1, 1]);

        Assert.Equal(4, visible);
    }

    [Fact]
    public void VisiblePoints_WindowStraddlesTheThreeSixtyDegreeSeam_GroupsAcrossIt()
    {
        // Angle -90 deg (point (0,-1)) and angle 180 deg (point (-1,0)) are only 90
        // degrees apart going the short way around (180 -> 270 == -90), but 270
        // degrees apart in a plain ascending sort - only the +360 duplication finds
        // the window that groups them.
        int[][] points = [[0, -1], [-1, 0]];

        var visible = VisiblePoints(points, angle: 90, location: [0, 0]);

        Assert.Equal(2, visible);
    }

    private static int VisiblePoints(int[][] points, int angle, int[] location)
    {
        var samePoint = 0;
        var angles = new List<double>(points.Length);

        foreach (var point in points)
        {
            var dx = point[0] - location[0];
            var dy = point[1] - location[1];

            if (dx == 0 && dy == 0)
            {
                samePoint++;
                continue;
            }

            angles.Add(Math.Atan2(dy, dx) * 180.0 / Math.PI);
        }

        var sorted = angles.ToArray();
        MergeSort.Sort<double, ArrayIndexedSequence<double>>(new ArrayIndexedSequence<double>(sorted));

        var doubled = new double[sorted.Length * 2];

        for (var i = 0; i < sorted.Length; i++)
        {
            doubled[i] = sorted[i];
            doubled[i + sorted.Length] = sorted[i] + 360.0;
        }

        var widestWindow = 0;
        var left = 0;

        for (var right = 0; right < doubled.Length; right++)
        {
            while (doubled[right] - doubled[left] > angle + 1e-9)
            {
                left++;
            }

            widestWindow = Math.Max(widestWindow, right - left + 1);
        }

        return Math.Min(widestWindow, sorted.Length) + samePoint;
    }
}
