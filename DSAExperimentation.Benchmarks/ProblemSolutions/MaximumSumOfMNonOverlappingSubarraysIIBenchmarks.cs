using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumOfMNonOverlappingSubarraysIISolution's,
// the same methods MaximumSumOfMNonOverlappingSubarraysIITests proves correct.
// The DynamicProgramming baseline is O(n*m*(r-l+1)) - fine at these modest
// sizes, but nowhere near LC's own n <= 1e5, m <= n ceiling, which is exactly
// why LagrangianRelaxation's O(n*log(PenaltyBound)) arm exists.
[MemoryDiagnoser]
public class MaximumSumOfMNonOverlappingSubarraysIIBenchmarks
{
    private const int Seed = 3957; private int[] _nums = [];

    private int _m;
    private int _l;
    private int _r;
    // LC problem number

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(-50, 51))];
        _m = Math.Max(1, Length / 4);
        _l = 3;
        _r = 8;
    }

    [Benchmark(Baseline = true)]
    public long DynamicProgramming() =>
        MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByDynamicProgramming(_nums, _m, _l, _r);

    [Benchmark]
    public long LagrangianRelaxation() =>
        MaximumSumOfMNonOverlappingSubarraysIISolution.MaximumSumByLagrangianRelaxation(_nums, _m, _l, _r);
}
