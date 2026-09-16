using DSAExperimentation.LeetCode.LexicographicalNumbers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LexicographicalNumbers;

// Harness only. Both strategies live in LexicographicalNumbersSolution and are
// asserted against the same examples.
public sealed partial class LexicographicalNumbersTests
{
    public static TheoryData<int, int[]> Examples =>
        new()
        {
            { 13, [1, 10, 11, 12, 13, 2, 3, 4, 5, 6, 7, 8, 9] },
            { 5, [1, 2, 3, 4, 5] },
            { 1, [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LexicalOrderByStringSort_LeetCodeExamples_ReturnsExpectedOrder(
        int upperBound, int[] expected) =>
        Assert.Equal(expected, LexicographicalNumbersSolution.LexicalOrderByStringSort(upperBound));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LexicalOrderByDepthFirstDigitTree_LeetCodeExamples_ReturnsExpectedOrder(
        int upperBound, int[] expected) =>
        Assert.Equal(
            expected, LexicographicalNumbersSolution.LexicalOrderByDepthFirstDigitTree(upperBound));
}
