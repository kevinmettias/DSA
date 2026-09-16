namespace DSAExperimentation.LeetCode.ValidBoomerang;

// LeetCode 1037. Valid Boomerang: three points form a boomerang exactly when they
// are distinct and not collinear.
//
// No repo container or algorithm primitive applies - there is nothing to compose
// over three fixed (x, y) pairs and one closed-form sign check, the same "lighter
// repo-primitive fit" case ComplexNumberMultiplication and MirrorReflection already
// establish. The two strategies are instead the two ways the check itself is
// commonly written: an inexact floating-point area, and the exact integer cross
// product that makes the epsilon unnecessary.
internal static class ValidBoomerangSolution
{
    private const int ThirdPointIndex = 2;
    private const double SemiPerimeterDivisor = 2.0;
    private const double Epsilon = 1e-9;

    // Heron's formula over Math.Sqrt distances - an inexact approach real enough to
    // be a common first instinct, and one that needs an epsilon tolerance to avoid
    // misclassifying near-collinear points. Deliberately BCL double arithmetic: it
    // is the arm the exact check below has to justify itself against.
    public static bool IsBoomerangByHeronArea(int[][] points)
    {
        var a = Distance(points[0], points[1]);
        var b = Distance(points[1], points[ThirdPointIndex]);
        var c = Distance(points[ThirdPointIndex], points[0]);

        var s = (a + b + c) / SemiPerimeterDivisor;
        var areaSquared = s * (s - a) * (s - b) * (s - c);

        return areaSquared > Epsilon;
    }

    private static double Distance(int[] firstPoint, int[] secondPoint)
    {
        var dx = firstPoint[0] - secondPoint[0];
        var dy = firstPoint[1] - secondPoint[1];

        return Math.Sqrt((dx * dx) + (dy * dy));
    }

    // The exact answer: one 2D cross product over the two edge vectors from the
    // first point. Zero means collinear (which covers duplicated points, whose edge
    // vector is zero); anything else is a boomerang. A single multiply-subtract in
    // long arithmetic, with no floating point and no tolerance to pick.
    public static bool IsBoomerangByCrossProduct(int[][] points)
    {
        var (x1, y1) = (points[0][0], points[0][1]);
        var (x2, y2) = (points[1][0], points[1][1]);
        var (x3, y3) = (points[ThirdPointIndex][0], points[ThirdPointIndex][1]);

        var cross = ((long)(x2 - x1) * (y3 - y1)) - ((long)(x3 - x1) * (y2 - y1));

        return cross != 0;
    }
}
