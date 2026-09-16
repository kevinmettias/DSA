using DSAExperimentation.LeetCode.StepsToMakeArrayNonDecreasing;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StepsToMakeArrayNonDecreasing;

// Harness only: both strategies are StepsToMakeArrayNonDecreasingSolution's -
// this file just pins them to LeetCode's published examples, plus the boundary
// cases the round simulation and the monotonic sweep disagree about most easily:
// an array that is already sorted (no round runs), a strictly decreasing one
// (every removal happens in a single round), and a leading maximum that peels one
// element per round for as many rounds as there are elements behind it.
public sealed partial class StepsToMakeArrayNonDecreasingTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [5, 3, 4, 4, 7, 3, 6, 11, 8, 5, 11], 3 },
            { [4, 5, 7, 7, 13], 0 },
            { [9, 7, 5, 3, 1], 1 },
            { [10, 1, 2, 3, 4], 4 },
            { [1, 2, 3, 10, 4, 5], 2 },
            { [7, 7, 7], 0 },
            { [1], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalStepsBySimulatingRounds_LeetCodeExamples_ReturnsRoundsUntilNonDecreasing(
        int[] nums, int expected) =>
        Assert.Equal(expected, StepsToMakeArrayNonDecreasingSolution.TotalStepsBySimulatingRounds(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TotalStepsByMonotonicStackSweep_LeetCodeExamples_ReturnsRoundsUntilNonDecreasing(
        int[] nums, int expected) =>
        Assert.Equal(expected, StepsToMakeArrayNonDecreasingSolution.TotalStepsByMonotonicStackSweep(nums));
}
