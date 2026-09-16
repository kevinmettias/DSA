using DSAExperimentation.LeetCode.MaximumNumberOfRobotsWithinBudget;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfRobotsWithinBudget;

// Harness only. Both strategies are MaximumNumberOfRobotsWithinBudgetSolution's -
// this file just pins them to LeetCode's published examples, plus the two
// single-robot edges (exactly on budget, one unit over) that separate a window of
// length one from a window of length zero.
public sealed partial class MaximumNumberOfRobotsWithinBudgetTests
{
    public static TheoryData<int[], int[], long, int> Examples =>
        new()
        {
            { [3, 6, 1, 3, 4], [2, 1, 3, 4, 5], 25L, 3 },
            { [11, 12, 19], [10, 8, 7], 19L, 0 },
            { [1, 2, 3, 4], [1, 1, 1, 1], 1_000L, 4 },
            { [5], [5], 10L, 1 },
            { [5], [6], 10L, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumRobotsByRescanEveryLeftEdge_LeetCodeExamples_ReturnsLongestAffordableRun(
        int[] chargeTimes, int[] runningCosts, long budget, int expected)
    {
        var actual = MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByRescanEveryLeftEdge(
            chargeTimes, runningCosts, budget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumRobotsByMonotonicDeque_LeetCodeExamples_ReturnsLongestAffordableRun(
        int[] chargeTimes, int[] runningCosts, long budget, int expected)
    {
        var actual = MaximumNumberOfRobotsWithinBudgetSolution.MaximumRobotsByMonotonicDeque(
            chargeTimes, runningCosts, budget);

        Assert.Equal(expected, actual);
    }
}
