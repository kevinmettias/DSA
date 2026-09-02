using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design SQL (LC 2408): a table implemented as a plain List<(int Id, string[] Row)>,
// where deleteRow/selectCell both linear-scan for the matching row id, vs. this
// repo's own HashMap<int,string[]> keyed directly by row id - the same
// list-scan-vs-hash-lookup shape DesignANumberContainerSystemBenchmarks already
// exercises. Every inserted row is later selected once and half of them deleted, so
// both strategies pay for a full read/write workload rather than an early-exit best
// case.
[MemoryDiagnoser]
public class DesignSQLBenchmarks
{
    private const int ColumnCount = 3;

    [Params(500, 4_000)]
    public int RowCount;

    private string[][] _rows = null!;
    private int[] _deleteIds = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rows = Enumerable.Range(0, RowCount)
            .Select(i => Enumerable.Range(0, ColumnCount).Select(c => $"r{i}c{c}").ToArray())
            .ToArray();
        _deleteIds = Enumerable.Range(1, RowCount).Where(id => id % 2 == 0).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ListScanTable()
    {
        var table = new List<(int Id, string[] Row)>();
        var nextId = 1;

        foreach (var row in _rows)
        {
            table.Add((nextId++, row));
        }

        long total = 0;

        for (var id = 1; id <= _rows.Length; id++)
        {
            total += SelectCell(table, id, 1).Length;
        }

        foreach (var id in _deleteIds)
        {
            DeleteRow(table, id);
        }

        return total;
    }

    private static string SelectCell(List<(int Id, string[] Row)> table, int rowId, int columnId)
    {
        foreach (var (id, row) in table)
        {
            if (id == rowId)
            {
                return row[columnId - 1];
            }
        }

        return string.Empty;
    }

    private static void DeleteRow(List<(int Id, string[] Row)> table, int rowId)
    {
        for (var i = 0; i < table.Count; i++)
        {
            if (table[i].Id == rowId)
            {
                table.RemoveAt(i);
                return;
            }
        }
    }

    [Benchmark]
    public long HashMapTable()
    {
        var table = new HashMap<int, string[]>();
        var nextId = 1;

        foreach (var row in _rows)
        {
            table.Set(nextId++, row);
        }

        long total = 0;

        for (var id = 1; id <= _rows.Length; id++)
        {
            table.TryGetValue(id, out var row);
            total += row[0].Length;
        }

        foreach (var id in _deleteIds)
        {
            table.TryRemove(id);
        }

        return total;
    }
}
