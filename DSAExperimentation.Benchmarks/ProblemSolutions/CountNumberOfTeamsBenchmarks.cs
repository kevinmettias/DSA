using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Count Number of Teams (LC 1395): the textbook cubic triple-loop scan vs. an
// O(n log n) pair of coordinate-compressed sweeps through this repo's own
// FenwickTree<int, SumOperation<int>>, ranking each rating via
// BinarySearch.LowerBound/UpperBound - the same rank-then-Fenwick-sweep shape
// CountOfSmallerNumbersAfterSelfBenchmarks uses, run once left-to-right and once
// right-to-left to get both halves of every middle soldier's team count.
[MemoryDiagnoser]
public class CountNumberOfTeamsBenchmarks
{
    private const int RandomSeed = 1395; // LC problem number
    private const int MaxRatingExclusive = 100_000;
    private const int TrailingElementsNeededForTeam = 2;

    [Params(80, 200)]
    public int Length;

    private int[] _rating = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _rating = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxRatingExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int TripleLoopScan()
    {
        var rating = _rating;
        var teams = 0;

        for (var i = 0; i < rating.Length - TrailingElementsNeededForTeam; i++)
        {
            for (var j = i + 1; j < rating.Length - 1; j++)
            {
                teams += CountTeamsWithMiddle(rating, i, j);
            }
        }

        return teams;
    }

    private static int CountTeamsWithMiddle(int[] rating, int i, int j)
    {
        var teams = 0;

        for (var k = j + 1; k < rating.Length; k++)
        {
            if ((rating[i] < rating[j] && rating[j] < rating[k]) ||
                (rating[i] > rating[j] && rating[j] > rating[k]))
            {
                teams++;
            }
        }

        return teams;
    }

    [Benchmark]
    public int FenwickTreeSweeps()
    {
        var rating = _rating;
        var n = rating.Length;
        var sortedDistinct = rating.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);

        var (leftLess, leftGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, forward: true);
        var (rightLess, rightGreater) = SweepCounts(rating, sequence, sortedDistinct.Length, forward: false);

        var teams = 0;
        for (var j = 0; j < n; j++)
        {
            teams += (leftLess[j] * rightGreater[j]) + (leftGreater[j] * rightLess[j]);
        }

        return teams;
    }

    private static (int[] Less, int[] Greater) SweepCounts(
        int[] rating, ArraySequence<int> sequence, int distinctCount, bool forward)
    {
        var n = rating.Length;
        var less = new int[n];
        var greater = new int[n];
        var tree = new FenwickTree<int, SumOperation<int>>(distinctCount);
        var seenSoFar = 0;

        for (var step = 0; step < n; step++)
        {
            var j = forward ? step : n - 1 - step;
            var lower = BinarySearch.LowerBound(sequence, rating[j]);
            var upper = BinarySearch.UpperBound(sequence, rating[j]);

            less[j] = lower == 0 ? 0 : tree.PrefixQuery(lower - 1);
            var lessOrEqual = upper == 0 ? 0 : tree.PrefixQuery(upper - 1);
            greater[j] = seenSoFar - lessOrEqual;

            tree.Add(lower, 1);
            seenSoFar++;
        }

        return (less, greater);
    }
}
