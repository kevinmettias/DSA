using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Sum Query 2D - Immutable (LC 304): a brute-force cell scan baseline (sums every cell in
// the region directly, O(rows*cols) per SumRegion call) vs. one FenwickTree<int,SumOperation<int>>
// built per row (O(rows*cols*log(cols)) once), so SumRegion afterward walks only the covered rows
// and does an O(log cols) FenwickTree.Query per row instead of rescanning every cell. Both answer
// the same fixed batch of queries over a square matrix.
[MemoryDiagnoser]
public class RangeSumQuery2DImmutableBenchmarks
{
    private const int QueryCount = 200;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 304;

    private const int CellValueRange = 1_000;

    [Params(20, 200)]
    public int Size;

    private int[][] _matrix = null!;
    private (int Row1, int Col1, int Row2, int Col2)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(-CellValueRange, CellValueRange)).ToArray())
            .ToArray();

        _queries = new (int Row1, int Col1, int Row2, int Col2)[QueryCount];
        for (var i = 0; i < QueryCount; i++)
        {
            var row1 = random.Next(0, Size);
            var row2 = random.Next(row1, Size);
            var col1 = random.Next(0, Size);
            var col2 = random.Next(col1, Size);
            _queries[i] = (row1, col1, row2, col2);
        }
    }

    [Benchmark(Baseline = true)]
    public long BruteForceCellScan()
    {
        var total = 0L;

        foreach (var (row1, col1, row2, col2) in _queries)
        {
            for (var row = row1; row <= row2; row++)
            {
                for (var col = col1; col <= col2; col++)
                {
                    total += _matrix[row][col];
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long RowFenwickTreeQuery()
    {
        var rows = _matrix.Select(row => new FenwickTree<int, SumOperation<int>>(row)).ToArray();
        var total = 0L;

        foreach (var (row1, col1, row2, col2) in _queries)
        {
            for (var row = row1; row <= row2; row++)
            {
                total += rows[row].Query(col1, col2);
            }
        }

        return total;
    }
}
