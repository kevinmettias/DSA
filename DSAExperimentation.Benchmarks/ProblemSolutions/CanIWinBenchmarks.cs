using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Can I Win (LC 464): the textbook unmemoized bitmask recursion (re-explores every
// permutation of picks, O(MaxChoosableInteger!) worst case) vs. the same recursion
// routed through this repo's own Memoizer, keyed on the used-numbers bitmask
// (O(2^MaxChoosableInteger * MaxChoosableInteger) states). desiredTotal is set to the
// exact sum of every choosable number, so a win can only be confirmed on the very last
// pick - forcing BOTH strategies through their full worst-case search tree instead of
// an early exit on the first invocation making the brute force look artificially fast
// (the same "force the real worst case" convention TwoSumBenchmarks already uses).
[MemoryDiagnoser]
public class CanIWinBenchmarks
{
    [Params(6, 8)]
    public int MaxChoosableInteger;

    private int _desiredTotal;

    [GlobalSetup]
    public void Setup() => _desiredTotal = MaxChoosableInteger * (MaxChoosableInteger + 1) / 2;

    [Benchmark(Baseline = true)]
    public bool BruteForceRecursion() => CanWinBruteForce(0, _desiredTotal);

    private bool CanWinBruteForce(int usedMask, int remainingTotal)
    {
        for (var i = 1; i <= MaxChoosableInteger; i++)
        {
            var bit = 1 << (i - 1);
            if ((usedMask & bit) != 0)
            {
                continue;
            }

            if (i >= remainingTotal || !CanWinBruteForce(usedMask | bit, remainingTotal - i))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool MemoizedRecursion() => Memoizer.Memoize<int, bool>(0, (usedMask, canWin) =>
    {
        for (var i = 1; i <= MaxChoosableInteger; i++)
        {
            var bit = 1 << (i - 1);
            if ((usedMask & bit) != 0)
            {
                continue;
            }

            var remaining = _desiredTotal - SumChosen(usedMask | bit);
            if (remaining <= 0 || !canWin(usedMask | bit))
            {
                return true;
            }
        }

        return false;
    });

    private int SumChosen(int mask)
    {
        var sum = 0;
        for (var i = 1; i <= MaxChoosableInteger; i++)
        {
            if ((mask & (1 << (i - 1))) != 0)
            {
                sum += i;
            }
        }

        return sum;
    }
}
