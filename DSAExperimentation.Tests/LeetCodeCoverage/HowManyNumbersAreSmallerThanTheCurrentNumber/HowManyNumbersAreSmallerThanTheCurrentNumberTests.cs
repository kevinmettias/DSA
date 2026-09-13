using DSAExperimentation.LeetCode.HowManyNumbersAreSmallerThanTheCurrentNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HowManyNumbersAreSmallerThanTheCurrentNumber;

// Harness only. Both counting strategies are
// HowManyNumbersAreSmallerThanTheCurrentNumberSolution's - this file pins them to
// LeetCode's three published examples plus the all-equal case, which is the one
// that catches a LowerBound that reported an upper bound instead.
public sealed class HowManyNumbersAreSmallerThanTheCurrentNumberTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [8, 1, 2, 2, 3], [4, 0, 1, 1, 3] },
            { [6, 5, 4, 8], [2, 1, 0, 3] },
            { [7, 7, 7, 7], [0, 0, 0, 0] },
            { [1], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallerNumbersThanCurrentByPairwiseCount_LeetCodeExamples_ReturnsCountsPerElement(
        int[] nums, int[] expected) =>
        Assert.Equal(
            expected,
            HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentByPairwiseCount(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallerNumbersThanCurrentBySortAndLowerBound_LeetCodeExamples_ReturnsCountsPerElement(
        int[] nums, int[] expected) =>
        Assert.Equal(
            expected,
            HowManyNumbersAreSmallerThanTheCurrentNumberSolution.SmallerNumbersThanCurrentBySortAndLowerBound(nums));
}
