using DSAExperimentation.LeetCode.SumOfTotalStrengthOfWizards;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfTotalStrengthOfWizards;

// Harness only: both strategies live in SumOfTotalStrengthOfWizardsSolution and are
// asserted against the same examples - LeetCode's two published cases, the three
// hand-enumerated cases this file carried before the algorithm moved to tier 4
// (increasing values, a repeated minimum, a lone element), a run of equal values
// that exercises the </<= tie-breaking end to end, and a strictly decreasing array
// where the last element is the minimum of every subarray reaching it.
public sealed partial class SumOfTotalStrengthOfWizardsTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 1, 2], 44 },
            { [5, 4, 6], 213 },
            { [1, 2, 3], 33 },
            { [2, 1, 2], 20 },
            { [7], 49 },
            { [2, 2, 2], 40 },
            { [4, 3, 2, 1], 98 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalStrengthByBruteForce_LeetCodeExamples_ReturnsSummedMinimumTimesSum(int[] strength, int expected) =>
        Assert.Equal(expected, SumOfTotalStrengthOfWizardsSolution.TotalStrengthByBruteForce(strength));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalStrengthByMonotonicStack_LeetCodeExamples_ReturnsSummedMinimumTimesSum(int[] strength, int expected) =>
        Assert.Equal(expected, SumOfTotalStrengthOfWizardsSolution.TotalStrengthByMonotonicStack(strength));
}
