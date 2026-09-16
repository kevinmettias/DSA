using DSAExperimentation.LeetCode.MinimumOperationsToMakeArrayEqualToTarget;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOperationsToMakeArrayEqualToTarget;

// Harness only. Both strategies are MinimumOperationsToMakeArrayEqualToTargetSolution's
// - this file just pins them to LeetCode's published examples, including the mixed
// increment/decrement case (Example 2) that a formula covering only the LC 1526
// same-direction case would get wrong.
public sealed class MinimumOperationsToMakeArrayEqualToTargetTests
{
    public static TheoryData<int[], int[], long> Examples =>
        new()
        {
            { [3, 5, 1, 2], [4, 6, 2, 4], 2 },
            { [1, 3, 2], [2, 1, 4], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceSimulation_LeetCodeExamples_ReturnsMinimumOperationCount(
        int[] nums, int[] target, long expected)
    {
        var actual = MinimumOperationsToMakeArrayEqualToTargetSolution.MinOperationsByBruteForceSimulation(nums, target);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByDifferenceScan_LeetCodeExamples_ReturnsMinimumOperationCount(
        int[] nums, int[] target, long expected)
    {
        var actual = MinimumOperationsToMakeArrayEqualToTargetSolution.MinOperationsByDifferenceScan(nums, target);
        Assert.Equal(expected, actual);
    }
}
