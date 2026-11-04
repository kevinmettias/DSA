namespace DSAExperimentation.LeetCode.Solutions.MaximumAreaRectangleWithPointConstraintsII;

// LC 3382's promise about its input: all the points are unique. LeetCode's judge never poses an
// input that breaks it. A repeated point makes a rectangle of no area, which the sweep answers as
// one and the scan does not. Generated cases do not always keep the promise, so a harness that runs
// them asks this first.
internal static class MaximumAreaRectangleWithPointConstraintsIIPromise
{
    public static bool IsKeptBy(int[] xCoord, int[] yCoord)
        => xCoord.Length == yCoord.Length && xCoord.Zip(yCoord).Distinct().Count() == xCoord.Length;
}
