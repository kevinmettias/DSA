namespace DSAExperimentation.LeetCode.Solutions.KthSmallestInstructions;

// The reach of KthSmallestInstructionsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. KthSmallestPathByEnumerateAndSort builds and sorts every one of the
// C(row + column, row) instructions, and takes 180.0 ms over the 50 of the 85 cases the promise keeps with at
// most 6,435 of them (measured 2026-11-05, Release); the cases with 11,440 add 103 ms, past the 250 ms a
// strategy's catalog cases are allowed.
internal static class KthSmallestInstructionsReach
{
    private const long LargestPathCount = 6_435;

    public static bool IsWithin(int[] destination, long rank) =>
        KthSmallestInstructionsPromise.PathCountTo(destination) <= LargestPathCount;
}
