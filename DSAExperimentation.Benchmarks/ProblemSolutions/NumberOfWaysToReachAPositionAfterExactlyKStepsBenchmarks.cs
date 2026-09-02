using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Reach a Position After Exactly k Steps (LC 2400): plain
// un-memoized (remainingSteps, distanceToTarget) recursion - exponential, since the
// same state recurs through many different left/right step-order choices - vs. this
// repo's own Memoizer<TState,TResult> caching that exact pair, the same shape
// TargetSumBenchmarks already uses for its own +/- step recursion. K is kept modest
// specifically because the un-memoized baseline's 2^K blowup is real, the same
// reasoning TargetSumBenchmarks/FibonacciNumberBenchmarks document.
[MemoryDiagnoser]
public class NumberOfWaysToReachAPositionAfterExactlyKStepsBenchmarks
{
    private const int Mod = 1_000_000_007;
    private const int Distance = 2;

    [Params(18, 22)]
    public int K;

    [Benchmark(Baseline = true)]
    public long UnmemoizedRecursion() => Ways(K, Distance);

    private long Ways(int steps, int diff)
    {
        if (diff > steps)
        {
            return 0;
        }

        if (steps == 0)
        {
            return diff == 0 ? 1 : 0;
        }

        return (Ways(steps - 1, Math.Abs(diff - 1)) + Ways(steps - 1, diff + 1)) % Mod;
    }

    [Benchmark]
    public long MemoizedRecursion()
        => Memoizer.Memoize<(int Steps, int Diff), long>((K, Distance), WaysMemoized);

    private long WaysMemoized((int Steps, int Diff) state, Func<(int Steps, int Diff), long> ways)
    {
        var (steps, diff) = state;

        if (diff > steps)
        {
            return 0;
        }

        if (steps == 0)
        {
            return diff == 0 ? 1 : 0;
        }

        return (ways((steps - 1, Math.Abs(diff - 1))) + ways((steps - 1, diff + 1))) % Mod;
    }
}
