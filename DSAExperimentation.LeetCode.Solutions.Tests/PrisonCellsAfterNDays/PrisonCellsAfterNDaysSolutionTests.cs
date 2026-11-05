using DSAExperimentation.LeetCode.Solutions.PrisonCellsAfterNDays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PrisonCellsAfterNDays;

// Harness only: both strategies live in PrisonCellsAfterNDaysSolution. One test
// method per strategy over one shared set of cases, so a failure names the
// strategy that broke - which now includes the day-by-day simulation the
// benchmark used as its untested baseline arm. The cases cover the cycle: zero
// days, the first transition, and day counts far enough along that the cycle
// length decides the answer. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class PrisonCellsAfterNDaysSolutionTests
{
    public static TheoryData<int[], int, int[]> Cases =>
        new()
        {
            // LeetCode's second published example is this cell layout at
            // dayCount = 10^9, whose answer is [0,0,1,1,1,1,1,0]. Both arms share
            // these cases and the daily simulation really does walk every day, so
            // dayCount is stated as 1_000 - the same position in the 14-day cycle
            // (999 and 999_999_999 are both 5 mod 14), hence the same published
            // answer, without charging the baseline a billion iterations.
            { [1, 0, 0, 1, 0, 0, 1, 0], 1_000, [0, 0, 1, 1, 1, 1, 1, 0] },
            // Zero days is the one case with no transition at all: the two end
            // cells are still whatever the caller passed in, rather than vacant.
            { [0, 1, 0, 1, 1, 0, 0, 1], 0, [0, 1, 0, 1, 1, 0, 0, 1] },
            // One day is the first transition, where both ends go vacant.
            { [0, 1, 0, 1, 1, 0, 0, 1], 1, [0, 1, 1, 0, 0, 0, 0, 0] },
            // Far enough past the first repeat that a wrong cycle length shows up
            // as a wrong answer rather than an accidentally aligned one.
            { [1, 0, 0, 1, 0, 0, 1, 0], 1_000_000, [0, 1, 0, 0, 1, 0, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CellsAfterNDaysByDailySimulation_Cases_ReturnsCellsOnDayN(
        int[] cells, int dayCount, int[] expected)
    {
        var actual = PrisonCellsAfterNDaysSolution.CellsAfterNDaysByDailySimulation(cells, dayCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CellsAfterNDaysByCycleDetection_Cases_ReturnsCellsOnDayN(
        int[] cells, int dayCount, int[] expected)
    {
        var actual = PrisonCellsAfterNDaysSolution.CellsAfterNDaysByCycleDetection(cells, dayCount);

        Assert.Equal(expected, actual);
    }
}
