namespace DSAExperimentation.LeetCode.Solutions.MedianOfTwoSortedArrays;

// LC 4's promise about its input: both arrays are sorted, and together they hold at least one
// number. LeetCode's judge never poses an input that breaks it. The partition search leans on the
// order the merge does not need. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class MedianOfTwoSortedArraysPromise
{
    public static bool IsKeptBy(int[] nums1, int[] nums2) => nums1.Length + nums2.Length > 0 && IsSorted(nums1) && IsSorted(nums2);

    private static bool IsSorted(int[] values) => values.Zip(values.Skip(1)).All(pair => pair.First <= pair.Second);
}
