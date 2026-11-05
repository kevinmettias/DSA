using DSAExperimentation.LeetCode.Solutions.TaskScheduler;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TaskScheduler;

// Harness only. All three strategies are TaskSchedulerSolution's; this file pins them to
// three schedules of its own.
public sealed partial class TaskSchedulerSolutionTests
{
    public static TheoryData<char[], int, int> Cases =>
        new()
        {
            // With no cooldown nothing idles, so 6 tasks take 6 ticks. A, B and C each
            // run 3 times with n = 2: (3 - 1) * (2 + 1) + 3 = 9 slots, fewer than the
            // 12 tasks, so the tasks fill every gap and take 12. One task takes 1.
            { ['A', 'A', 'A', 'B', 'B', 'B'], 0, 6 },
            { ['A', 'A', 'A', 'B', 'B', 'B', 'C', 'C', 'C', 'D', 'D', 'E'], 2, 12 },
            { ['A'], 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LeastIntervalByCooldownHeap_Cases_ReturnsFewestCpuTicks(
        char[] tasks, int cooldownTicks, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByCooldownHeap(tasks, cooldownTicks);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LeastIntervalByIdleSlotFormula_Cases_ReturnsFewestCpuTicks(
        char[] tasks, int cooldownTicks, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByIdleSlotFormula(tasks, cooldownTicks);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LeastIntervalByArrayScan_Cases_ReturnsFewestCpuTicks(
        char[] tasks, int cooldownTicks, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByArrayScan(tasks, cooldownTicks);

        Assert.Equal(expected, actual);
    }
}
