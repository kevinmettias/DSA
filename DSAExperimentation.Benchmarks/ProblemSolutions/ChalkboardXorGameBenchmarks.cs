using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Chalkboard XOR Game (LC 810): the textbook unmemoized bitmask minimax recursion
// (re-explores every erase order that reaches the same remaining subset, O(n!) worst
// case - same shape CanIWinBenchmarks' used-numbers bitmask) vs. that same recursion
// routed through this repo's own Memoizer, keyed on the remaining-elements bitmask
// (O(2^n * n) states), vs. the well-known O(n) closed form the recursion itself
// reduces to (xor(nums) == 0) || (nums.Length % 2 == 0) - the same "recursion reduces
// to a closed form" comparison NimGameBenchmarks already makes. The two Length values
// deliberately straddle the crossover: at 9 the unmemoized tree is still small enough
// to roughly match the Memoizer's own per-call overhead, but at 13 it is already
// ~32x slower in a local dry run - the same factorial blowup CanIWinBenchmarks
// documents for its own unmemoized bitmask baseline, here made visible sooner
// because every mask, not just a used-numbers subset, is a distinct memo key.
[MemoryDiagnoser]
public class ChalkboardXorGameBenchmarks
{
    [Params(9, 13)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(810);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1 << 16)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForceRecursion() => CurrentPlayerWinsBruteForce((1 << _nums.Length) - 1);

    private bool CurrentPlayerWinsBruteForce(int mask)
    {
        if (XorOf(mask) == 0)
        {
            return true;
        }

        for (var i = 0; i < _nums.Length; i++)
        {
            var bit = 1 << i;
            if ((mask & bit) == 0)
            {
                continue;
            }

            var remaining = mask & ~bit;
            if (XorOf(remaining) != 0 && !CurrentPlayerWinsBruteForce(remaining))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool MemoizedRecursion()
    {
        var fullMask = (1 << _nums.Length) - 1;
        return Memoizer.Memoize<int, bool>(fullMask, CurrentPlayerWinsMemoized);

        bool CurrentPlayerWinsMemoized(int mask, Func<int, bool> currentPlayerWins)
        {
            if (XorOf(mask) == 0)
            {
                return true;
            }

            for (var i = 0; i < _nums.Length; i++)
            {
                var bit = 1 << i;
                if ((mask & bit) == 0)
                {
                    continue;
                }

                var remaining = mask & ~bit;
                if (XorOf(remaining) != 0 && !currentPlayerWins(remaining))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [Benchmark]
    public bool ClosedFormFormula()
    {
        var xor = 0;
        foreach (var num in _nums)
        {
            xor ^= num;
        }

        return xor == 0 || _nums.Length % 2 == 0;
    }

    private int XorOf(int mask)
    {
        var result = 0;
        for (var i = 0; i < _nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                result ^= _nums[i];
            }
        }

        return result;
    }
}
