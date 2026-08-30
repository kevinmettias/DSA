using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cost to Connect Two Groups of Points (LC 1595): the textbook unmemoized
// bitmask recursion over (index, connectedMask) - the same (index, mask) pair
// re-explored from scratch down every branch, since many different group-1
// point orderings reach the identical "these group-2 points are already
// connected" state - vs. the same recursion routed through this repo's own
// Memoizer, the identical (int, int) tuple-state shape
// NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks.cs already uses for LC
// 1434's (hat, mask) recursion.
[MemoryDiagnoser]
public class MinimumCostToConnectTwoGroupsOfPointsBenchmarks
{
    [Params(4, 7)]
    public int GroupSize;

    private int[][] _cost = null!;
    private int[] _minCost2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1595);
        _cost = new int[GroupSize][];
        for (var i = 0; i < GroupSize; i++)
        {
            _cost[i] = Enumerable.Range(0, GroupSize).Select(_ => random.Next(1, 100)).ToArray();
        }

        _minCost2 = new int[GroupSize];
        for (var j = 0; j < GroupSize; j++)
        {
            _minCost2[j] = int.MaxValue;
            for (var i = 0; i < GroupSize; i++)
            {
                _minCost2[j] = Math.Min(_minCost2[j], _cost[i][j]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => MinCost(0, 0);

    private int MinCost(int index, int mask)
    {
        if (index == GroupSize)
        {
            var remaining = 0;
            for (var j = 0; j < GroupSize; j++)
            {
                if ((mask & (1 << j)) == 0)
                {
                    remaining += _minCost2[j];
                }
            }

            return remaining;
        }

        var best = int.MaxValue;
        for (var j = 0; j < GroupSize; j++)
        {
            var candidate = _cost[index][j] + MinCost(index + 1, mask | (1 << j));
            best = Math.Min(best, candidate);
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Index, int Mask), int>((0, 0), (state, costFor) =>
    {
        var (index, mask) = state;

        if (index == GroupSize)
        {
            var remaining = 0;
            for (var j = 0; j < GroupSize; j++)
            {
                if ((mask & (1 << j)) == 0)
                {
                    remaining += _minCost2[j];
                }
            }

            return remaining;
        }

        var best = int.MaxValue;
        for (var j = 0; j < GroupSize; j++)
        {
            var candidate = _cost[index][j] + costFor((index + 1, mask | (1 << j)));
            best = Math.Min(best, candidate);
        }

        return best;
    });
}
