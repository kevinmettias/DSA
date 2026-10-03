using DSAExperimentation.LeetCode.SingleNumber;

namespace DSAExperimentation.LeetCode.Tests.SingleNumber;

// Harness only: both strategies live in SingleNumberSolution and are asserted
// against LeetCode's published examples plus a single-element case and a
// negative-value case.
public sealed partial class SingleNumberSolutionTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { new[] { 2, 2, 1 }, 1 },
            { new[] { 4, 1, 2, 1, 2 }, 4 },
            { new[] { 1 }, 1 },
            { new[] { -1, -1, -2 }, -2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindUniqueByXorFold_LeetCodeExamples_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberSolution.FindUniqueByXorFold(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindUniqueBySetToggling_LeetCodeExamples_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberSolution.FindUniqueBySetToggling(nums));
}
