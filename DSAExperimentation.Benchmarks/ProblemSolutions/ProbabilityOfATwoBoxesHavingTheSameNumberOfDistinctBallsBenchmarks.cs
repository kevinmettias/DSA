using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Probability of a Two Boxes Having The Same Number of Distinct Balls (LC 1467):
// a hand-rolled recursive enumeration (baseline - the same choose-a-split-per-
// type recursion, written by hand with no engine underneath) vs.
// Backtrack.Search (Algorithms/Backtracking/Backtrack.cs) driving the identical
// choose/unchoose/onSolution walk through this repo's own primitive. Both sides
// visit the exact same search tree and do the exact same BinomialCoefficient
// arithmetic per node, so the comparison isolates Backtrack's own call/delegate
// overhead rather than a difference in what's being computed.
[MemoryDiagnoser]
public class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarks
{
    // LC problem number, used as the deterministic seed for ball-count generation.
    private const int RandomSeed = 1467;

    // Random.Next(1, BallCountUpperBound) yields each type's ball count in [1, BallCountUpperBound - 1].
    private const int BallCountUpperBound = 7;

    // The balls are split between exactly two boxes.
    private const int BoxCount = 2;

    [Params(4, 6)]
    public int TypeCount;

    private int[] _balls = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _balls = [.. Enumerable.Range(0, TypeCount).Select(_ => random.Next(1, BallCountUpperBound))];

        if (_balls.Sum() % BoxCount != 0)
        {
            _balls[^1]++;
        }
    }

    [Benchmark(Baseline = true)]
    public double HandRolledRecursion()
    {
        var half = _balls.Sum() / BoxCount;
        var matchingWays = Recurse(typeIndex: 0, new SplitCounts(0, 0, 0), ways: 1.0, half);
        return matchingWays / BinomialCoefficient(_balls.Sum(), half);
    }

    // Bundles the recursion's per-branch running totals so Recurse stays at 4 parameters.
    private readonly record struct SplitCounts(int Box1Total, int Box1Distinct, int Box2Distinct);

    private double Recurse(int typeIndex, SplitCounts counts, double ways, int half)
    {
        if (typeIndex == _balls.Length)
        {
            return counts.Box1Total == half && counts.Box1Distinct == counts.Box2Distinct ? ways : 0.0;
        }

        var typeCount = _balls[typeIndex];
        var matching = 0.0;

        for (var toBox1 = 0; toBox1 <= typeCount; toBox1++)
        {
            var nextWays = ways * BinomialCoefficient(typeCount, toBox1);
            var nextCounts = new SplitCounts(
                counts.Box1Total + toBox1,
                counts.Box1Distinct + (toBox1 > 0 ? 1 : 0),
                counts.Box2Distinct + (typeCount - toBox1 > 0 ? 1 : 0));
            matching += Recurse(typeIndex + 1, nextCounts, nextWays, half);
        }

        return matching;
    }

    [Benchmark]
    public double BacktrackPrimitive() => GetProbability(_balls);

    // Mutable per-search scratch state Choose/Unchoose thread through - the
    // TState : class Backtrack.Search requires.
    private sealed class SplitState
    {
        public int TypeIndex;
        public int Box1Total;
        public int Box1DistinctCount;
        public int Box2DistinctCount;
        public double Ways = 1.0;
    }

    private static double GetProbability(int[] balls)
    {
        var total = balls.Sum();
        var half = total / BoxCount;
        var totalWays = BinomialCoefficient(total, half);
        var matchingWays = 0.0;
        var state = new SplitState();

        Backtrack.Search<SplitState, int>(
            state,
            isSolution: s => s.TypeIndex == balls.Length,
            // Search's onSolution never signals "stop" (see Backtrack.cs), so
            // TryEachCandidate still runs even at a leaf where IsSolution was
            // already true - Candidates must return empty there rather than
            // index balls out of bounds.
            candidates: s => s.TypeIndex == balls.Length ? [] : Enumerable.Range(0, balls[s.TypeIndex] + 1),
            choose: (s, toBox1) => ChooseSplit(s, balls, toBox1),
            unchoose: (s, toBox1) => UnchooseSplit(s, balls, toBox1),
            onSolution: s => matchingWays += SolutionWays(s, half));

        return matchingWays / totalWays;
    }

    private static void ChooseSplit(SplitState state, int[] balls, int toBox1)
    {
        var typeCount = balls[state.TypeIndex];
        state.Ways *= BinomialCoefficient(typeCount, toBox1);
        state.Box1Total += toBox1;

        if (toBox1 > 0)
        {
            state.Box1DistinctCount++;
        }

        if (typeCount - toBox1 > 0)
        {
            state.Box2DistinctCount++;
        }

        state.TypeIndex++;
    }

    private static void UnchooseSplit(SplitState state, int[] balls, int toBox1)
    {
        state.TypeIndex--;
        var typeCount = balls[state.TypeIndex];

        if (typeCount - toBox1 > 0)
        {
            state.Box2DistinctCount--;
        }

        if (toBox1 > 0)
        {
            state.Box1DistinctCount--;
        }

        state.Box1Total -= toBox1;
        state.Ways /= BinomialCoefficient(typeCount, toBox1);
    }

    private static double SolutionWays(SplitState state, int half) =>
        state.Box1Total == half && state.Box1DistinctCount == state.Box2DistinctCount ? state.Ways : 0.0;

    private static double BinomialCoefficient(int n, int r)
    {
        var result = 1.0;

        for (var i = 0; i < r; i++)
        {
            result = result * (n - i) / (i + 1);
        }

        return result;
    }
}
