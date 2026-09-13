using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumNumberOfVisiblePoints;

// LeetCode 1610. Maximum Number of Visible Points: standing at `location` with a
// field of view `angle` degrees wide, rotate to see as many points as possible.
//
// Both strategies reduce every point to its polar angle in degrees from `location`.
// Points stacked exactly on `location` have no direction at all - they are visible
// whichever way you face - so they are counted separately and added back at the end.
// What is left is "the widest window of width `angle` on a circle", answered either
// by trying every point's angle as the window's leading edge, or by sorting once and
// sliding.
internal static class MaximumNumberOfVisiblePointsSolution
{
    private const double FullCircleDegrees = 360.0;
    private const double DegreesPerRadian = 180.0;
    private const double AngleEpsilon = 1e-9;
    private const int AngleDoublingFactor = 2;

    // The textbook answer: every point's angle tried as the window's leading edge,
    // each scored by a full pass counting the points within `angle` degrees clockwise
    // of it - O(n^2), BCL-only, the arm the composed solution below has to justify
    // itself against. The window's edge is what anchors: an optimal window can always
    // be rotated until it touches a point, but a window *centered* on a point would
    // be 2*angle wide and would overcount.
    public static int VisiblePointsByPairwiseBruteForce(int[][] points, int angle, int[] location)
    {
        var angles = CollectAngles(points, location, out var atLocation);
        var widest = 0;

        foreach (var anchor in angles)
        {
            widest = Math.Max(widest, CountWithinWindow(angles, anchor, angle));
        }

        return widest + atLocation;
    }

    private static int CountWithinWindow(double[] angles, double anchor, int angle)
    {
        var count = 0;

        foreach (var candidate in angles)
        {
            var clockwise = candidate - anchor;

            if (clockwise < -AngleEpsilon)
            {
                clockwise += FullCircleDegrees;
            }

            if (clockwise <= angle + AngleEpsilon)
            {
                count++;
            }
        }

        return count;
    }

    // Sort the angles with this repo's own Algorithms.Sorting.MergeSort (SortAnArray
    // precedent) over an ArrayIndexedSequence<double>, then duplicate the sorted run
    // shifted by +360 degrees so a single two-pointer sweep can slide a window of
    // width `angle` straight across the 360/0-degree seam instead of special-casing
    // it. O(n log n) sorting plus one O(n) sweep, against the brute force's O(n^2)
    // of per-pair window tests.
    public static int VisiblePointsBySortAndSlideWindow(int[][] points, int angle, int[] location)
    {
        var angles = CollectAngles(points, location, out var atLocation);

        MergeSort.Sort<double, ArrayIndexedSequence<double>>(new ArrayIndexedSequence<double>(angles));

        var doubled = DuplicateWithWrap(angles);
        var widestWindow = WidestWindow(doubled, angle);

        return Math.Min(widestWindow, angles.Length) + atLocation;
    }

    private static double[] DuplicateWithWrap(double[] sorted)
    {
        var doubled = new double[sorted.Length * AngleDoublingFactor];

        for (var i = 0; i < sorted.Length; i++)
        {
            doubled[i] = sorted[i];
            doubled[i + sorted.Length] = sorted[i] + FullCircleDegrees;
        }

        return doubled;
    }

    private static int WidestWindow(double[] doubled, int angle)
    {
        var widestWindow = 0;
        var left = 0;

        for (var right = 0; right < doubled.Length; right++)
        {
            while (doubled[right] - doubled[left] > angle + AngleEpsilon)
            {
                left++;
            }

            widestWindow = Math.Max(widestWindow, right - left + 1);
        }

        return widestWindow;
    }

    // Points sitting exactly on `location` are reported through `atLocation` rather
    // than given an angle: Atan2(0, 0) would answer 0 degrees, which is a direction
    // they do not have.
    private static double[] CollectAngles(int[][] points, int[] location, out int atLocation)
    {
        atLocation = 0;
        var angles = new List<double>(points.Length);

        foreach (var point in points)
        {
            var dx = point[0] - location[0];
            var dy = point[1] - location[1];

            if (dx == 0 && dy == 0)
            {
                atLocation++;
                continue;
            }

            angles.Add(Math.Atan2(dy, dx) * DegreesPerRadian / Math.PI);
        }

        return [.. angles];
    }
}
