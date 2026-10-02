using DSAExperimentation.LeetCode.SingleNumberII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SingleNumberII;

// Harness only: both strategies live in SingleNumberIISolution and are asserted
// against LeetCode's published examples plus a single-element case and a
// negative-value case the two-case original didn't cover.
public sealed partial class SingleNumberIITests
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

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSingleByTwoBitCounters_LeetCodeExamples_ReturnsTheUnpairedValue(int[] nums, int expected) =>
        Assert.Equal(expected, SingleNumberIISolution.FindSingleByTwoBitCounters(nums));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they name the same survivor on every example - not merely that
    // each agrees with the expectation beside it.
    [Theory]
    [MemberData(nameof(Examples))]
    public void FindSingle_AgreeOnEveryExample(int[] nums, int expected) =>
        Assert.Equal(
            SingleNumberIISolution.FindSingleByBitCountModThree(nums),
            SingleNumberIISolution.FindSingleByTwoBitCounters(nums));
}
