using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountGoodTripletsInAnArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountGoodTripletsInAnArraySolution's, the same
// methods CountGoodTripletsInAnArrayTests proves correct - the textbook O(n^2)
// pairwise scan against two FenwickTree<int, SumOperation<int>> sweeps at
// O(n log n), the same contrast CountOfSmallerNumbersAfterSelfBenchmarks and
// ReversePairsBenchmarks already draw for LC 315/493. [GlobalSetup] shuffles the
// two permutations, so only the counting is measured.
[MemoryDiagnoser]
public class CountGoodTripletsInAnArrayBenchmarks
{
    private const int RandomSeed = 2179; private int[] _nums1 = [];

    private int[] _nums2 = [];
    // LC problem number

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = ShuffledPermutation(random, Length);
        _nums2 = ShuffledPermutation(random, Length);
    }

    private static int[] ShuffledPermutation(Random random, int length)
    {
        var values = Enumerable.Range(0, length).ToArray();

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        return values;
    }

    [Benchmark(Baseline = true)]
    public long PairwiseScan() =>
        CountGoodTripletsInAnArraySolution.CountGoodTripletsByPairwiseScan(_nums1, _nums2);

    [Benchmark]
    public long FenwickTreeSweep() =>
        CountGoodTripletsInAnArraySolution.CountGoodTripletsByFenwickTreeSweep(_nums1, _nums2);
}
