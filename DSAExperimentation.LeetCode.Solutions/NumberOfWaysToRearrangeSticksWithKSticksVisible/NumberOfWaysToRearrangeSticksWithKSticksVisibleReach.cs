namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToRearrangeSticksWithKSticksVisible;

// The reach of NumberOfWaysToRearrangeSticksWithKSticksVisibleSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. RearrangeSticksByPermutationEnumeration
// takes 236.6 ms over the 8 of the catalog's 57 cases with a value of stickCount up to 10 (measured
// 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfWaysToRearrangeSticksWithKSticksVisibleReach
{
    private const int LargestStickCount = 10;

    public static bool IsWithin(int stickCount, int visibleCount) =>
        stickCount <= LargestStickCount;
}
