using DSAExperimentation.LeetCode.Solutions.NRepeatedElementInSize2NArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NRepeatedElementInSize2NArray;

// Harness only. Both strategies are NRepeatedElementInSize2NArraySolution's - the
// pairwise baseline the benchmark used to hide, and the Set<int> single pass - so this
// file just pins them to the cases the original
// test added: the repeat trailing the array, and the smallest valid n = 2 input.
public sealed partial class NRepeatedElementInSize2NArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            // n = 2: 4 elements, n + 1 = 3 unique values, one repeated exactly n times.
            { [4, 5, 4, 6], 4 },
            // The repeat is the very first pair, so neither strategy gets to scan far.
            { [9, 9, 1, 2], 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindRepeatedByPairwiseScan_Cases_ReturnsTheValueRepeatedNTimes(
        int[] nums, int expected) =>
        Assert.Equal(expected, NRepeatedElementInSize2NArraySolution.FindRepeatedByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindRepeatedByTrackingSet_Cases_ReturnsTheValueRepeatedNTimes(
        int[] nums, int expected) =>
        Assert.Equal(expected, NRepeatedElementInSize2NArraySolution.FindRepeatedByTrackingSet(nums));
}
