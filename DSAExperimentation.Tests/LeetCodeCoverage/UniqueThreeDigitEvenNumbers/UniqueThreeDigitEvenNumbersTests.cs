using DSAExperimentation.LeetCode.UniqueThreeDigitEvenNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniqueThreeDigitEvenNumbers;

// Harness only. Both strategies are UniqueThreeDigitEvenNumbersSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class UniqueThreeDigitEvenNumbersTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4], 12 },
            { [0, 2, 2], 2 },
            { [6, 6, 6], 1 },
            { [1, 3, 5], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByIndexPermutationScan_LeetCodeExamples_ReturnsDistinctEvenNumberCount(
        int[] digits, int expected) =>
        Assert.Equal(expected, UniqueThreeDigitEvenNumbersSolution.CountByIndexPermutationScan(digits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBacktracking_LeetCodeExamples_ReturnsDistinctEvenNumberCount(
        int[] digits, int expected) =>
        Assert.Equal(expected, UniqueThreeDigitEvenNumbersSolution.CountByBacktracking(digits));
}
