using DSAExperimentation.LeetCode.MinimumSumOfValuesByDividingArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumSumOfValuesByDividingArray;

// Harness only. Both strategies are
// MinimumSumOfValuesByDividingArraySolution's - this file just pins them to
// LeetCode's published examples, including the unsatisfiable case whose
// whole-array AND can never reach the single requested andValues entry.
public sealed partial class MinimumSumOfValuesByDividingArrayTests
{
    public static TheoryData<int[], int[], long> Examples =>
        new()
        {
            { [1, 4, 3, 3, 2], [0, 3, 3, 2], 12 },
            { [2, 3, 5, 7, 7, 7, 5], [0, 7, 5], 17 },
            { [1, 2, 3, 4], [2], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumValueSumByDictionaryMemo_LeetCodeExamples_ReturnsMinimumLastElementSum(
        int[] nums, int[] andValues, long expected)
    {
        var actual = MinimumSumOfValuesByDividingArraySolution.MinimumValueSumByDictionaryMemo(nums, andValues);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumValueSumByMemoizedPartition_LeetCodeExamples_ReturnsMinimumLastElementSum(
        int[] nums, int[] andValues, long expected)
    {
        var actual = MinimumSumOfValuesByDividingArraySolution.MinimumValueSumByMemoizedPartition(nums, andValues);
        Assert.Equal(expected, actual);
    }
}
