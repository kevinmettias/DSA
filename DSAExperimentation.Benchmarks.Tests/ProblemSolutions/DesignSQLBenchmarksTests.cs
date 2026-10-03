using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSQLBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: every cell
// the replay reads back, known from Setup's construction rather than from either arm. Setup builds the rows and the
// ids to delete from RowCount alone, and each arm returns every cell SelectCell read, in order.
public sealed partial class DesignSQLBenchmarksTests
{
    private const int SmallestRowCount = 500;

    [Fact]
    public void ListScanTable_InsertReadDeleteScript_ReadsBackTheFirstColumnOfEveryRow() =>
        Assert.Equal(ExpectedCells(SmallestRowCount), BuildHarness().ListScanTable());

    [Fact]
    public void HashMapTable_InsertReadDeleteScript_ReadsBackTheFirstColumnOfEveryRow() =>
        Assert.Equal(ExpectedCells(SmallestRowCount), BuildHarness().HashMapTable());

    private static DesignSQLBenchmarks BuildHarness()
    {
        var harness = new DesignSQLBenchmarks { RowCount = SmallestRowCount };
        harness.Setup();

        return harness;
    }

    // The replay inserts a row per index, then reads column 1 of every row id in insertion order, then
    // deletes the even ones. Row id n was inserted as "r{n - 1}c{column}" with columns counted from 0,
    // while LC 2408 counts column ids from 1, so only a table handing back the row it was given reads
    // "r{n - 1}c0" for every n from 1 through RowCount.
    private static IEnumerable<string> ExpectedCells(int rowCount) =>
        Enumerable.Range(0, rowCount).Select(index => $"r{index}c0");
}
