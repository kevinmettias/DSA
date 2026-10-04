using DSAExperimentation.Algorithms.Geometry;

namespace DSAExperimentation.LeetCode.LargestTriangleArea;

// LeetCode 812. Largest Triangle Area: the maximum area of any triangle formed by
// three of the given points.
//
// Both strategies answer with that area. They differ only in how large a candidate
// set the cubic triple enumeration runs over: the baseline enumerates every triple
// of every point, the composed strategy first reduces the points to their convex
// hull and enumerates only hull triples. The reduction is sound because the
// maximum-area triangle always has all three vertices on the hull - sliding an
// interior vertex outward along the direction perpendicular to the opposite side
// can only grow the triangle - so no candidate that could win is discarded.
//
// The hull is Algorithms.Geometry.ConvexHull - Andrew's monotone chain over
// MergeSort - the same one ErectTheFenceSolution (LC 587) starts from. This problem
// needs only the strict corners it returns; LC 587 goes on to fold in every point
// collinear-and-between on an edge.
//
// A degenerate all-collinear input has no hull triangle at all - the sweep leaves
// fewer than three corners - so the composed strategy falls back to the original
// points, which all have zero area anyway, rather than enumerating an empty set.
internal static class LargestTriangleAreaSolution
{
    // A triangle needs three vertices; fewer hull corners than this means the
    // input is degenerate (collinear) and there is nothing to reduce to.
    private const int MinHullVerticesForTriangle = 3;

    // The shoelace formula gives twice the triangle's area.
    private const double TriangleAreaDivisor = 2.0;

    // The textbook O(n^3) answer: every triple of points, keep the largest area.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed strategy below has to justify itself against.
    public static double LargestAreaByAllTriples((int X, int Y)[] points) => LargestOver(points);

    // Reduce the candidate set to the convex hull first, then run the same cubic
    // enumeration over just the hull vertices - O(n log n + h^3) instead of O(n^3),
    // and for points drawn from a filled region h is a small fraction of n.
    public static double LargestAreaByConvexHullReduction((int X, int Y)[] points)
    {
        var hull = ConvexHull.Corners(points);

        return LargestOver(hull.Length >= MinHullVerticesForTriangle ? hull : points);
    }

    private static double LargestOver((int X, int Y)[] points)
    {
        var best = 0.0;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                for (var k = j + 1; k < points.Length; k++)
                {
                    var area = Area(points[i], points[j], points[k]);

                    best = Math.Max(best, area);
                }
            }
        }

        return best;
    }

    private static double Area((int X, int Y) firstPoint, (int X, int Y) secondPoint, (int X, int Y) thirdPoint)
    {
        var cross = Cross(firstPoint, secondPoint, thirdPoint);

        return Math.Abs(cross) / TriangleAreaDivisor;
    }

    private static long Cross((int X, int Y) origin, (int X, int Y) firstPoint, (int X, int Y) secondPoint)
        => (long)(firstPoint.X - origin.X) * (secondPoint.Y - origin.Y) - (long)(firstPoint.Y - origin.Y) * (secondPoint.X - origin.X);
}
