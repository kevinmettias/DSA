namespace DSAExperimentation.LeetCode.CircleAndRectangleOverlapping;

// LeetCode 1401. Circle and Rectangle Overlapping: report whether the circle of the
// given radius centred at (xCenter, yCenter) shares any point with the axis-aligned
// rectangle whose opposite corners are (x1, y1) and (x2, y2).
//
// Both strategies answer the same yes/no question and differ only in how much of the
// rectangle they have to look at: one point, found by clamping, or every lattice point
// in it. No repo container or algorithm primitive applies to a handful of coordinate
// comparisons - the same "lighter repo-primitive fit" case RectangleOverlap (LC 836)
// and RectangleArea (LC 223) already document.
internal static class CircleAndRectangleOverlappingSolution
{
    // The textbook answer: an O(width * height) sweep that tests every integer lattice
    // point of the rectangle for circle membership, recording each verdict in a plain
    // BCL buffer and scanning it afterwards, so the full rectangle is always visited.
    // Deliberately written without this repo's primitives - it is the arm the closed
    // form below has to justify itself against.
    //
    // Note this only samples the rectangle at integer coordinates, so it answers the
    // continuous question only where the two agree; it is a measurement baseline, and
    // the examples it is asserted against are ones where they do.
    public static bool HasOverlapByLatticePointScan(Circle circle, Rectangle rectangle)
    {
        var withinCircle = new List<bool>();

        for (var y = rectangle.Y1; y <= rectangle.Y2; y++)
        {
            for (var x = rectangle.X1; x <= rectangle.X2; x++)
            {
                var isWithin = IsWithinCircle(x, y, circle);
                withinCircle.Add(isWithin);
            }
        }

        foreach (var inside in withinCircle)
        {
            if (inside)
            {
                return true;
            }
        }

        return false;
    }

    // O(1): clamping the centre to the rectangle's bounds yields the rectangle point
    // nearest the centre, so the circle overlaps exactly when that point is within the
    // radius. Squared distances keep the comparison in integer arithmetic.
    public static bool HasOverlapByClampedDistance(Circle circle, Rectangle rectangle)
    {
        var closestX = Math.Clamp(circle.X, rectangle.X1, rectangle.X2);
        var closestY = Math.Clamp(circle.Y, rectangle.Y1, rectangle.Y2);

        return IsWithinCircle(closestX, closestY, circle);
    }

    // Shared by both strategies so the only thing they differ in is which points they
    // ask about. Widened to long because the coordinate range squares past int.
    private static bool IsWithinCircle(int pointX, int pointY, Circle circle)
    {
        var dx = (long)(pointX - circle.X);
        var dy = (long)(pointY - circle.Y);

        return (dx * dx) + (dy * dy) <= (long)circle.Radius * circle.Radius;
    }
}
