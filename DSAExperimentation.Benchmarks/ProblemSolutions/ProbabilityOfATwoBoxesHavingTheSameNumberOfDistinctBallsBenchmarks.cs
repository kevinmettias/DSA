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
    [Params(4, 6)]
    public int TypeCount;

    private int[] _balls = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1467);
        _balls = [.. Enumerable.Range(0, TypeCount).Select(_ => random.Next(1, 7))];

        if (_balls.Sum() % 2 != 0)
        {
            _balls[^1]++;
        }
    }

    [Benchmark(Baseline = true)]
    public double HandRolledRecursion()
    {
        var half = _balls.Sum() / 2;
        var matchingWays = Recurse(typeIndex: 0, box1Total: 0, box1Distinct: 0, box2Distinct: 0, ways: 1.0, half);
        return matchingWays / BinomialCoefficient(_balls.Sum(), half);
    }

    private double Recurse(int typeIndex, int box1Total, int box1Distinct, int box2Distinct, double ways, int half)
    {
        if (typeIndex == _balls.Length)
        {
            return box1Total == half && box1Distinct == box2Distinct ? ways : 0.0;
        }

        var typeCount = _balls[typeIndex];
        var matching = 0.0;

        for (var toBox1 = 0; toBox1 <= typeCount; toBox1++)
        {
            var nextWays = ways * BinomialCoefficient(typeCount, toBox1);
            var nextBox1Distinct = box1Distinct + (toBox1 > 0 ? 1 : 0);
            var nextBox2Distinct = box2Distinct + (typeCount - toBox1 > 0 ? 1 : 0);
            matching += Recurse(typeIndex + 1, box1Total + toBox1, nextBox1Distinct, nextBox2Distinct, nextWays, half);
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
        var half = total / 2;
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
            choose: (s, toBox1) =>
            {
                var typeCount = balls[s.TypeIndex];
                s.Ways *= BinomialCoefficient(typeCount, toBox1);
                s.Box1Total += toBox1;

                if (toBox1 > 0)
                {
                    s.Box1DistinctCount++;
                }

                if (typeCount - toBox1 > 0)
                {
                    s.Box2DistinctCount++;
                }

                s.TypeIndex++;
            },
            unchoose: (s, toBox1) =>
            {
                s.TypeIndex--;
                var typeCount = balls[s.TypeIndex];

                if (typeCount - toBox1 > 0)
                {
                    s.Box2DistinctCount--;
                }

                if (toBox1 > 0)
                {
                    s.Box1DistinctCount--;
                }

                s.Box1Total -= toBox1;
                s.Ways /= BinomialCoefficient(typeCount, toBox1);
            },
            onSolution: s =>
            {
                if (s.Box1Total == half && s.Box1DistinctCount == s.Box2DistinctCount)
                {
                    matchingWays += s.Ways;
                }
            });

        return matchingWays / totalWays;
    }

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
