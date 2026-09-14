using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueryKthSmallestTrimmedNumber;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueryKthSmallestTrimmedNumberSolution's, proved correct by
// QueryKthSmallestTrimmedNumberTests. The naive per-query "scan for the current smallest
// trimmed suffix, k times" baseline (O(n*k)) against sorting an index array once per query
// with this repo's own MergeSort (O(n log n)) and reading off position k directly. Both
// replay the identical randomly generated query batch.
[MemoryDiagnoser]
public class QueryKthSmallestTrimmedNumberBenchmarks
{
    private const int DigitLength = 5;
    private const int QueryCount = 8;
    private const int RandomSeed = 2343;

    [Params(200, 3_000)]
    public int Length;

    private string[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length)
            .Select(_ => string.Concat(Enumerable.Range(0, DigitLength).Select(_ => random.Next(0, 10))))
            .ToArray();

        _queries = Enumerable.Range(0, QueryCount)
            .Select(_ => new[] { random.Next(1, Length + 1), random.Next(1, DigitLength + 1) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] SelectionScanPerQuery()
        => QueryKthSmallestTrimmedNumberSolution.AnswerQueriesBySelectionScan(_nums, _queries);

    [Benchmark]
    public int[] MergeSortPerQuery()
        => QueryKthSmallestTrimmedNumberSolution.AnswerQueriesByMergeSort(_nums, _queries);
}
