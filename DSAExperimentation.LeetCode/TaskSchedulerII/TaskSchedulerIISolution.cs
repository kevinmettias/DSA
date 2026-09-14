using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.TaskSchedulerII;

// LeetCode 2365. Task Scheduler II: tasks are performed in the given order, one per
// day at the earliest, and two tasks of the same type must be at least `space` days
// apart - so the answer is the day the last task lands on.
//
// The whole problem is "when was this task type last performed?", asked once per
// task. The two strategies differ only in how they answer it: rescan every earlier
// task looking for the same type, or remember each type's most recent day as you go.
internal static class TaskSchedulerIISolution
{
    // The textbook answer: for each task walk backwards over everything already
    // scheduled until the same type turns up, giving O(n) work per task and O(n^2)
    // overall. Plain BCL arrays and loops - it is the arm the HashMap strategy below
    // has to justify itself against.
    public static long CountDaysByBackwardScan(int[] tasks, int space)
    {
        var dayOfIndex = new long[tasks.Length];
        long currentDay = 0;

        for (var index = 0; index < tasks.Length; index++)
        {
            var mustWait = TryFindPreviousDay(tasks, dayOfIndex, index, out var previousDay) &&
                currentDay - previousDay <= space;

            currentDay = mustWait ? previousDay + space + 1 : currentDay + 1;
            dayOfIndex[index] = currentDay;
        }

        return currentDay;
    }

    private static bool TryFindPreviousDay(int[] tasks, long[] dayOfIndex, int index, out long previousDay)
    {
        for (var earlier = index - 1; earlier >= 0; earlier--)
        {
            if (tasks[earlier] == tasks[index])
            {
                previousDay = dayOfIndex[earlier];
                return true;
            }
        }

        previousDay = 0;
        return false;
    }

    // This repo's own HashMap<int,long> keyed by task type: one forward pass, one
    // lookup and one write per task, so the cooldown check is O(1) average instead of
    // a backward rescan - the same "have I seen this key recently" bookkeeping
    // TwoSum and LongestSubstringWithoutRepeatingCharacters use, applied to task-type
    // recency rather than value or character recency.
    public static long CountDaysByHashMapOnePass(int[] tasks, int space)
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
