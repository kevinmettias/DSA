using DSAExperimentation.LeetCode.Solutions.DailyTemperatures;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DailyTemperatures;

// Harness only. Both strategies are DailyTemperaturesSolution's - this file pins
// them to two cases: a strictly falling run, where no day has a warmer one ahead of
// it, and a single day. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class DailyTemperaturesSolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            { [80, 70, 60, 50], [0, 0, 0, 0] },
            { [60], [0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaitDaysByBruteForceScan_Cases_ReturnsDaysUntilWarmer(
        int[] temperatures, int[] expected) =>
        Assert.Equal(expected, DailyTemperaturesSolution.WaitDaysByBruteForceScan(temperatures));

    [Theory]
    [MemberData(nameof(Cases))]
    public void WaitDaysByMonotonicStackSweep_Cases_ReturnsDaysUntilWarmer(
        int[] temperatures, int[] expected) =>
        Assert.Equal(expected, DailyTemperaturesSolution.WaitDaysByMonotonicStackSweep(temperatures));
}
