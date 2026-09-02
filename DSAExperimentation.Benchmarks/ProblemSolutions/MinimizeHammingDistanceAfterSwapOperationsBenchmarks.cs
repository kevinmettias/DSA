using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimize Hamming Distance After Swap Operations (LC 1722): adjacency-list BFS to find
// each index's swap-component (the same baseline SmallestStringWithSwapsBenchmarks uses)
// vs. this repo's own DisjointSet(n) - O(1) Union per allowed swap and O(a(n)) Find per
// index instead of a queue-driven walk. Both strategies finish the per-component Hamming
// distance the same way, with a HashMap<value,int> frequency count over each component.
[MemoryDiagnoser]
public class MinimizeHammingDistanceAfterSwapOperationsBenchmarks
{
    private const int RandomSeed = 1722; // LC problem number
    private const int MaxValueExclusive = 50;

    [Params(200, 5_000)]
    public int Length;

    private int[] _source = null!;
    private int[] _target = null!;
    private int[][] _allowedSwaps = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _source = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _target = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();

        _allowedSwaps = new int[Length][];
        for (var i = 0; i < Length; i++)
        {
            _allowedSwaps[i] = [random.Next(Length), random.Next(Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public int AdjacencyListBfs()
    {
        var adjacency = new List<int>[Length];
        for (var i = 0; i < Length; i++)
        {
            adjacency[i] = [];
        }

        foreach (var swap in _allowedSwaps)
        {
            adjacency[swap[0]].Add(swap[1]);
            adjacency[swap[1]].Add(swap[0]);
        }

        var visited = new bool[Length];
        var distance = 0;

        for (var start = 0; start < Length; start++)
        {
            distance += ExploreComponentAndCountMismatches(start, adjacency, visited);
        }

        return distance;
    }

    private int ExploreComponentAndCountMismatches(int start, List<int>[] adjacency, bool[] visited)
    {
        if (visited[start])
        {
            return 0;
        }

        var queue = new Queue<int>();
        queue.Enqueue(start);
        visited[start] = true;

        var component = new List<int> { start };
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var next in adjacency[current])
            {
                VisitIfUnseen(next, visited, component, queue);
            }
        }

        return MismatchCount(component);
    }

    private static void VisitIfUnseen(int next, bool[] visited, List<int> component, Queue<int> queue)
    {
        if (visited[next])
        {
            return;
        }

        visited[next] = true;
        component.Add(next);
        queue.Enqueue(next);
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var components = BuildDisjointSet();
        var indicesByRoot = GroupIndicesByRoot(components);

        return SumMismatchesByComponent(indicesByRoot);
    }

    private DisjointSet BuildDisjointSet()
    {
        var components = new DisjointSet(Length);

        foreach (var swap in _allowedSwaps)
        {
            components.Union(swap[0], swap[1]);
        }

        return components;
    }

    private HashMap<int, List<int>> GroupIndicesByRoot(DisjointSet components)
    {
        var indicesByRoot = new HashMap<int, List<int>>();
        for (var i = 0; i < Length; i++)
        {
            var root = components.Find(i);
            if (!indicesByRoot.TryGetValue(root, out var indices))
            {
                indices = [];
                indicesByRoot.Set(root, indices);
            }

            indices.Add(i);
        }

        return indicesByRoot;
    }

    private int SumMismatchesByComponent(HashMap<int, List<int>> indicesByRoot)
    {
        var distance = 0;
        foreach (var root in indicesByRoot.Keys)
        {
            indicesByRoot.TryGetValue(root, out var indices);
            distance += MismatchCount(indices!);
        }

        return distance;
    }

    private int MismatchCount(List<int> indices)
    {
        var availableCounts = new HashMap<int, int>();
        foreach (var i in indices)
        {
            availableCounts.TryGetValue(_source[i], out var count);
            availableCounts.Set(_source[i], count + 1);
        }

        var mismatches = 0;
        foreach (var i in indices)
        {
            if (availableCounts.TryGetValue(_target[i], out var count) && count > 0)
            {
                availableCounts.Set(_target[i], count - 1);
            }
            else
            {
                mismatches++;
            }
        }

        return mismatches;
    }
}
