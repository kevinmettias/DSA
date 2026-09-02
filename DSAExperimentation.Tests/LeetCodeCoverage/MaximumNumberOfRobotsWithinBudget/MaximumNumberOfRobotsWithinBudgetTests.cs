using RepoDeque = DSAExperimentation.DataStructures.Deque.Deque<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfRobotsWithinBudget;

// LeetCode 2398. Maximum Number of Robots Within Budget: a two-pointer sliding
// window whose cost is max(chargeTimes in window) + windowSize * sum(runningCosts
// in window). The max term reuses SlidingWindowMaximumTests' own monotonic-deque-
// of-indices trick over this repo's Deque<int> (decreasing chargeTimes order, so
// the front index is always the window's max); the sum term is a running total
// maintained as the window's left edge advances. Since runningCosts are positive,
// cost is strictly increasing in window size for a fixed left edge, so shrinking
// from the left whenever the budget is exceeded (instead of re-scanning) is safe -
// no new primitive needed for either half.
public sealed class MaximumNumberOfRobotsWithinBudgetTests
{
    [Fact]
    public void MaximumRobots_LeetCodeExampleOne_ReturnsThree()
    {
        int[] chargeTimes = [3, 6, 1, 3, 4];
        int[] runningCosts = [2, 1, 3, 4, 5];

        var result = MaximumRobots(chargeTimes, runningCosts, budget: 25);

        Assert.Equal(3, result);
    }

    [Fact]
    public void MaximumRobots_LeetCodeExampleTwo_ReturnsZero()
    {
        int[] chargeTimes = [11, 12, 19];
        int[] runningCosts = [10, 8, 7];

        var result = MaximumRobots(chargeTimes, runningCosts, budget: 19);

        Assert.Equal(0, result);
    }

    [Fact]
    public void MaximumRobots_BudgetCoversEveryRobot_ReturnsFullLength()
    {
        int[] chargeTimes = [1, 2, 3, 4];
        int[] runningCosts = [1, 1, 1, 1];

        var result = MaximumRobots(chargeTimes, runningCosts, budget: 1_000);

        Assert.Equal(4, result);
    }

    private static int MaximumRobots(int[] chargeTimes, int[] runningCosts, long budget)
    {
        var maxWindow = new RepoDeque();
        var best = 0;
        long runningCostSum = 0;
        var left = 0;

        for (var right = 0; right < chargeTimes.Length; right++)
        {
            while (maxWindow.TryPeekBack(out var backIndex) && chargeTimes[backIndex] <= chargeTimes[right])
            {
                maxWindow.TryPopBack(out _);
            }

            maxWindow.PushBack(right);
            runningCostSum += runningCosts[right];

            while (maxWindow.TryPeekFront(out var maxIndex)
                && chargeTimes[maxIndex] + ((long)(right - left + 1) * runningCostSum) > budget)
            {
                if (maxIndex == left)
                {
                    maxWindow.TryPopFront(out _);
                }

                runningCostSum -= runningCosts[left];
                left++;
            }

            best = Math.Max(best, right - left + 1);
        }

        return best;
    }
}
