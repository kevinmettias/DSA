namespace DSAExperimentation.LeetCode.Solutions.NumberOfVisiblePeopleInAQueue;

// LC 1944's promise about its input: all the heights are distinct. LeetCode's judge never poses an
// input that breaks it. Two people of one height hide each other on one reading and not the other.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class NumberOfVisiblePeopleInAQueuePromise
{
    public static bool IsKeptBy(int[] heights) => heights.Distinct().Count() == heights.Length;
}
