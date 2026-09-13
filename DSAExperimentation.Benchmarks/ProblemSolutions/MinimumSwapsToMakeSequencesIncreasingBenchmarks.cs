using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumSwapsToMakeSequencesIncreasing;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumSwapsToMakeSequencesIncreasingSolution's, the
// same methods MinimumSwapsToMakeSequencesIncreasingTests proves correct - the
// textbook flat keep[i]/swap[i] tabulation against this repo's own Memoizer closing
// over a two-state (index, wasSwapped) recurrence. Values at index i are always
// {2i, 2i+1} in random order, so both keep and swap stay valid transitions at every
// step regardless of prior choices - both strategies run their full real workload
// instead of one branch getting pruned away immediately.
[MemoryDiagnoser]
public class MinimumSwapsToMakeSequencesIncreasingBenchmarks
{
    // LC problem number, reused as the fixed benchmark-data seed.
    private const int RandomSeed = 801;

    // Each index's two candidate values are a {2i, 2i+1} pair.
    private const int PairSpacing = 2;

    // random.Next(CoinFlipBound) picks one of exactly two outcomes.
    private const int CoinFlipBound = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = new int[Length];
        _nums2 = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            var low = PairSpacing * i;
            var high = PairSpacing * i + 1;

            if (random.Next(CoinFlipBound) == 0)
            {
                _nums1[i] = low;
                _nums2[i] = high;
            }
            else
            {
                _nums1[i] = high;
                _nums2[i] = low;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int Tabulation() =>
        MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByTabulation(_nums1, _nums2);

    [Benchmark]
    public int MemoizedTwoState() =>
        MinimumSwapsToMakeSequencesIncreasingSolution.MinSwapByMemoizedTwoState(_nums1, _nums2);
}
