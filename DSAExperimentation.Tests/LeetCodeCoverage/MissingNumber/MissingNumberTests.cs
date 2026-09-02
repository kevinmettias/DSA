using DSAExperimentation.LeetCode.MissingNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MissingNumber;

// Harness only: both strategies live in MissingNumberSolution and are asserted
// against the same examples.
public sealed class MissingNumberTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [3, 0, 1], 2 },
            { [0, 1], 2 },
            { [9, 6, 4, 2, 3, 5, 7, 0, 1], 8 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMissingNumberByBruteForce_LeetCodeExamples_ReturnsMissingValue(int[] nums, int expected) =>
        Assert.Equal(expected, MissingNumberSolution.FindMissingNumberByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMissingNumberBySetMembership_LeetCodeExamples_ReturnsMissingValue(int[] nums, int expected) =>
        Assert.Equal(expected, MissingNumberSolution.FindMissingNumberBySetMembership(nums));
}
