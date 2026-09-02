using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Koko Eating Bananas (LC 875): a hand-rolled int lo/hi bisection loop vs. this
// repo's own BinarySearch.LowerBound over an on-demand FeasibleSpeedSequence
// (KokoEatingBananasTests precedent, itself the same "binary search on the
// answer" shape SplitArrayLargestSumBenchmarks already uses) - both binary-search
// the same monotone feasibility predicate in O(piles.Length * log(max(piles))),
// just one through a bespoke loop and the other through the reusable
// IRandomAccessSequence<bool> abstraction.
[MemoryDiagnoser]
public class KokoEatingBananasBenchmarks
{
    private const int RandomSeed = 875; // LC problem number
    private const int MaxPileSizeExclusive = 1_000;
    private const int HoursPerBanana = 5;
    private const int MidpointDivisor = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _piles = null!;
    private int _h;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _piles = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPileSizeExclusive)).ToArray();
        _h = Length * HoursPerBanana;
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch()
    {
        var low = 1;
        var high = _piles.Max();

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);
            if (HoursNeeded(_piles, mid) <= _h)
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var sequence = new FeasibleSpeedSequence(_piles, _h);
        return 1 + BinarySearch.LowerBound(sequence, true);
    }

    private static long HoursNeeded(int[] piles, int speed)
    {
        var hours = 0L;

        foreach (var pile in piles)
        {
            hours += (pile + speed - 1) / speed;
        }

        return hours;
    }

    private readonly struct FeasibleSpeedSequence(int[] piles, int h) : IRandomAccessSequence<bool>
    {
        public int Length => piles.Max();

        public bool Get(int index) => HoursNeeded(piles, 1 + index) <= h;
    }
}
