namespace DSAExperimentation.LeetCode.Solutions.RemoveBoxes;

// The reach of RemoveBoxesSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxPointsByUnmemoizedRecursion takes 156.3 ms over the 40 of the catalog's
// 122 cases with a length of boxes up to 17 (measured 2026-11-05, Release); the larger cases run past the 250
// ms a strategy's catalog cases are allowed.
internal static class RemoveBoxesReach
{
    private const int LargestBoxesLength = 17;

    public static bool IsWithin(int[] boxes) =>
        boxes.Length <= LargestBoxesLength;
}
