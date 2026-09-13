using DSAExperimentation.LeetCode.CheckIfItIsAGoodArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfItIsAGoodArray;

// Harness only. Both gcd strategies are CheckIfItIsAGoodArraySolution's - this
// file just pins them to LeetCode's published examples plus the degenerate
// single-element cases and a set whose values are pairwise non-coprime yet whose
// overall gcd is still 1.
public sealed class CheckIfItIsAGoodArrayTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [12, 5, 7, 23], true },
            { [29, 6, 10], true },
            { [3, 6], false },
            { [6, 10, 15], true },
            { [2, 4, 8], false },
            { [1], true },
            { [5], false },
            { [1, 1], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsGoodArrayBySubtractionGcd_LeetCodeExamples_ReportsWhetherTheArrayGcdIsOne(
        int[] nums, bool expected) =>
        Assert.Equal(expected, CheckIfItIsAGoodArraySolution.IsGoodArrayBySubtractionGcd(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsGoodArrayByEuclideanGcd_LeetCodeExamples_ReportsWhetherTheArrayGcdIsOne(
        int[] nums, bool expected) =>
        Assert.Equal(expected, CheckIfItIsAGoodArraySolution.IsGoodArrayByEuclideanGcd(nums));
}
