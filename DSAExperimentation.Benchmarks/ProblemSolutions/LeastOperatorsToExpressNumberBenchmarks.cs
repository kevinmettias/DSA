using BenchmarkDotNet.Attributes;

using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Least Operators to Express Number (LC 964): the base-x digit recursion always
// branches into two choices (round the current digit down, or up with a +1 carry
// into the next exponent) - plain UnmemoizedRecursion re-explores both branches
// from scratch every time two different root-to-node paths land on the same
// (remaining, level) pair, giving a full binary recursion tree. MemoizedRecursion
// caches on that exact pair via this repo's own Memoizer<TState,TResult> - the
// same tuple-state shape BurstBalloonsBenchmarks already uses - collapsing the
// tree back down to the handful of distinct states digit DP actually needs.
// TargetBitLength is kept modest, the same reasoning BurstBalloonsBenchmarks'
// BalloonCount cap already documents for its own un-memoized baseline.
[MemoryDiagnoser]
public class LeastOperatorsToExpressNumberBenchmarks
{
    private const int X = 2;

    // The units-digit (level 0) cost weight is always 2, independent of X - writing
    // the base value x itself costs one x/x division to reach 1, so its weight is
    // fixed rather than derived from the recursion level like every other digit's.
    private const int UnitsDigitWeight = 2;

    [Params(16, 20)]
    public int TargetBitLength;

    private int _target;

    [GlobalSetup]
    public void Setup() => _target = (1 << TargetBitLength) - 1;

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() => Cost(_target, 0) - 1;

    private static int Cost(int remaining, int level)
    {
        var weight = level == 0 ? UnitsDigitWeight : level;

        return remaining < X
            ? BaseCaseCost(remaining, level, weight)
            : RecursiveCaseCost(remaining, level, weight, Cost);
    }

    // remaining < X is the base case, not just remaining == 0 - see
    // LeastOperatorsToExpressNumberTests.Cost for why recursing past it
    // (instead of this closed form) loops forever without ever winning.
    private static int BaseCaseCost(int remaining, int level, int weight)
    {
        var baseRoundDown = remaining * weight;
        var baseRoundUp = (level + 1) + ((X - remaining) * weight);
        return Math.Min(baseRoundDown, baseRoundUp);
    }

    private static int RecursiveCaseCost(int remaining, int level, int weight, Func<int, int, int> cost)
    {
        var digit = remaining % X;
        var quotient = remaining / X;
        var roundDown = (digit * weight) + cost(quotient, level + 1);

        // X == 2 is the one value where quotient+1 can land back on remaining
        // itself (remaining == X == 2): skip round-up rather than recurse into
        // that same state forever - see LeastOperatorsToExpressNumberTests.Cost.
        if (quotient + 1 >= remaining)
        {
            return roundDown;
        }

        var roundUp = ((X - digit) * weight) + cost(quotient + 1, level + 1);
        return Math.Min(roundDown, roundUp);
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        var totalWeight = Memoizer.Memoize<(int Remaining, int Level), int>((_target, 0), CostMemoized);
        return totalWeight - 1;

        int CostMemoized((int Remaining, int Level) state, Func<(int Remaining, int Level), int> cost)
        {
            var (remaining, level) = state;
            var weight = level == 0 ? UnitsDigitWeight : level;

            return remaining < X
                ? BaseCaseCost(remaining, level, weight)
                : RecursiveCaseCost(remaining, level, weight, (r, l) => cost((r, l)));
        }
    }
}
