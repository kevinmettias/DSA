using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignSQLSolution's, the same classes DesignSQLSolutionTests
// proves correct. [GlobalSetup] builds the rows, the misfit row and the cell probes, so
// workload construction is charged to setup rather than to the replay each arm measures;
// there is no prepared input to hoist into a strategy overload, because a Design
// problem's input is the call script itself.
//
// One table, "rows", of three columns, and one name, "none", that is not a table. The
// script, in order:
//   1. Calls LeetCode answers without a row: ins on "none" and ins of a two-cell row on
//      "rows" (both refused, neither using up an id), rmv, sel and exp on "none".
//   2. RowCount inserts, with an exp of "rows" after every ExportStride-th.
//   3. rmv of every even id, then of id 2 again and of id RowCount + 1, never issued.
//   4. CellProbeCount sels sweeping row ids 0 through RowCount + 1, sweep s reading
//      column s mod 5 - columns 0 and 4 never exist, nor do rows 0, RowCount + 1 and the
//      even ids - so the list scan pays a walk of the table on every probe, a miss
//      walking all of it, where the HashMap table pays one lookup.
//   5. One more insert, which gets id RowCount + 1, and a final exp.
// Each arm returns every ins answer, every sel answer and every exp answer, each in call
// order, written into buffers sized in setup; rmv answers nothing, and the sels and the
// final exp are what observe it.
//
// LC 2408 allows 2000 ins and rmv calls together, 10^4 sel and 500 exp. The script makes
// RowCount + 3 ins and RowCount / 2 + 3 rmv calls, so 1328 is the largest even RowCount;
// 1 + CellProbeCount sels, exactly 10^4; and RowCount / ExportStride + 2 exps, 28 at most.
// Names and cells are lowercase letters, at most 10 of them: the cell at row id i and
// column c is spelled "r", then i - 1 and "c", then c - 1, both through LowercaseNames.
public class DesignSQLBenchmarks
{
    private const int ColumnCount = 3;
    private const string TableName = "rows";
    private const string MissingTableName = "none";

    private const string RowPrefix = "r";
    private const string ColumnPrefix = "c";

    private const int FirstId = 1;
    private const int ExportStride = 50;

    // Every second id is removed, starting at the first even one, so the odd ids are left.
    private const int RemovalStride = 2;

    // The answers outside the fill and the probes: the two refused inserts before the fill
    // and the one insert after the removals; the sel on the missing table; the export of
    // the missing table and the final export.
    private const int RefusedInsertCount = 2;
    private const int InsertsAfterRemovals = 1;
    private const int MissingTableSelects = 1;
    private const int ExportsOutsideTheFill = 2;

    // With the one sel on the missing table, LC 2408's 10^4 sel calls.
    private const int CellProbeCount = 9_999;

    // A sweep reads row ids 0 through RowCount + 1: the fill's ids and one id past each end.
    private const int RowIdsPastTheFill = 2;

    // Column ids 0 through ColumnCount + 1, so each sweep's column either exists or misses by one.
    private const int ProbedColumnCount = ColumnCount + 2;

    private string[] _names = [];
    private int[] _columns = [];

    // The row inserted as id i sits at index i - 1, for ids 1 through RowCount + 1.
    private string[][] _rows = [];
    private string[] _misfitRow = [];
    private (int RowId, int ColumnId)[] _cellProbes = [];

    // Every ins, sel and exp answer, in call order; sized in setup so the replay allocates
    // nothing but what the strategies themselves allocate.
    private bool[] _inserted = [];
    private string[] _selected = [];
    private string[][] _exported = [];

    [Params(500, 1_328)]
    public int RowCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _names = [TableName];
        _columns = [ColumnCount];
        _rows = [.. Enumerable.Range(FirstId, RowCount + InsertsAfterRemovals).Select(RowOf)];
        _misfitRow = [.. _rows[0].Take(ColumnCount - 1)];
        _cellProbes = [.. Enumerable.Range(0, CellProbeCount).Select(ProbeOf)];
        _inserted = new bool[RefusedInsertCount + RowCount + InsertsAfterRemovals];
        _selected = new string[MissingTableSelects + CellProbeCount];
        _exported = new string[(RowCount / ExportStride) + ExportsOutsideTheFill][];
    }

    private static string[] RowOf(int rowId) =>
        [.. Enumerable.Range(FirstId, ColumnCount).Select(columnId => CellText(rowId, columnId))];

    private static string CellText(int rowId, int columnId) =>
        RowPrefix + LowercaseNames.Of(rowId - FirstId) + ColumnPrefix + LowercaseNames.Of(columnId - FirstId);

    // Probe p reads row id p mod (RowCount + 2) - 0 through RowCount + 1 - at the column its
    // sweep is on.
    private (int RowId, int ColumnId) ProbeOf(int probe)
    {
        var sweepLength = RowCount + RowIdsPastTheFill;
        var sweep = probe / sweepLength;

        return (probe % sweepLength, sweep % ProbedColumnCount);
    }

    [Benchmark(Baseline = true)]
    public (bool[] Inserted, string[] Selected, string[][] Exported) ListScanTable() =>
        Replay(new DesignSQLSolution.SqlByListScan(_names, _columns));

    [Benchmark]
    public (bool[] Inserted, string[] Selected, string[][] Exported) HashMapTable() =>
        Replay(new DesignSQLSolution.SqlByHashMapTables(_names, _columns));

    private (bool[] Inserted, string[] Selected, string[][] Exported) Replay(DesignSQLSolution.ISqlStrategy sql)
    {
        CallWithoutARow(sql);
        FillAndExport(sql);
        RemoveEvenRows(sql);
        ProbeCells(sql);

        _inserted[^1] = sql.TryInsert(TableName, _rows[^1]);
        _exported[^1] = sql.Export(TableName);

        return (_inserted, _selected, _exported);
    }

    private void CallWithoutARow(DesignSQLSolution.ISqlStrategy sql)
    {
        _inserted[0] = sql.TryInsert(MissingTableName, _rows[0]);
        _inserted[1] = sql.TryInsert(TableName, _misfitRow);
        sql.Remove(MissingTableName, FirstId);
        _selected[0] = sql.Select(MissingTableName, FirstId, FirstId);
        _exported[0] = sql.Export(MissingTableName);
    }

    private void FillAndExport(DesignSQLSolution.ISqlStrategy sql)
    {
        for (var rowId = FirstId; rowId <= RowCount; rowId++)
        {
            _inserted[RefusedInsertCount + rowId - FirstId] = sql.TryInsert(TableName, _rows[rowId - FirstId]);

            if (rowId % ExportStride == 0)
            {
                _exported[rowId / ExportStride] = sql.Export(TableName);
            }
        }
    }

    private void RemoveEvenRows(DesignSQLSolution.ISqlStrategy sql)
    {
        for (var rowId = RemovalStride; rowId <= RowCount; rowId += RemovalStride)
        {
            sql.Remove(TableName, rowId);
        }

        // Two removals that find nothing: the first even id, gone already, and the id the
        // insert after the removals will get, not issued yet.
        sql.Remove(TableName, RemovalStride);
        sql.Remove(TableName, RowCount + InsertsAfterRemovals);
    }

    private void ProbeCells(DesignSQLSolution.ISqlStrategy sql)
    {
        for (var probe = 0; probe < _cellProbes.Length; probe++)
        {
            var (rowId, columnId) = _cellProbes[probe];
            _selected[MissingTableSelects + probe] = sql.Select(TableName, rowId, columnId);
        }
    }
}
