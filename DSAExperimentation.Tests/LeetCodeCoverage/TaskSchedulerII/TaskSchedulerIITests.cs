using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TaskSchedulerII;

// LeetCode 2365. Task Scheduler II: a HashMap<int,long> tracks each task type's most
// recently used day; a single forward pass advances the current day past that type's
// cooldown whenever performing it today would violate `space`. The same HashMap
// single-pass bookkeeping convention TwoSumTests/LongestSubstringWithoutRepeatingCharactersTests
// already use for "have I seen this key recently", applied here to task-type recency
// instead of value or character recency.
public sealed partial class TaskSchedulerIITests
{
    [Fact]
    public void CountDays_RepeatedTaskForcesWaiting_ReturnsExpandedDayCount()
    {
        int[] tasks = [1, 2, 1, 2, 3, 1];

        var days = CountDays(tasks, space: 3);

        Assert.Equal(9, days);
    }

    [Fact]
    public void CountDays_CooldownAlreadySatisfied_SkipsOnlyWhereNeeded()
    {
        int[] tasks = [5, 8, 8, 5];

        var days = CountDays(tasks, space: 2);

        Assert.Equal(6, days);
    }

    [Fact]
    public void CountDays_AllDistinctTasks_NeedsNoWaiting()
    {
        int[] tasks = [1, 2, 3, 4];

        var days = CountDays(tasks, space: 10);

        Assert.Equal(4, days);
    }

    private static long CountDays(int[] tasks, int space)
    {
        var lastDay = new HashMap<int, long>();
        long currentDay = 0;

        foreach (var task in tasks)
        {
            if (lastDay.TryGetValue(task, out var previousDay) && currentDay - previousDay <= space)
            {
                currentDay = previousDay + space + 1;
            }
            else
            {
                currentDay++;
            }

            lastDay.Set(task, currentDay);
        }

        return currentDay;
    }
}
