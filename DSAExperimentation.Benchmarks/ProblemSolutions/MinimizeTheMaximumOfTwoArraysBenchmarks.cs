using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimize the Maximum of Two Arrays (LC 2513): a hand-rolled long lo/hi bisection
// loop vs. this repo's own BinarySearch.LowerBound over an on-demand
// FeasibleMaximumSequence (MinimizeTheMaximumOfTwoArraysTests precedent, itself the
// same "binary search on the answer" shape KokoEatingBananasBenchmarks already
// uses) - both binary-search the same monotone feasibility predicate, just one
// through a bespoke loop and the other through the reusable
// IRandomAccessSequence<bool> abstraction. Divisors are fixed and coprime (2, 3) so
// every UniqueCountScale forces a real lcm=6 inclusion-exclusion check instead of
// degenerating to a single-divisor case.
[MemoryDiagnoser]
public class MinimizeTheMaximumOfTwoArraysBenchmarks
{
    private const int Divisor1 = 2;
    private const int Divisor2 = 3;
    private const int MidpointDivisor = 2;

    [Params(1_000, 1_000_000)]
    public int UniqueCountScale;

    private int _uniqueCnt1;
    private int _uniqueCnt2;

    [GlobalSetup]
    public void Setup()
    {
        _uniqueCnt1 = UniqueCountScale;
        _uniqueCnt2 = UniqueCountScale;
    }

    [Benchmark(Baseline = true)]
    public long ManualBinarySearch()
    {
        var low = 1L;
        var high = (2L * (_uniqueCnt1 + _uniqueCnt2)) + 1;

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);

            if (IsFeasible(_uniqueCnt1, _uniqueCnt2, mid))
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
    public long SequenceLowerBound()
    {
        var sequence = new FeasibleMaximumSequence(_uniqueCnt1, _uniqueCnt2);
        return 1 + BinarySearch.LowerBound(sequence, true);
    }

    private static bool IsFeasible(int uniqueCnt1, int uniqueCnt2, long max)
    {
        const long lcm = 6; // lcm(Divisor1, Divisor2)
        var eligible1 = max - (max / Divisor1);
        var eligible2 = max - (max / Divisor2);
        var eligibleEither = max - (max / lcm);

        return eligible1 >= uniqueCnt1 && eligible2 >= uniqueCnt2 && eligibleEither >= uniqueCnt1 + (long)uniqueCnt2;
    }

    private readonly struct FeasibleMaximumSequence(int uniqueCnt1, int uniqueCnt2) : IRandomAccessSequence<bool>
    {
        public int Length => (2 * (uniqueCnt1 + uniqueCnt2)) + 1;

        public bool Get(int index) => IsFeasible(uniqueCnt1, uniqueCnt2, 1 + index);
    }
}
