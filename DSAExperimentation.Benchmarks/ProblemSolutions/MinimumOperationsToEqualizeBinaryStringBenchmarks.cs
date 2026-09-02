using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumOperationsToEqualizeBinaryStringSolution's,
// the same methods MinimumOperationsToEqualizeBinaryStringTests proves correct.
// ReduceGraph takes the prepared overload - an EqualizeStateGraph built once in
// [GlobalSetup] - so graph construction is charged to setup rather than to the
// search being measured, the same split OpenTheLockBenchmarks uses for LockGraph.
[MemoryDiagnoser]
public class MinimumOperationsToEqualizeBinaryStringBenchmarks
{
    private const int Seed = 3666;

    [Params(200, 1_000)]
    public int Length;

    private string _s = null!;
    private int _k;
    private int _zeroCount;
    private EqualizeStateGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_s, _k) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, seed: Seed);
        _zeroCount = _s.Count(c => c == '0');
        _graph = EqualizeStateGraph.Build(Length, _k);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByMutationQueue(_s, _k);

    [Benchmark]
    public int ReduceGraphBfs() =>
        MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByReduceGraph(_graph, _zeroCount);
}
