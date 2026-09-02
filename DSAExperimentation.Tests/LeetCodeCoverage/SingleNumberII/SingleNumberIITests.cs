using DSAExperimentation.LeetCode.SingleNumberII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SingleNumberII;

// Harness only: the one strategy lives in SingleNumberIISolution and is asserted
// against LeetCode's published examples plus a single-element case and a
// negative-value case the two-case original didn't cover.
public sealed class SingleNumberIITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [2, 2, 3, 2], 3 },
            { [0, 1, 0, 1, 0, 1, 99], 99 },
            { [5], 5 },
            { [-2, -2, 1, -2], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSingleByBitCountModThree_LeetCodeExamples_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberIISolution.FindSingleByBitCountModThree(nums));
}
