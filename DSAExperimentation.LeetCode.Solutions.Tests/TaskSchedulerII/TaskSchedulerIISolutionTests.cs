using DSAExperimentation.LeetCode.Solutions.TaskSchedulerII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TaskSchedulerII;

// Harness only: both strategies live in TaskSchedulerIISolution and are asserted here
// under their own names, so a failure names the strategy that broke. The cases are
// the boundaries the cooldown rule turns on -
// no repeats at all, back-to-back repeats that each force a full wait, and a single
// task that can never wait for anything.
public sealed partial class TaskSchedulerIISolutionTests
{
    public static TheoryData<int[], int, long> Cases => new()
    {
        { [1, 2, 3, 4], 10, 4 },
        { [1, 1, 1], 2, 7 },
        { [2, 2], 1, 3 },
        { [7], 5, 1 },
    };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDaysByBackwardScan_Example_ReturnsDayTheLastTaskLandsOn(
        int[] tasks, int space, long expected)
    {
        var actual = TaskSchedulerIISolution.CountDaysByBackwardScan(tasks, space);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountDaysByHashMapOnePass_Example_ReturnsDayTheLastTaskLandsOn(
        int[] tasks, int space, long expected)
    {
        var actual = TaskSchedulerIISolution.CountDaysByHashMapOnePass(tasks, space);

        Assert.Equal(expected, actual);
    }
}
