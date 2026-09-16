using DSAExperimentation.LeetCode.PlusOne;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PlusOne;

// Harness only: both strategies live in PlusOneSolution and are asserted against
// the same examples, including the all-nines case that grows the result.
public sealed partial class PlusOneTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { new[] { 1, 2, 3 }, new[] { 1, 2, 4 } },
            { new[] { 4, 3, 2, 1 }, new[] { 4, 3, 2, 2 } },
            { new[] { 9 }, new[] { 1, 0 } },
            { new[] { 9, 9, 9 }, new[] { 1, 0, 0, 0 } },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IncrementByArrayWalk_LeetCodeExamples_ReturnsIncrementedDigits(int[] digits, int[] expected) =>
        Assert.Equal(expected, PlusOneSolution.IncrementByArrayWalk(digits));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IncrementByDigitStack_LeetCodeExamples_ReturnsIncrementedDigits(int[] digits, int[] expected) =>
        Assert.Equal(expected, PlusOneSolution.IncrementByDigitStack(digits));
}
