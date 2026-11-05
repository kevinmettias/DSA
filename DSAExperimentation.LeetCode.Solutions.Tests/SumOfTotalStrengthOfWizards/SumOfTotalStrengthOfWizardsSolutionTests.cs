using DSAExperimentation.LeetCode.Solutions.SumOfTotalStrengthOfWizards;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumOfTotalStrengthOfWizards;

// Harness only: both strategies live in SumOfTotalStrengthOfWizardsSolution and are
// asserted against the same cases - LeetCode's two published cases, the three
// hand-enumerated cases this file carried before the algorithm moved to tier 4
// (increasing values, a repeated minimum, a lone element), a run of equal values
// that exercises the </<= tie-breaking end to end, and a strictly decreasing array
// where the last element is the minimum of every subarray reaching it.
public sealed partial class SumOfTotalStrengthOfWizardsSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 2, 3], 33 },
            { [2, 1, 2], 20 },
            { [7], 49 },
            { [2, 2, 2], 40 },
            { [4, 3, 2, 1], 98 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalStrengthByBruteForce_Cases_ReturnsSummedMinimumTimesSum(int[] strength, int expected) =>
        Assert.Equal(expected, SumOfTotalStrengthOfWizardsSolution.TotalStrengthByBruteForce(strength));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalStrengthByMonotonicStack_Cases_ReturnsSummedMinimumTimesSum(int[] strength, int expected) =>
        Assert.Equal(expected, SumOfTotalStrengthOfWizardsSolution.TotalStrengthByMonotonicStack(strength));
}
