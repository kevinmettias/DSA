using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RangeSumQuery2DImmutable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are RangeSumQuery2DImmutableSolution's, the same factories
// RangeSumQuery2DImmutableSolutionTests proves correct - a brute-force cell scan baseline (O(rows *
// cols) per SumRegion call), one FenwickTree<int,SumOperation<int>> per row (an O(log cols) query per
// covered row), and the prefix-sum table LeetCode's O(1) requirement asks for (four lookups per
// call). [GlobalSetup] builds the random matrix and query batch; each arm's own factory call
// (construction included) plus the full query replay is what gets measured, the same shape
// LRUCacheBenchmarks uses for its own design problem.
public class RangeSumQuery2DImmutableBenchmarks
{
    private const int QueryCount = 200;

    // LC problem number, reused as the Random seed for reproducible benchmark input.
    private const int RandomSeed = 304;

    private const int CellValueRange = 1_000;

    private int[][] _matrix = [];

    private (int Row1, int Col1, int Row2, int Col2)[] _queries = [];

    // Every SumRegion answer, in query order - what each arm returns.
    private int[] _sums = [];

    [Params(20, 200)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, -CellValueRange, CellValueRange, random))
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

        _sums = new int[QueryCount];
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForceCellScan() => Replay(RangeSumQuery2DImmutableSolution.CreateByBruteForceCellScan(_matrix));

    [Benchmark]
    public int[] RowFenwickTreeQuery() => Replay(RangeSumQuery2DImmutableSolution.CreateByRowFenwickTree(_matrix));

    [Benchmark]
    public int[] PrefixSums() => Replay(RangeSumQuery2DImmutableSolution.CreateByPrefixSums(_matrix));

    private int[] Replay(INumMatrix numMatrix)
    {
        for (var i = 0; i < _queries.Length; i++)
        {
            var (row1, col1, row2, col2) = _queries[i];
            _sums[i] = numMatrix.SumRegion(row1, col1, row2, col2);
        }

        return _sums;
    }
}
