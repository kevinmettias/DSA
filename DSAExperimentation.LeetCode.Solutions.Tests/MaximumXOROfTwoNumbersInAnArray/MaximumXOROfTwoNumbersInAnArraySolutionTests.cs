using DSAExperimentation.LeetCode.Solutions.MaximumXOROfTwoNumbersInAnArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumXOROfTwoNumbersInAnArray;

// Harness only. Both strategies are MaximumXOROfTwoNumbersInAnArraySolution's - this
// file pins them to a two-element array and to a single element, whose only pair is
// itself and so XORs to 0, one theory per strategy so a failure names the strategy
// that broke. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MaximumXOROfTwoNumbersInAnArraySolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [14, 70], 14 ^ 70 },
            { [9], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaximumXorByPairwiseScan_Cases_ReturnsBestPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByPairwiseScan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaximumXorByBitTrie_Cases_ReturnsBestPairXor(int[] nums, int expected) =>
        Assert.Equal(expected, MaximumXOROfTwoNumbersInAnArraySolution.FindMaximumXorByBitTrie(nums));
}
