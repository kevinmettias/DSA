using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignSQLSolution's, the same classes DesignSQLSolutionTests
// proves correct. [GlobalSetup] builds the rows and the ids to delete, so workload
// construction is charged to setup rather than to the replay each arm measures;
// there is no prepared input to hoist into a strategy overload, because a Design
// problem's input is the call script itself.
//
// Every inserted row is later selected once and half of them deleted, so both
// strategies pay for a full read/write workload rather than an early-exit best
// case. The list-scan table linear-scans for a matching row id on every select and
// every delete; the HashMap table indexes straight to it - the same
// list-scan-vs-hash-lookup shape DesignANumberContainerSystemBenchmarks exercises.
public class DesignSQLBenchmarks
{
    private const int ColumnCount = 3;
    private const string TableName = "rows";

    private string[] _names = [];

    private int[] _columns = [];
    private string[][] _rows = [];
    private int[] _deleteIds = [];

    // Every cell SelectCell reads, in call order; sized in setup so the replay allocates nothing.
    private string[] _cells = [];
    [Params(500, 4_000)]
    public int RowCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _names = [TableName];
        _columns = [ColumnCount];
        _rows = Enumerable.Range(0, RowCount)
            .Select(i => Enumerable.Range(0, ColumnCount).Select(c => $"r{i}c{c}").ToArray())
            .ToArray();
        _deleteIds = Enumerable.Range(1, RowCount).Where(id => id % 2 == 0).ToArray();
        _cells = new string[_rows.Length];
    }

    [Benchmark(Baseline = true)]
    public string[] ListScanTable() => Replay(new DesignSQLSolution.SqlByListScan(_names, _columns));

    [Benchmark]
    public string[] HashMapTable() => Replay(new DesignSQLSolution.SqlByHashMapTables(_names, _columns));

    // Returns every cell read, in order, so the JIT cannot eliminate the replay as
    // dead code.
    private string[] Replay(DesignSQLSolution.ISqlStrategy sql)
    {
        foreach (var row in _rows)
        {
            sql.InsertRow(TableName, row);
        }

        for (var rowId = 1; rowId <= _rows.Length; rowId++)
        {
            _cells[rowId - 1] = sql.SelectCell(TableName, rowId, 1);
        }

        foreach (var rowId in _deleteIds)
        {
            sql.DeleteRow(TableName, rowId);
        }

        return _cells;
    }
}
