using DSAExperimentation.LeetCode.TaskSchedulerII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TaskSchedulerII;

// Harness only: both strategies live in TaskSchedulerIISolution and are asserted here
// under their own names, so a failure names the strategy that broke. The cases are
// LeetCode's two published examples plus the boundaries the cooldown rule turns on -
// no repeats at all, back-to-back repeats that each force a full wait, and a single
// task that can never wait for anything.
public sealed partial class TaskSchedulerIITests
{
    public static TheoryData<int[], int, long> Examples => new()
    {
        { [1, 2, 1, 2, 3, 1], 3, 9 },
        { [5, 8, 8, 5], 2, 6 },
        { [1, 2, 3, 4], 10, 4 },
        { [1, 1, 1], 2, 7 },
        { [2, 2], 1, 3 },
        { [7], 5, 1 },
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDaysByBackwardScan_Example_ReturnsDayTheLastTaskLandsOn(
        int[] tasks, int space, long expected)
    {
        var actual = TaskSchedulerIISolution.CountDaysByBackwardScan(tasks, space);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountDaysByHashMapOnePass_Example_ReturnsDayTheLastTaskLandsOn(
        int[] tasks, int space, long expected)
    {
        var actual = TaskSchedulerIISolution.CountDaysByHashMapOnePass(tasks, space);

        Assert.Equal(expected, actual);
    }
}
