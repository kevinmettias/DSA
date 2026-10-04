using System.Globalization;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSQLBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: every answer
// the script observes, derived from the script's construction rather than from either arm, at both sizes. The script
// (see the benchmark's comment) refuses two inserts up front, fills ids 1 through RowCount exporting after every 50th,
// removes the even ids, sweeps sel over row ids 0 through RowCount + 1, then inserts once more and exports.
public sealed partial class DesignSQLBenchmarksTests
{
    private const int SmallestRowCount = 500;

    // The largest RowCount whose RowCount + 3 ins and RowCount / 2 + 3 rmv calls stay inside LC 2408's 2000.
    private const int LargestRowCount = 1_328;

    private const int ColumnCount = 3;
    private const int ExportStride = 50;
    private const int CellProbeCount = 9_999;

    // Column ids 0 through 4: a sweep reads one of them across every row id.
    private const int ProbedColumnCount = 5;

    // A sweep reads row ids 0 through rowCount + 1: the fill's ids and one id past each end.
    private const int RowIdsPastTheFill = 2;

    // The replay removes every second id, starting at 2, so the odd ids are the ones left.
    private const int RemovalStride = 2;

    private const string NoCell = "<null>";
    private const string RowPrefix = "r";
    private const string ColumnPrefix = "c";
    private const string CellSeparator = ",";

    // The smallest size, which BenchmarkArmsTests also runs, and the largest, which no other test does.
    public static TheoryData<int> RowCounts => new() { SmallestRowCount, LargestRowCount };

    [Theory]
    [MemberData(nameof(RowCounts))]
    public void ListScanTable_FillRemoveProbeScript_AnswersWhatTheScriptImplies(int rowCount)
    {
        var (inserted, selected, exported) = BuildHarness(rowCount).ListScanTable();

        Assert.Equal(ExpectedInserted(rowCount), inserted);
        Assert.Equal(ExpectedSelected(rowCount), selected);
        Assert.Equal(ExpectedExported(rowCount), exported);
    }

    [Theory]
    [MemberData(nameof(RowCounts))]
    public void HashMapTable_FillRemoveProbeScript_AnswersWhatTheScriptImplies(int rowCount)
    {
        var (inserted, selected, exported) = BuildHarness(rowCount).HashMapTable();

        Assert.Equal(ExpectedInserted(rowCount), inserted);
        Assert.Equal(ExpectedSelected(rowCount), selected);
        Assert.Equal(ExpectedExported(rowCount), exported);
    }

    private static DesignSQLBenchmarks BuildHarness(int rowCount)
    {
        var harness = new DesignSQLBenchmarks { RowCount = rowCount };
        harness.Setup();

        return harness;
    }

    // The ins on the missing table and the two-cell row are refused; every other ins - the fill's and the one after
    // the removals - fits the three-column table and succeeds.
    private static bool[] ExpectedInserted(int rowCount) => [false, false, .. Enumerable.Repeat(true, rowCount + 1)];

    // The sel on the missing table, then probe p: row id p mod (rowCount + 2) at column (p div (rowCount + 2)) mod 5.
    private static string[] ExpectedSelected(int rowCount)
    {
        var sweepLength = rowCount + RowIdsPastTheFill;
        var probes = Enumerable.Range(0, CellProbeCount)
            .Select(probe => CellAfterRemovals(rowCount, probe % sweepLength, probe / sweepLength % ProbedColumnCount));

        return [NoCell, .. probes];
    }

    // When the probes run, the live rows are the odd ids through rowCount - the evens were removed and id rowCount + 1
    // is not issued until after them - and the column ids are 1 through 3.
    private static string CellAfterRemovals(int rowCount, int rowId, int columnId)
    {
        var rowIsLive = rowId % RemovalStride == 1 && rowId <= rowCount;
        var columnExists = columnId >= 1 && columnId <= ColumnCount;

        return rowIsLive && columnExists ? Cell(rowId, columnId) : NoCell;
    }

    // The missing table exports nothing; the export after the m-th 50 inserts lists ids 1 through 50m, none yet
    // removed; the final one lists the odd ids through rowCount, then rowCount + 1 - the next id after rowCount,
    // since neither refused insert used one up.
    private static string[][] ExpectedExported(int rowCount)
    {
        var duringFill = Enumerable.Range(1, rowCount / ExportStride)
            .Select(exportCount => LinesFor(FirstRowIds(exportCount * ExportStride)));
        var oddRowIds = FirstRowIds(rowCount).Where(rowId => rowId % RemovalStride == 1);

        return [[], .. duringFill, LinesFor(oddRowIds.Append(rowCount + 1))];
    }

    // 1 through count, the ids a table issues first.
    private static IEnumerable<int> FirstRowIds(int count) => Enumerable.Range(1, count);

    private static string[] LinesFor(IEnumerable<int> rowIds) => [.. rowIds.Select(Line)];

    private static string Line(int rowId)
    {
        var cells = Enumerable.Range(1, ColumnCount).Select(columnId => Cell(rowId, columnId));

        return string.Join(CellSeparator, cells.Prepend(rowId.ToString(CultureInfo.InvariantCulture)));
    }

    // Mirrors the benchmark's cell spelling: "r", the row id less one, "c", the column id less one, each spelled by
    // LowercaseNames.Of.
    private static string Cell(int rowId, int columnId) =>
        RowPrefix + LowercaseNames.Of(rowId - 1) + ColumnPrefix + LowercaseNames.Of(columnId - 1);
}
