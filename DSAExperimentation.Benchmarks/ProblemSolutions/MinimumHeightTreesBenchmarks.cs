using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Height Trees (LC 310): the textbook O(n^2) approach - BFS from every single
// node to measure the tree's height when rooted there, then keep the nodes achieving
// the minimum - vs. the O(n) approach of peeling leaves (degree-1 nodes) layer by
// layer until at most 2 centroid nodes remain, using this repo's own Queue<int> as the
// frontier (RemoveInvalidParenthesesTests.cs's own level-by-level BFS peel).
[MemoryDiagnoser]
public class MinimumHeightTreesBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private List<int>[] _adjacency = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            _adjacency[i] = [];
        }

        // A random recursive tree: each node (after the first) attaches to a
        // uniformly-chosen earlier node, giving a connected, cycle-free graph on
        // NodeCount vertices with NodeCount-1 edges.
        for (var i = 1; i < NodeCount; i++)
        {
            var parent = random.Next(i);
            _adjacency[i].Add(parent);
            _adjacency[parent].Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public int HeightFromEveryNode()
    {
        var minHeight = int.MaxValue;
        var rootsAtMinHeight = 0;

        for (var root = 0; root < NodeCount; root++)
        {
            var height = HeightFrom(root);
            if (height < minHeight)
            {
                minHeight = height;
                rootsAtMinHeight = 1;
            }
            else if (height == minHeight)
            {
                rootsAtMinHeight++;
            }
        }

        return rootsAtMinHeight;
    }

    private int HeightFrom(int root)
    {
        var visited = new bool[NodeCount];
        visited[root] = true;
        var frontier = new Queue<int>();
        frontier.Enqueue(root);
        var height = -1;

        while (frontier.Count > 0)
        {
            var levelSize = frontier.Count;
            height++;

            for (var i = 0; i < levelSize; i++)
            {
                var node = frontier.Dequeue();
                foreach (var neighbor in _adjacency[node])
                {
                    if (!visited[neighbor])
                    {
                        visited[neighbor] = true;
                        frontier.Enqueue(neighbor);
                    }
                }
            }
        }

        return height;
    }

    [Benchmark]
    public int LeafPeeling()
    {
        if (NodeCount == 1)
        {
            return 1;
        }

        var degree = new int[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            degree[i] = _adjacency[i].Count;
        }

        var leaves = new RepoQueue();
        var remaining = NodeCount;
        for (var i = 0; i < NodeCount; i++)
        {
            if (degree[i] == 1)
            {
                leaves.Enqueue(i);
            }
        }

        while (remaining > 2)
        {
            var leafCount = leaves.Count;
            remaining -= leafCount;

            for (var i = 0; i < leafCount; i++)
            {
                leaves.TryDequeue(out var leaf);

                foreach (var neighbor in _adjacency[leaf])
                {
                    if (--degree[neighbor] == 1)
                    {
                        leaves.Enqueue(neighbor);
                    }
                }
            }
        }

        return leaves.Count;
    }
}
