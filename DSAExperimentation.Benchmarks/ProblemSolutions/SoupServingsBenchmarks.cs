using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Soup Servings (LC 808): plain un-memoized recursion over the (soupA, soupB)
// remaining-amount pair - exponential, since the same pair recurs through many
// different serving-choice orders - vs. this repo's own Memoizer<TState,TResult>
// caching that exact pair, the same shape PredictTheWinnerBenchmarks already uses for
// its own two-state game recurrence. The two N values deliberately straddle the
// cliff: at 600 the un-memoized tree is still small (~2x slower than memoized in a
// local dry run), but at 850 it blows up to ~700x slower - the real exponential
// blowup PredictTheWinnerBenchmarks/CanIWinBenchmarks document for their own
// un-memoized game-tree baselines, here made sharper by soup's branching factor of 4
// (vs. those two problems' factor of 2-3).
[MemoryDiagnoser]
public class SoupServingsBenchmarks
{
    [Params(600, 850)]
    public int N;

    private int _servings;

    [GlobalSetup]
    public void Setup() => _servings = (N + 24) / 25;

    [Benchmark(Baseline = true)]
    public double UnmemoizedRecursion() => Probability(_servings, _servings);

    private double Probability(int a, int b)
    {
        if (a <= 0 && b <= 0)
        {
            return 0.5;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return 0.25 * (
            Probability(a - 4, b)
            + Probability(a - 3, b - 1)
            + Probability(a - 2, b - 2)
            + Probability(a - 1, b - 3));
    }

    [Benchmark]
    public double MemoizedRecursion()
        => Memoizer.Memoize<(int A, int B), double>((_servings, _servings), ProbabilityMemoized);

    private double ProbabilityMemoized((int A, int B) remaining, Func<(int A, int B), double> probability)
    {
        var (a, b) = remaining;
        if (a <= 0 && b <= 0)
        {
            return 0.5;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return 0.25 * (
            probability((a - 4, b))
            + probability((a - 3, b - 1))
            + probability((a - 2, b - 2))
            + probability((a - 1, b - 3)));
    }
}
