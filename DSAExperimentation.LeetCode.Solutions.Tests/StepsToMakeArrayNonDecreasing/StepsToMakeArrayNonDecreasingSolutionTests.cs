using DSAExperimentation.LeetCode.Solutions.StepsToMakeArrayNonDecreasing;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StepsToMakeArrayNonDecreasing;

// Harness only: both strategies are StepsToMakeArrayNonDecreasingSolution's -
// this file just pins them to the boundary
// cases the round simulation and the monotonic sweep disagree about most easily:
// an array that is already sorted (no round runs), a strictly decreasing one
// (every removal happens in a single round), and a leading maximum that peels one
// element per round for as many rounds as there are elements behind it.
public sealed partial class StepsToMakeArrayNonDecreasingSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [9, 7, 5, 3, 1], 1 },
            { [10, 1, 2, 3, 4], 4 },
            { [1, 2, 3, 10, 4, 5], 2 },
            { [7, 7, 7], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalStepsBySimulatingRounds_Cases_ReturnsRoundsUntilNonDecreasing(
        int[] nums, int expected) =>
        Assert.Equal(expected, StepsToMakeArrayNonDecreasingSolution.TotalStepsBySimulatingRounds(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void TotalStepsByMonotonicStackSweep_Cases_ReturnsRoundsUntilNonDecreasing(
        int[] nums, int expected) =>
        Assert.Equal(expected, StepsToMakeArrayNonDecreasingSolution.TotalStepsByMonotonicStackSweep(nums));
}
