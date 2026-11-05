using DSAExperimentation.LeetCode.Solutions.SelectCellsInGridWithMaximumScore;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SelectCellsInGridWithMaximumScore;

// Harness only: both strategies are SelectCellsInGridWithMaximumScoreSolution's -
// this file just pins them to LeetCode's published examples.
public sealed partial class SelectCellsInGridWithMaximumScoreSolutionTests
{
    // LeetCode example 1, whose last row repeats one value: a row is listed once per value
    // it holds, however often it holds it, because the strategy may take one cell per row.
    [Fact]
    public void RowsHoldingEachValue_RowRepeatingAValue_ListsThatRowOnceForIt()
    {
        var rowsByValue = SelectCellsInGridWithMaximumScoreSolution.RowsHoldingEachValue([[1, 2, 3], [4, 3, 2], [1, 1, 1]]);

        Assert.Equal(
            ["1:0,2", "2:0,1", "3:0,1", "4:1"],
            rowsByValue.OrderBy(entry => entry.Key).Select(entry => $"{entry.Key}:{string.Join(',', entry.Value)}"));
    }
}
