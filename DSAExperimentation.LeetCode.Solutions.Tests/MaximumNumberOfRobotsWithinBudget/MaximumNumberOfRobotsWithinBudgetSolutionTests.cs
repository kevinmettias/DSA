using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfRobotsWithinBudget;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfRobotsWithinBudget;

// Harness only. Both strategies are MaximumNumberOfRobotsWithinBudgetSolution's -
// this file just pins them to the two
// single-robot edges (exactly on budget, one unit over) that separate a window of
// length one from a window of length zero.
public sealed partial class MaximumNumberOfRobotsWithinBudgetSolutionTests
{
    public static TheoryData<int[], int[], long, int> Cases =>
        new()
        {
            { [1, 2, 3, 4], [1, 1, 1, 1], 1_000L, 4 },
            { [5], [5], 10L, 1 },
            { [5], [6], 10L, 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumRobotsByRescanEveryLeftEdge_Cases_ReturnsLongestAffordableRun(
        int[] chargeTimes, int[] runningCosts, long budget, int expected)
    {
        var actual = MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByRescanEveryLeftEdge(
            chargeTimes, runningCosts, budget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumRobotsByMonotonicDeque_Cases_ReturnsLongestAffordableRun(
        int[] chargeTimes, int[] runningCosts, long budget, int expected)
    {
        var actual = MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByMonotonicDeque(
            chargeTimes, runningCosts, budget);

        Assert.Equal(expected, actual);
    }
}
