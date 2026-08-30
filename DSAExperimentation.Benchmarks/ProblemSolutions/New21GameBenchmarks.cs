using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// New 21 Game (LC 837): plain un-memoized recursion over Alice's running point total -
// exponential, since the same points value recurs through many different draw orders -
// vs. this repo's own Memoizer<TState,TResult> caching that value, the same shape
// SoupServingsBenchmarks already uses for its own probability recurrence. MaxPts is
// fixed at 6 and K is the varying [Params] axis (N == K, so only the reachability of
// the recursion tree matters, not the final probability value) so both the recursion
// depth (bounded by K) and the branching factor (MaxPts) stay large enough for the
// un-memoized tree's overlapping-state blowup to show clearly without becoming
// impractically slow.
[MemoryDiagnoser]
public class New21GameBenchmarks
{
    private const int MaxPts = 6;

    [Params(12, 20)]
    public int K;

    private int _n;

    [GlobalSetup]
    public void Setup() => _n = K;

    [Benchmark(Baseline = true)]
    public double UnmemoizedRecursion() => Probability(0);

    private double Probability(int points)
    {
        if (points >= K)
        {
            return points <= _n ? 1.0 : 0.0;
        }

        var total = 0.0;

        for (var draw = 1; draw <= MaxPts; draw++)
        {
            total += Probability(points + draw);
        }

        return total / MaxPts;
    }

    [Benchmark]
    public double MemoizedRecursion() => Memoizer.Memoize<int, double>(0, ProbabilityMemoized);

    private double ProbabilityMemoized(int points, Func<int, double> probability)
    {
        if (points >= K)
        {
            return points <= _n ? 1.0 : 0.0;
        }

        var total = 0.0;

        for (var draw = 1; draw <= MaxPts; draw++)
        {
            total += probability(points + draw);
        }

        return total / MaxPts;
    }
}
