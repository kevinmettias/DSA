using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfPointsFromGridQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfPointsFromGridQueriesSolution's, the
// same methods MaximumNumberOfPointsFromGridQueriesTests proves correct. The
// problem's own per-query re-simulation (a fresh 4-directional flood fill from
// (0,0), bounded by that query's threshold, for every query independently -
// O(QueriesCount * Rows*Cols)) against this repo's single shared flood fill, which
// sorts queries ascending and drains a min-heap frontier exactly once, in
// O(Rows*Cols*log(Rows*Cols) + QueriesCount*log QueriesCount) total. The grid is
// fixed-size; only QueriesCount grows, so the baseline's cost scales linearly with
// it while the heap-based one barely moves - the same "one shared pass amortizes
// across many queries" shape ReconstructItineraryBenchmarks and
// DeleteGreatestValueInEachRowBenchmarks already demonstrate for their own problems.
//
// [GlobalSetup] builds the grid and the query list, which is exactly the argument
// shape both strategies take, so no §17.4 hoisted overload applies here.
[MemoryDiagnoser]
public class MaximumNumberOfPointsFromGridQueriesBenchmarks
{
    private const int RandomSeed = 2503;
    private const int Side = 60;
    private const int ValueBound = 1_000_000;

    [Params(50, 1_000)]
    public int QueriesCount;

    private int[][] _grid = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _grid = Enumerable.Range(0, Side)
            .Select(_ => Enumerable.Range(0, Side).Select(_ => random.Next(ValueBound)).ToArray())
            .ToArray();

        _queries = Enumerable.Range(0, QueriesCount).Select(_ => random.Next(ValueBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] FloodFillPerQuery() =>
        MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByFloodFillPerQuery(_grid, _queries);

    [Benchmark]
    public int[] MinHeapFloodFill() =>
        MaximumNumberOfPointsFromGridQueriesSolution.MaxPointsByMinHeapFrontier(_grid, _queries);
}
