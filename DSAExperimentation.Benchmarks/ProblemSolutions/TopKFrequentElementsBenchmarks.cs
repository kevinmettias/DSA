using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TopKFrequentElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TopKFrequentElementsSolution's, the same methods
// TopKFrequentElementsTests proves correct.
[MemoryDiagnoser]
public class TopKFrequentElementsBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 5;
    private const int ValueUpperBoundExclusive = 2_000;

    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        // Bounded to a range far smaller than Length so values repeat and real
        // frequency skew emerges, the same reasoning TwoSumBenchmarks' bounded
        // random range documents.
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] FullSort() => TopKFrequentElementsSolution.FindTopKFrequentByFullSort(_values, K);

    [Benchmark]
    public int[] SizeKMinHeap() => TopKFrequentElementsSolution.FindTopKFrequentBySizeKMinHeap(_values, K);
}
