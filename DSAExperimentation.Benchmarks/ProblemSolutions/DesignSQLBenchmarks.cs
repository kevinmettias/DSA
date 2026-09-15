using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignSQL;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignSQLSolution's, the same classes DesignSQLTests
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
[MemoryDiagnoser]
public class DesignSQLBenchmarks
{
    private const int ColumnCount = 3;
    private const string TableName = "rows";

    private string[] _names = [];

    private int[] _columns = [];
    private string[][] _rows = [];
    private int[] _deleteIds = [];
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
    }

    [Benchmark(Baseline = true)]
    public long ListScanTable() => Replay(new DesignSQLSolution.SqlByListScan(_names, _columns));

    [Benchmark]
    public long HashMapTable() => Replay(new DesignSQLSolution.SqlByHashMapTables(_names, _columns));

    // Sums the length of every cell read rather than discarding it, so the JIT
    // cannot eliminate the replay as dead code.
    private long Replay(DesignSQLSolution.ISqlStrategy sql)
    {
        foreach (var row in _rows)
        {
            sql.InsertRow(TableName, row);
        }

        var readLength = 0L;

        for (var rowId = 1; rowId <= _rows.Length; rowId++)
        {
            readLength += sql.SelectCell(TableName, rowId, 1).Length;
        }

        foreach (var rowId in _deleteIds)
        {
            sql.DeleteRow(TableName, rowId);
        }

        return readLength;
    }
}
