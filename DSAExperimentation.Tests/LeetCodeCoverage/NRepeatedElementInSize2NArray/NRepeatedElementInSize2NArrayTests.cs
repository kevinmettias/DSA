using DSAExperimentation.LeetCode.NRepeatedElementInSize2NArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NRepeatedElementInSize2NArray;

// Harness only. Both strategies are NRepeatedElementInSize2NArraySolution's - the
// pairwise baseline the benchmark used to hide, and the Set<int> single pass - so this
// file just pins them to LeetCode's published examples plus the cases the original
// test added: the repeat trailing the array, and the smallest valid n = 2 input.
public sealed class NRepeatedElementInSize2NArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 3], 3 },
            { [2, 1, 2, 5, 3, 2], 2 },
            { [5, 1, 5, 2, 5, 3, 5, 4], 5 },
            // n = 2: 4 elements, n + 1 = 3 unique values, one repeated exactly n times.
            { [4, 5, 4, 6], 4 },
            // The repeat is the very first pair, so neither strategy gets to scan far.
            { [9, 9, 1, 2], 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRepeatedByPairwiseScan_LeetCodeExamples_ReturnsTheValueRepeatedNTimes(
        int[] nums, int expected) =>
        Assert.Equal(expected, NRepeatedElementInSize2NArraySolution.FindRepeatedByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRepeatedByTrackingSet_LeetCodeExamples_ReturnsTheValueRepeatedNTimes(
        int[] nums, int expected) =>
        Assert.Equal(expected, NRepeatedElementInSize2NArraySolution.FindRepeatedByTrackingSet(nums));
}
