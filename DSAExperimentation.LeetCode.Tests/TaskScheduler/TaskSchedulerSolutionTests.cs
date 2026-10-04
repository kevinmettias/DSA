using DSAExperimentation.LeetCode.TaskScheduler;

namespace DSAExperimentation.LeetCode.Tests.TaskScheduler;

// Harness only. Both strategies are TaskSchedulerSolution's; this file pins them to
// LeetCode's published examples and three schedules of its own.
public sealed partial class TaskSchedulerSolutionTests
{
    public static TheoryData<char[], int, int> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { ['A', 'A', 'A', 'B', 'B', 'B'], 2, 8 },
            { ['A', 'C', 'A', 'B', 'D', 'B'], 1, 6 },
            { ['A', 'A', 'A', 'B', 'B', 'B'], 3, 10 },

            // With no cooldown nothing idles, so 6 tasks take 6 ticks. A, B and C each
            // run 3 times with n = 2: (3 - 1) * (2 + 1) + 3 = 9 slots, fewer than the
            // 12 tasks, so the tasks fill every gap and take 12. One task takes 1.
            { ['A', 'A', 'A', 'B', 'B', 'B'], 0, 6 },
            { ['A', 'A', 'A', 'B', 'B', 'B', 'C', 'C', 'C', 'D', 'D', 'E'], 2, 12 },
            { ['A'], 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastIntervalByCooldownHeap_LeetCodeExamples_ReturnsFewestCpuTicks(
        char[] tasks, int cooldownTicks, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByCooldownHeap(tasks, cooldownTicks);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastIntervalByArrayScan_LeetCodeExamples_ReturnsFewestCpuTicks(
        char[] tasks, int cooldownTicks, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByArrayScan(tasks, cooldownTicks);

        Assert.Equal(expected, actual);
    }
}
