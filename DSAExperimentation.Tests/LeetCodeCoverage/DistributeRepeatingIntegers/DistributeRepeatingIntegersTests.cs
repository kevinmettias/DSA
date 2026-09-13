using DSAExperimentation.LeetCode.DistributeRepeatingIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeRepeatingIntegers;

// Harness only. Both strategies are DistributeRepeatingIntegersSolution's - this
// file just pins them to LeetCode's published examples, plus the cases that
// separate "enough copies in total" from "enough copies of one value", which is
// the whole point of the problem.
public sealed class DistributeRepeatingIntegersTests
{
    public static TheoryData<int[], int[], bool> Examples =>
        new()
        {
            { [1, 2, 3, 4], [2], false },
            { [1, 2, 3, 3], [2], true },
            { [1, 1, 2, 2], [2, 2], true },
            { [1, 1, 2, 3], [2, 2], false },
            { [1, 1, 1, 1, 1], [2, 3], true },
            { [1, 1, 2, 2], [3, 1], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanDistributeByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherEveryOrderFitsOneValuesStock(
        int[] nums, int[] quantity, bool expected) =>
        Assert.Equal(expected, DistributeRepeatingIntegersSolution.CanDistributeByNaiveBacktracking(nums, quantity));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanDistributeByGenericBacktrack_LeetCodeExamples_ReturnsWhetherEveryOrderFitsOneValuesStock(
        int[] nums, int[] quantity, bool expected) =>
        Assert.Equal(expected, DistributeRepeatingIntegersSolution.CanDistributeByGenericBacktrack(nums, quantity));
}
