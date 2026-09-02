using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfWorkSessionsToFinishTheTasks;

// LeetCode 1986. Minimum Number of Work Sessions to Finish the Tasks: bitmask DP over "which
// tasks are still unscheduled," the same ParallelCoursesII/SmallestSufficientTeam shape via this
// repo's own Memoizer, keyed on an int bitmask (one bit per task, tasks.Length <= 14). feasible[mask]
// precomputes whether that task subset fits in one session (sum <= sessionTime); the recurrence
// picks any non-empty feasible submask of what's left as "the next session" and recurses on the
// remainder, taking whichever split needs the fewest further sessions - submask enumeration
// (`sub = (sub - 1) & remaining`) is the same bit trick ParallelCoursesIITests already uses.
public sealed partial class MinimumNumberOfWorkSessionsToFinishTheTasksTests
{
    [Fact]
    public void MinSessions_ClassicExample_PacksTwoTasksThenOne()
    {
        int[] tasks = [1, 2, 3];

        var result = MinSessions(tasks, sessionTime: 3);

        Assert.Equal(2, result);
    }

    [Fact]
    public void MinSessions_FiveTasksFittingTwoSessions_ReturnsTwo()
    {
        int[] tasks = [3, 1, 3, 1, 1];

        var result = MinSessions(tasks, sessionTime: 8);

        Assert.Equal(2, result);
    }

    [Fact]
    public void MinSessions_AllTasksFitOneSession_ReturnsOne()
    {
        int[] tasks = [1, 2, 3, 4, 5];

        var result = MinSessions(tasks, sessionTime: 15);

        Assert.Equal(1, result);
    }

    private static int MinSessions(int[] tasks, int sessionTime)
    {
        var feasible = ComputeFeasibleMasks(tasks, sessionTime);
        var fullMask = (1 << tasks.Length) - 1;

        return Memoizer.Memoize<int, int>(fullMask, (remaining, sessionsFor) =>
            remaining == 0 ? 0 : 1 + BestOverFeasibleSubsets(remaining, feasible, sessionsFor));
    }

    private static bool[] ComputeFeasibleMasks(int[] tasks, int sessionTime)
    {
        var maskCount = 1 << tasks.Length;
        var sum = new int[maskCount];
        var feasible = new bool[maskCount];

        for (var mask = 1; mask < maskCount; mask++)
        {
            var lowestBit = mask & -mask;
            var taskIndex = TrailingZeroCount(lowestBit);
            sum[mask] = sum[mask ^ lowestBit] + tasks[taskIndex];
            feasible[mask] = sum[mask] <= sessionTime;
        }

        return feasible;
    }

    private static int BestOverFeasibleSubsets(int remaining, bool[] feasible, Func<int, int> sessionsFor)
    {
        var best = int.MaxValue;

        for (var sub = remaining; sub > 0; sub = (sub - 1) & remaining)
        {
            if (feasible[sub])
            {
                best = Math.Min(best, sessionsFor(remaining ^ sub));
            }
        }

        return best;
    }

    private static int TrailingZeroCount(int lowestBit)
    {
        var index = 0;

        while ((lowestBit & 1) == 0)
        {
            lowestBit >>= 1;
            index++;
        }

        return index;
    }
}
