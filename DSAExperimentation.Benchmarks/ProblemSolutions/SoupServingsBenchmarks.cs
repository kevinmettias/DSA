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
    private const int ServingSizeMl = 25;
    private const int CeilingRoundingOffset = 24;
    private const double TieProbability = 0.5;
    private const double BranchProbability = 0.25;
    private const int FourUnitPour = 4;
    private const int ThreeUnitPour = 3;
    private const int TwoUnitPour = 2;

    [Params(600, 850)]
    public int N;

    private int _servings;

    [GlobalSetup]
    public void Setup() => _servings = (N + CeilingRoundingOffset) / ServingSizeMl;

    [Benchmark(Baseline = true)]
    public double UnmemoizedRecursion() => Probability(_servings, _servings);

    private double Probability(int a, int b)
    {
        if (a <= 0 && b <= 0)
        {
            return TieProbability;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return BranchProbability * (
            Probability(a - FourUnitPour, b)
            + Probability(a - ThreeUnitPour, b - 1)
            + Probability(a - TwoUnitPour, b - TwoUnitPour)
            + Probability(a - 1, b - ThreeUnitPour));
    }

    [Benchmark]
    public double MemoizedRecursion()
        => Memoizer.Memoize<(int A, int B), double>((_servings, _servings), ProbabilityMemoized);

    private double ProbabilityMemoized((int A, int B) remaining, Func<(int A, int B), double> probability)
    {
        var (a, b) = remaining;
        if (a <= 0 && b <= 0)
        {
            return TieProbability;
        }

        if (a <= 0)
        {
            return 1.0;
        }

        if (b <= 0)
        {
            return 0.0;
        }

        return BranchProbability * (
            probability((a - FourUnitPour, b))
            + probability((a - ThreeUnitPour, b - 1))
            + probability((a - TwoUnitPour, b - TwoUnitPour))
            + probability((a - 1, b - ThreeUnitPour)));
    }
}
