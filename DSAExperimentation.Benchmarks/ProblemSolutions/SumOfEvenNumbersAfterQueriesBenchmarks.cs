using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfEvenNumbersAfterQueries;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfEvenNumbersAfterQueriesSolution's, the same
// strategies SumOfEvenNumbersAfterQueriesTests proves correct. Setup builds Length
// random values and Length random queries from a fixed seed, so the rescanning arm
// pays O(n) per query against the running-invariant arm's O(1).
[MemoryDiagnoser]
public class SumOfEvenNumbersAfterQueriesBenchmarks
{
    private const int RandomSeed = 985; // LC problem number
    private const int ValueMagnitudeBound = 1_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitudeBound, ValueMagnitudeBound)).ToArray();
        _queries = Enumerable.Range(0, Length)
            .Select(_ => new[] { random.Next(-ValueMagnitudeBound, ValueMagnitudeBound), random.Next(0, Length) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RescanAfterEveryQuery() =>
        SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRescan(_nums, _queries);

    [Benchmark]
    public int[] RunningEvenSum() =>
        SumOfEvenNumbersAfterQueriesSolution.SumEvenAfterQueriesByRunningEvenSum(_nums, _queries);
}
