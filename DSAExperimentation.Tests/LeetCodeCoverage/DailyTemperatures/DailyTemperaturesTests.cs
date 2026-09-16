using DSAExperimentation.LeetCode.DailyTemperatures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DailyTemperatures;

// Harness only. Both strategies are DailyTemperaturesSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class DailyTemperaturesTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [73, 74, 75, 71, 69, 72, 76, 73], [1, 1, 4, 2, 1, 1, 0, 0] },
            { [80, 70, 60, 50], [0, 0, 0, 0] },
            { [60], [0] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaitDaysByBruteForceScan_LeetCodeExamples_ReturnsDaysUntilWarmer(
        int[] temperatures, int[] expected) =>
        Assert.Equal(expected, DailyTemperaturesSolution.WaitDaysByBruteForceScan(temperatures));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WaitDaysByMonotonicStackSweep_LeetCodeExamples_ReturnsDaysUntilWarmer(
        int[] temperatures, int[] expected) =>
        Assert.Equal(expected, DailyTemperaturesSolution.WaitDaysByMonotonicStackSweep(temperatures));
}
