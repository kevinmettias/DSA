namespace DSAExperimentation.LeetCode.Solutions.NumberOfFlowersInFullBloom;

// LC 2251's promise about its input: every flower starts blooming no later than it stops.
// LeetCode's judge never poses an input that breaks it. A flower that stops before it starts counts
// negatively in the sorted bounds and not at all in the scan. Generated cases do not always keep
// the promise, so a harness that runs them asks this first.
internal static class NumberOfFlowersInFullBloomPromise
{
    public static bool IsKeptBy(int[][] flowers, int[] persons) => flowers.All(flower => flower[0] <= flower[1]);
}
