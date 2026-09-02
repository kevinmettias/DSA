using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// IPO (LC 502): an O(k*n) linear-scan-per-round baseline (re-scan every remaining
// project each round for the best affordable one) vs. the O((n+k) log n) two-heap
// greedy using this repo's own Heap<T,TOrder> - a min-heap of projects by required
// capital (ByPriorityOrder<int,int>'s (node, priority) projection) feeding a
// max-heap of unlocked profits (MaxHeapOrder<int>), the same composition
// IPOTests uses.
[MemoryDiagnoser]
public class IPOBenchmarks
{
    private const int K = 20;
    private const int RandomSeed = 11;
    private const int MaxProfit = 1_000;
    private const int MaxCapital = 1_000;

    [Params(200, 5_000)]
    public int ProjectCount;

    private int[] _profits = null!;
    private int[] _capitals = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _profits = Enumerable.Range(0, ProjectCount).Select(_ => random.Next(1, MaxProfit)).ToArray();
        _capitals = Enumerable.Range(0, ProjectCount).Select(_ => random.Next(0, MaxCapital)).ToArray();
    }

    private int FindBestAffordableIndex(bool[] used, int capital)
    {
        var bestIndex = -1;

        for (var i = 0; i < _profits.Length; i++)
        {
            if (!used[i] && _capitals[i] <= capital && (bestIndex == -1 || _profits[i] > _profits[bestIndex]))
            {
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    [Benchmark(Baseline = true)]
    public int LinearScanPerRound()
    {
        var used = new bool[_profits.Length];
        var w = 0;

        for (var round = 0; round < K; round++)
        {
            var bestIndex = FindBestAffordableIndex(used, w);

            if (bestIndex == -1)
            {
                break;
            }

            used[bestIndex] = true;
            w += _profits[bestIndex];
        }

        return w;
    }

    private Heap<(int Node, int Priority), ByPriorityOrder<int, int>> BuildCapitalHeap()
    {
        var byCapital = new Heap<(int Node, int Priority), ByPriorityOrder<int, int>>();

        for (var i = 0; i < _profits.Length; i++)
        {
            byCapital.Push((_profits[i], _capitals[i]));
        }

        return byCapital;
    }

    private static void UnlockAffordableProjects(
        Heap<(int Node, int Priority), ByPriorityOrder<int, int>> byCapital,
        Heap<int, MaxHeapOrder<int>> byProfit,
        int capital)
    {
        while (byCapital.TryPeek(out var cheapest) && cheapest.Priority <= capital)
        {
            byCapital.TryPop(out var popped);
            byProfit.Push(popped.Node);
        }
    }

    [Benchmark]
    public int TwoHeapGreedy()
    {
        var byCapital = BuildCapitalHeap();
        var byProfit = new Heap<int, MaxHeapOrder<int>>();
        var w = 0;

        for (var round = 0; round < K; round++)
        {
            UnlockAffordableProjects(byCapital, byProfit, w);

            if (!byProfit.TryPop(out var bestProfit))
            {
                break;
            }

            w += bestProfit;
        }

        return w;
    }
}
