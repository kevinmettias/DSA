using DSAExperimentation.LeetCode.FindTheDuplicateNumber;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheDuplicateNumber;

// Harness only. Both strategies are FindTheDuplicateNumberSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class FindTheDuplicateNumberTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 4, 2, 2], 2 },
            { [3, 1, 3, 4, 2], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDuplicateByBruteForce_LeetCodeExamples_ReturnsRepeatedValue(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheDuplicateNumberSolution.FindDuplicateByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindDuplicateByCycleDetection_LeetCodeExamples_ReturnsRepeatedValue(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheDuplicateNumberSolution.FindDuplicateByCycleDetection(nums));
}
