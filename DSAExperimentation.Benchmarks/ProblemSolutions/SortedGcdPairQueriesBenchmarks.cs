using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortedGcdPairQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortedGcdPairQueriesSolution's, the same methods
// SortedGcdPairQueriesTests proves correct. The sieve arm is handed a prebuilt
// GcdPairCountIndex, so its O(maxValue log maxValue) build cost is charged to
// [GlobalSetup] rather than to the queries being measured.
[MemoryDiagnoser]
public class SortedGcdPairQueriesBenchmarks
{
    private const int MaxValue = 500;
    private const int QueryCount = 200;
    private const int Seed = 3312;

    private int[] _nums = [];

    private int[] _queries = [];
    private GcdPairCountIndex _index = null!;
    [Params(50, 300)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValue + 1)).ToArray();

        var pairCount = Length * (Length - 1) / 2;
        _queries = Enumerable.Range(0, QueryCount).Select(_ => random.Next(pairCount)).ToArray();

        _index = GcdPairCountIndex.Build(_nums);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() => SortedGcdPairQueriesSolution.AnswerQueriesByBruteForce(_nums, _queries);

    [Benchmark]
    public int[] GcdCountingSieve() => SortedGcdPairQueriesSolution.AnswerQueriesByGcdCountingSieve(_index, _queries);
}
