namespace DSAExperimentation.LeetCode.Solutions.MinimumSwapsToMakeSequencesIncreasing;

// LC 801's promise about its input: some choice of swaps makes both sequences strictly increasing.
// LeetCode's judge never poses an input that breaks it. Where none does, both strategies answer
// int.MaxValue. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class MinimumSwapsToMakeSequencesIncreasingPromise
{
    public static bool IsKeptBy(int[] nums1, int[] nums2)
    {
        var keptIsPossible = true;
        var swappedIsPossible = true;

        for (var index = 1; index < nums1.Length; index++)
        {
            var straight = nums1[index - 1] < nums1[index] && nums2[index - 1] < nums2[index];
            var crossed = nums1[index - 1] < nums2[index] && nums2[index - 1] < nums1[index];

            (keptIsPossible, swappedIsPossible) = (
                (keptIsPossible && straight) || (swappedIsPossible && crossed),
                (swappedIsPossible && straight) || (keptIsPossible && crossed));
        }

        return nums1.Length == nums2.Length && (keptIsPossible || swappedIsPossible);
    }
}
