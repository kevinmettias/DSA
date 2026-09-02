namespace DSAExperimentation.LeetCode.AnglesOfATriangle;

// LeetCode 3899. Angles of a Triangle: given three side lengths, return the
// triangle's three internal angles (degrees, non-decreasing) via the law of
// cosines, or an empty array when no positive-area triangle has those side
// lengths (the strict triangle inequality fails).
//
// Both strategies share the same triangle-inequality check and law-of-cosines
// formula; they differ only in how the third angle is obtained - a direct
// Acos of its own, or the cheaper 180-minus-the-other-two derivation every
// triangle's angle sum already guarantees, saving one transcendental call.
internal static class AnglesOfATriangleSolution
{
    private const double DegreesPerRadian = 180.0 / Math.PI;

    // The textbook answer: every angle from its own Acos of the law of cosines.
    public static double[] InternalAnglesByLawOfCosines(int[] sides)
    {
        if (!IsValidTriangle(sides, out var a, out var b, out var c))
        {
            return [];
        }

        var angleA = AngleOpposite(a, b, c);
        var angleB = AngleOpposite(b, a, c);
        var angleC = AngleOpposite(c, a, b);

        return Sort(angleA, angleB, angleC);
    }

    // Composed: only two Acos calls - the third angle is derived from the
    // triangle's angle sum (always exactly 180 degrees) instead of a third
    // transcendental call.
    public static double[] InternalAnglesByAngleSum(int[] sides)
    {
        if (!IsValidTriangle(sides, out var a, out var b, out var c))
        {
            return [];
        }

        var angleA = AngleOpposite(a, b, c);
        var angleB = AngleOpposite(b, a, c);
        var angleC = 180.0 - angleA - angleB;

        return Sort(angleA, angleB, angleC);
    }

    private static bool IsValidTriangle(int[] sides, out double a, out double b, out double c)
    {
        (a, b, c) = (sides[0], sides[1], sides[2]);

        var sorted = new[] { a, b, c };
        Array.Sort(sorted);

        return sorted[0] + sorted[1] > sorted[2];
    }

    // Law of cosines: the angle opposite "opposite", given the two sides
    // adjacent to it.
    private static double AngleOpposite(double opposite, double adjacentOne, double adjacentTwo)
    {
        var cosine = ((adjacentOne * adjacentOne) + (adjacentTwo * adjacentTwo) - (opposite * opposite))
            / (2 * adjacentOne * adjacentTwo);

        return Math.Acos(cosine) * DegreesPerRadian;
    }

    private static double[] Sort(double first, double second, double third)
    {
        var angles = new[] { first, second, third };
        Array.Sort(angles);
        return angles;
    }
}
