namespace DSAExperimentation.LeetCode.MinimumCutsToDivideACircle;

// LeetCode 2481. Minimum Cuts to Divide a Circle: the fewest straight cuts that
// divide a circle into sliceCount equal slices.
//
// Purely arithmetic. A cut through the centre (a diameter) produces two slices at
// once, so an even sliceCount needs exactly sliceCount / 2 of them. An odd
// sliceCount can never pair opposite slices across the centre - among sliceCount
// evenly spaced points around the circle, no point has an antipodal partner also on
// that list unless sliceCount is even - so every cut must be a single radius cut and
// sliceCount of them are needed. sliceCount == 1 is already one slice and needs no
// cut at all.
//
// No repo primitive models this: it is a closed-form parity check with no
// recursive or search structure to compose over, the same "lighter repo-primitive
// fit" shape the other bit/arithmetic-only Easy problems have. Both arms therefore
// live here in full, which is the point - the simulation used to be a benchmark-
// only arm that nothing asserted.
internal static class MinimumCutsToDivideACircleSolution
{
    // A circle that is already one whole slice; no cut divides it further.
    private const int UncutCircle = 1;

    // Slices produced by one cut through the centre; an odd sliceCount cannot use these.
    private const int SlicesPerDiameterCut = 2;

    // Slices produced by one cut from the centre to the edge.
    private const int SlicesPerRadiusCut = 1;

    // The textbook answer: place one cut at a time and count them, taking two
    // slices off the remainder for a diameter cut and one for a radius cut.
    // Deliberately written as the O(n) loop the closed form always reduces to -
    // it is the arm the closed form has to justify itself against.
    public static int NumberOfCutsBySimulation(int sliceCount)
    {
        if (sliceCount == UncutCircle)
        {
            return 0;
        }

        var remaining = sliceCount;
        var remainingIsEven = remaining % SlicesPerDiameterCut == 0;
        var slicesPerCut = remainingIsEven
            ? SlicesPerDiameterCut
            : SlicesPerRadiusCut;
        var cuts = 0;

        while (remaining > 0)
        {
            remaining -= slicesPerCut;
            cuts++;
        }

        return cuts;
    }

    // The same count read straight off sliceCount's parity, in O(1).
    public static int NumberOfCutsByClosedFormParity(int sliceCount)
    {
        if (sliceCount == UncutCircle)
        {
            return 0;
        }

        var sliceCountIsEven = sliceCount % SlicesPerDiameterCut == 0;

        return sliceCountIsEven ? DiameterCutCount(sliceCount) : sliceCount;
    }

    // An even slice count is halved by cutting through the centre.
    private static int DiameterCutCount(int sliceCount) => sliceCount / SlicesPerDiameterCut;
}
