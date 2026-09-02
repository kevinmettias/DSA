using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum AND Sum of Array (LC 2172): the textbook unmemoized recursion over (which
// slot is being filled, which elements are already placed) - re-exploring the
// identical (Slot, UsedMask) subtree once per distinct order the earlier slots
// happened to consume elements in - vs. the same recursion routed through this
// repo's own Memoizer, keyed on that same tuple - MaximumStudentsTakingExamBenchmarks'
// precedent, renaming "row" to "slot." nums always fills every slot to capacity
// (Length == 2 * NumSlots) to maximize both the per-slot branching factor (every
// remaining element is a candidate single or pairing) and how often different paths
// converge on the same (Slot, UsedMask) - the worst case for an unmemoized walk and
// the best case for memoization. NumSlots stays small - the unmemoized side's
// branching is quadratic in the *remaining* element count at every one of NumSlots
// levels, so it grows far faster than MaximumStudentsTakingExamBenchmarks' per-row
// linear-in-columns branching.
[MemoryDiagnoser]
public class MaximumAndSumOfArrayBenchmarks
{
    private const int MaxValueExclusive = 1 << 20;
    private const int RandomSeed = 2172; // LC problem number
    private const int ElementsPerSlot = 2;

    [Params(2, 3)]
    public int NumSlots;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, NumSlots * ElementsPerSlot).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => BestFromBruteForce(1, 0);

    private int BestFromBruteForce(int slot, int usedMask)
    {
        if (slot > NumSlots)
        {
            return 0;
        }

        var best = BestFromBruteForce(slot + 1, usedMask);

        for (var i = 0; i < _nums.Length; i++)
        {
            best = BestConsideringElementBruteForce(slot, usedMask, i, best);
        }

        return best;
    }

    private int BestConsideringElementBruteForce(int slot, int usedMask, int i, int best)
    {
        var bitI = 1 << i;
        if ((usedMask & bitI) != 0)
        {
            return best;
        }

        var withOne = (slot & _nums[i]) + BestFromBruteForce(slot + 1, usedMask | bitI);
        best = Math.Max(best, withOne);

        return BestConsideringPairBruteForce(slot, usedMask, i, best);
    }

    private int BestConsideringPairBruteForce(int slot, int usedMask, int i, int best)
    {
        var bitI = 1 << i;

        for (var j = i + 1; j < _nums.Length; j++)
        {
            var bitJ = 1 << j;
            if ((usedMask & bitJ) != 0)
            {
                continue;
            }

            var withTwo = (slot & _nums[i]) + (slot & _nums[j])
                + BestFromBruteForce(slot + 1, usedMask | bitI | bitJ);
            best = Math.Max(best, withTwo);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Slot, int UsedMask), int>((1, 0), BestFrom);

    private int BestFrom((int Slot, int UsedMask) state, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var (slot, usedMask) = state;
        if (slot > NumSlots)
        {
            return 0;
        }

        var best = bestFrom((slot + 1, usedMask));

        for (var i = 0; i < _nums.Length; i++)
        {
            best = BestConsideringElement(state, i, best, bestFrom);
        }

        return best;
    }

    private int BestConsideringElement(
        (int Slot, int UsedMask) state, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var (slot, usedMask) = state;
        var bitI = 1 << i;
        if ((usedMask & bitI) != 0)
        {
            return best;
        }

        var withOne = (slot & _nums[i]) + bestFrom((slot + 1, usedMask | bitI));
        best = Math.Max(best, withOne);

        return BestConsideringPair(state, i, best, bestFrom);
    }

    private int BestConsideringPair(
        (int Slot, int UsedMask) state, int i, int best, Func<(int Slot, int UsedMask), int> bestFrom)
    {
        var (slot, usedMask) = state;
        var bitI = 1 << i;

        for (var j = i + 1; j < _nums.Length; j++)
        {
            var bitJ = 1 << j;
            if ((usedMask & bitJ) != 0)
            {
                continue;
            }

            var withTwo = (slot & _nums[i]) + (slot & _nums[j])
                + bestFrom((slot + 1, usedMask | bitI | bitJ));
            best = Math.Max(best, withTwo);
        }

        return best;
    }
}
