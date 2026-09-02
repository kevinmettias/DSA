using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Ice Cream Bars (LC 1833): an O(n^2) repeated-selection-scan baseline
// (mirrors selection sort - find the cheapest remaining bar, buy it if
// affordable, repeat) vs. this repo's MergeSort once up front followed by a
// single O(n) greedy pass. _coins is sized to roughly a quarter of the bars'
// total cost so both strategies are forced to scan well past the cheapest few
// bars instead of exiting after one or two purchases.
[MemoryDiagnoser]
public class MaximumIceCreamBarsBenchmarks
{
    private const int MaxCostExclusive = 100;
    private const int CoinsPerBarDivisor = 4;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1833;

    [Params(200, 5_000)]
    public int Length;

    private int[] _costs = null!;
    private int _coins;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _costs = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxCostExclusive)).ToArray();
        _coins = (Length * MaxCostExclusive) / CoinsPerBarDivisor;
    }

    [Benchmark(Baseline = true)]
    public int SelectionScan()
    {
        var costs = (int[])_costs.Clone();
        var used = new bool[costs.Length];
        var coins = _coins;
        var count = 0;

        for (var picked = 0; picked < costs.Length; picked++)
        {
            if (!TryBuyCheapestUnused(costs, used, coins, out var cost))
            {
                break;
            }

            coins -= cost;
            count++;
        }

        return count;
    }

    private static bool TryBuyCheapestUnused(int[] costs, bool[] used, int coins, out int cost)
    {
        var cheapestIndex = -1;

        for (var i = 0; i < costs.Length; i++)
        {
            if (!used[i] && (cheapestIndex == -1 || costs[i] < costs[cheapestIndex]))
            {
                cheapestIndex = i;
            }
        }

        cost = costs[cheapestIndex];

        if (cost > coins)
        {
            return false;
        }

        used[cheapestIndex] = true;
        return true;
    }

    [Benchmark]
    public int MergeSortGreedy()
    {
        var costs = (int[])_costs.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(costs));

        var coins = _coins;
        var count = 0;

        foreach (var cost in costs)
        {
            if (cost > coins)
            {
                break;
            }

            coins -= cost;
            count++;
        }

        return count;
    }
}
