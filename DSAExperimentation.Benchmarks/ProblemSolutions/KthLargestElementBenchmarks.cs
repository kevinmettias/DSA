using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthLargestElement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthLargestElementSolution's, the same methods
// KthLargestElementSolutionTests proves correct. Values are drawn from LC 215's
// [-10^4, 10^4].
public class KthLargestElementBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 7;
    private const int MinValue = -10_000;
    private const int MaxValueExclusive = 10_001;

    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, MinValue, MaxValueExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int FullSort() => KthLargestElementSolution.FindKthLargestByFullSort(_values, K);

    [Benchmark]
    public int SizeKMinHeap() => KthLargestElementSolution.FindKthLargestBySizeKMinHeap(_values, K);
}
