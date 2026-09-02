using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfOperationsWithTheSameScoreII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfOperationsWithTheSameScoreIISolution's,
// the same methods MaximumNumberOfOperationsWithTheSameScoreIITests proves correct.
// Uniform random values over a narrow range keep front/back-pair sum collisions
// frequent, so both interval searches actually branch through all three moves
// instead of the target score being unreachable past the first operation.
[MemoryDiagnoser]
public class MaximumNumberOfOperationsWithTheSameScoreIIBenchmarks
{
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 3040;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDp() => MaximumNumberOfOperationsWithTheSameScoreIISolution.MaxOperationsByBruteForceDp(_nums);

    [Benchmark]
    public int MemoizedTwoPointer() =>
        MaximumNumberOfOperationsWithTheSameScoreIISolution.MaxOperationsByMemoizedTwoPointer(_nums);
}
