using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeSumQuery2DImmutableSolution's, the same factories
// RangeSumQuery2DImmutableTests proves correct - a brute-force cell scan baseline (sums every
// cell in the region directly, O(rows*cols) per SumRegion call) vs. one
// FenwickTree<int,SumOperation<int>> built per row (O(rows*cols*log(cols)) once), so SumRegion
// afterward walks only the covered rows and does an O(log cols) FenwickTree.Query per row instead
// of rescanning every cell. [GlobalSetup] builds the random matrix and query batch; each
// [Benchmark] arm's own factory call (construction included) plus the full query replay is what
// gets measured, the same shape LRUCacheBenchmarks uses for its own design problem.
[MemoryDiagnoser]
public class RangeSumQuery2DImmutableBenchmarks
{
    private const int QueryCount = 200;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 304;

    private const int CellValueRange = 1_000;

    private int[][] _matrix = [];

    private (int Row1, int Col1, int Row2, int Col2)[] _queries = [];
    [Params(20, 200)]
    public int Size { get; set; }

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
    public long BruteForceCellScan() => Replay(RangeSumQuery2DImmutableSolution.CreateByBruteForceCellScan(_matrix));

    [Benchmark]
    public long RowFenwickTreeQuery() => Replay(RangeSumQuery2DImmutableSolution.CreateByRowFenwickTree(_matrix));

    // Sums every returned region sum rather than discarding it, so the JIT can't eliminate the
    // replay as dead code - the same "return the real answer, not a weaker proxy" shape
    // OpenTheLockBenchmarks/LRUCacheBenchmarks already follow.
    private long Replay(INumMatrix numMatrix)
    {
        var total = 0L;

        foreach (var (row1, col1, row2, col2) in _queries)
        {
            total += numMatrix.SumRegion(row1, col1, row2, col2);
        }

        return total;
    }
}
