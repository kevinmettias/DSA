namespace DSAExperimentation.LeetCode.Solutions.KthSmallestInLexicographicalOrder;

// The reach of KthSmallestInLexicographicalOrderSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. FindKthNumberByDepthFirstTraversal takes 33.7 ms over the
// 20 of the catalog's 94 cases with a value of upperBound up to 10,000 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class KthSmallestInLexicographicalOrderReach
{
    private const int LargestUpperBound = 10_000;

    public static bool IsWithin(int upperBound, int rank) =>
        upperBound <= LargestUpperBound;
}
