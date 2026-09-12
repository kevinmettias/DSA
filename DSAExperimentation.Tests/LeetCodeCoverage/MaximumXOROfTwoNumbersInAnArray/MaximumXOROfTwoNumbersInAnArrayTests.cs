using DSAExperimentation.LeetCode.MaximumXOROfTwoNumbersInAnArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumXOROfTwoNumbersInAnArray;

// Harness only. Both strategies are MaximumXOROfTwoNumbersInAnArraySolution's - this
// file just pins them to LeetCode's published examples, one theory per strategy so a
// failure names the strategy that broke.
public sealed class MaximumXOROfTwoNumbersInAnArrayTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 10, 5, 25, 2, 8], 28 },
            { [14, 70], 14 ^ 70 },
            { [9], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximumXorByPairwiseScan_LeetCodeExamples_ReturnsBestPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaximumXorByBitTrie_LeetCodeExamples_ReturnsBestPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByBitTrie(nums));
}
