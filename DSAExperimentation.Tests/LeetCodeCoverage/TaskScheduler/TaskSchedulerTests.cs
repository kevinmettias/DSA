using DSAExperimentation.LeetCode.TaskScheduler;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TaskScheduler;

// Harness only. Both strategies are TaskSchedulerSolution's; this file pins them to
// LeetCode's published examples.
public sealed class TaskSchedulerTests
{
    public static TheoryData<char[], int, int> Examples =>
        new()
        {
            { ['A', 'A', 'A', 'B', 'B', 'B'], 2, 8 },
            { ['A', 'A', 'A', 'B', 'B', 'B'], 0, 6 },
            { ['A', 'A', 'A', 'B', 'B', 'B', 'C', 'C', 'C', 'D', 'D', 'E'], 2, 12 },
            { ['A'], 2, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastIntervalByCooldownHeap_LeetCodeExamples_ReturnsFewestCpuTicks(
        char[] tasks, int n, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByCooldownHeap(tasks, n);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LeastIntervalByArrayScan_LeetCodeExamples_ReturnsFewestCpuTicks(
        char[] tasks, int n, int expected)
    {
        var actual = TaskSchedulerSolution.LeastIntervalByArrayScan(tasks, n);

        Assert.Equal(expected, actual);
    }
}
