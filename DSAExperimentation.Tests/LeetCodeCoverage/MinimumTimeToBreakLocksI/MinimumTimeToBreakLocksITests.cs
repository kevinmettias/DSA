using DSAExperimentation.LeetCode.MinimumTimeToBreakLocksI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToBreakLocksI;

// Harness only: both strategies live in MinimumTimeToBreakLocksISolution. One
// test method per strategy over one shared set of LeetCode's own examples, so
// a failure names the strategy that broke.
public sealed partial class MinimumTimeToBreakLocksITests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [3, 4, 1], 1, 4 },
            { [2, 5, 4], 2, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinimumTimeByPermutationBruteForce_LeetCodeExamples_ReturnsMinimumMinutes(
        int[] strength, int energyStep, int expected)
    {
        var actual = MinimumTimeToBreakLocksISolution.FindMinimumTimeByPermutationBruteForce(strength, energyStep);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMinimumTimeByBitmaskMemo_LeetCodeExamples_ReturnsMinimumMinutes(
        int[] strength, int energyStep, int expected)
    {
        var actual = MinimumTimeToBreakLocksISolution.FindMinimumTimeByBitmaskMemo(strength, energyStep);

        Assert.Equal(expected, actual);
    }
}
