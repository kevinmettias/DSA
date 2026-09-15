using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubarrayWithElementsGreaterThanVaryingThreshold;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// SubarrayWithElementsGreaterThanVaryingThresholdSolution's, the same methods
// SubarrayWithElementsGreaterThanVaryingThresholdTests proves correct. Threshold is
// set deliberately unreachable (larger than Length * MaxValue, the biggest
// length * value product either strategy could ever see) so both are forced through
// their full worst case instead of an early return on the first qualifying window
// making one look artificially fast - the same "force the real worst case" convention
// TwoSumBenchmarks/GraphConnectivityWithThresholdBenchmarks already use.
[MemoryDiagnoser]
public class SubarrayWithElementsGreaterThanVaryingThresholdBenchmarks
{
    private const int MaxValueExclusive = 1_000;

    // LC problem number, reused as the deterministic value seed.
    private const int ValueSeed = 2334;

    private int[] _nums = [];

    private int _threshold;
    [Params(500, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(ValueSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _threshold = Length * MaxValueExclusive + 1; // unreachable: no window can qualify
    }

    [Benchmark(Baseline = true)]
    public int WindowMinimumScan() =>
        SubarrayWithElementsGreaterThanVaryingThresholdSolution.ValidSubarraySizeByWindowMinimumScan(
            _nums, _threshold);

    [Benchmark]
    public int UnionFindOrder() =>
        SubarrayWithElementsGreaterThanVaryingThresholdSolution.ValidSubarraySizeByUnionFindOrder(
            _nums, _threshold);
}
