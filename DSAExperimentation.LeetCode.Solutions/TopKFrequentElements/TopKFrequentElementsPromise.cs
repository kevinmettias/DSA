namespace DSAExperimentation.LeetCode.Solutions.TopKFrequentElements;

// LC 347's promise about its input: the answer is unique, so the kth most frequent element is
// strictly more frequent than the next. LeetCode's judge never poses an input that breaks it. Given
// a tie at the kth frequency, the sort and the heap keep different elements. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class TopKFrequentElementsPromise
{
    public static bool IsKeptBy(int[] nums, int topCount)
    {
        var frequencies = nums.GroupBy(value => value).Select(group => group.Count()).OrderDescending().ToArray();

        return topCount >= 1 && topCount <= frequencies.Length && (topCount == frequencies.Length || frequencies[topCount - 1] > frequencies[topCount]);
    }
}
