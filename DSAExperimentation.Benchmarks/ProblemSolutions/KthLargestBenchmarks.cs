using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KthLargestElement;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are KthLargestElementSolution's, the same methods
// KthLargestElementTests proves correct.
[MemoryDiagnoser]
public class KthLargestBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 7;

    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int FullSort() => KthLargestElementSolution.FindKthLargestByFullSort(_values, K);

    [Benchmark]
    public int SizeKMinHeap() => KthLargestElementSolution.FindKthLargestBySizeKMinHeap(_values, K);
}
