using DSAExperimentation.LeetCode.Solutions.SingleNumberII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SingleNumberII;

// Harness only: both strategies live in SingleNumberIISolution and are asserted
// against a single-element case and a
// negative-value case the two-case original didn't cover.
public sealed partial class SingleNumberIISolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [5], 5 },
            { [-2, -2, 1, -2], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindSingleByBitCountModThree_Cases_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberIISolution.FindSingleByBitCountModThree(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindSingleByTwoBitCounters_Cases_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberIISolution.FindSingleByTwoBitCounters(nums));
}
