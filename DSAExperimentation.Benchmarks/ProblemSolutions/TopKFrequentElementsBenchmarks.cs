using DSAExperimentation.LeetCode.TopKFrequentElements;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TopKFrequentElementsSolution's, the same methods
// TopKFrequentElementsSolutionTests proves correct.
//
// LeetCode 347 guarantees the answer is unique, and an earlier version of this
// workload broke that: drawing every value at random from [0, 2000) tied 23 values
// at the top-10 boundary, so each arm legitimately returned a different 8 of the 23
// and the two timed different selections. The workload now plants its answer. The
// values 0..K-1 each appear PlantedCount times; every value in [K, 2000) takes its
// turn filling the rest, so none of them appears more than one more than
// Length / range times - fewer than PlantedCount by construction. A seeded shuffle
// then scatters the array, so neither strategy meets the answer in a convenient
// order. What separates sorting every distinct value from keeping a heap of K is the
// number of distinct values, and that stays at the old scale: about 1,000 at the
// smaller Length, all 2,000 at the larger.
public class TopKFrequentElementsBenchmarks
{
    private const int K = 10;
    private const int RandomSeed = 5;
    private const int ValueUpperBoundExclusive = 2_000;

    // How many more times than Length / range each planted value appears: the background's most
    // frequent value can reach one more than that quotient, so two keeps every planted value ahead.
    private const int PlantedMargin = 2;

    private int[] _values = [];

    [Params(1_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var backgroundRange = ValueUpperBoundExclusive - K;
        var plantedCount = (Length / backgroundRange) + PlantedMargin;
        var plantedTotal = K * plantedCount;
        var values = new int[Length];

        for (var i = 0; i < plantedTotal; i++)
        {
            values[i] = i / plantedCount;
        }

        for (var i = plantedTotal; i < Length; i++)
        {
            values[i] = K + ((i - plantedTotal) % backgroundRange);
        }

        Shuffle(values, new Random(RandomSeed));
        _values = values;
    }

    private static void Shuffle(int[] values, Random random)
    {
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int[] FullSort() => TopKFrequentElementsSolution.FindTopKFrequentByFullSort(_values, K);

    [Benchmark]
    public int[] SizeKMinHeap() => TopKFrequentElementsSolution.FindTopKFrequentBySizeKMinHeap(_values, K);
}
