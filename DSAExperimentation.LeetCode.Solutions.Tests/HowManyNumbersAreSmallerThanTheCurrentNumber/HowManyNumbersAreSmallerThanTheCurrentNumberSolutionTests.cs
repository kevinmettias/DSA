using DSAExperimentation.LeetCode.Solutions.HowManyNumbersAreSmallerThanTheCurrentNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.HowManyNumbersAreSmallerThanTheCurrentNumber;

// Harness only. Both counting strategies are
// HowManyNumbersAreSmallerThanTheCurrentNumberSolution's - this file pins them to
// the all-equal case, which is the one
// that catches a LowerBound that reported an upper bound instead.
public sealed partial class HowManyNumbersAreSmallerThanTheCurrentNumberSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [1], [0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallerNumbersThanCurrentByPairwiseCount_Cases_ReturnsCountsPerElement(
        int[] nums, int[] expected) =>
        Assert.Equal(
            expected,
            HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentByPairwiseCount(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void SmallerNumbersThanCurrentBySortAndLowerBound_Cases_ReturnsCountsPerElement(
        int[] nums, int[] expected) =>
        Assert.Equal(
            expected,
            HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentBySortAndLowerBound(nums));
}
