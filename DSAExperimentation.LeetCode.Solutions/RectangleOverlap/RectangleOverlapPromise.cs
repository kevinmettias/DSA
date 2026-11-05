namespace DSAExperimentation.LeetCode.Solutions.RectangleOverlap;

// LC 836's promise about its input: each of rec1 and rec2 is a valid rectangle with a non-zero area, its
// bottom-left corner below and to the left of its top-right one. LeetCode's judge never poses an input that
// breaks it. A rectangle of no width covers no unit cell, so the unit-grid scan finds no overlap where the
// closed form, comparing the axis intervals, still finds one. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class RectangleOverlapPromise
{
    public static bool IsKeptBy(int[] rec1, int[] rec2) => HasArea(rec1) && HasArea(rec2);

    private static bool HasArea(int[] rectangle) => rectangle[0] < rectangle[2] && rectangle[1] < rectangle[3];
}
