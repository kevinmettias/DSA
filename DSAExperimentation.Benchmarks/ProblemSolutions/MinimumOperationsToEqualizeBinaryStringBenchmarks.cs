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

    private string _binary = "";

    private int _flipCount;
    private int _zeroCount;
    private EqualizeStateGraph _graph = null!;
    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_binary, _flipCount) = MinimumOperationsToEqualizeBinaryStringWorkloads.Build(Length, seed: Seed);
        _zeroCount = _binary.Count(c => c == '0');
        _graph = EqualizeStateGraph.Build(Length, _flipCount);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByMutationQueue(_binary, _flipCount);

    [Benchmark]
    public int ReduceGraphBfs() =>
        MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByReduceGraph(_graph, _zeroCount);
}
