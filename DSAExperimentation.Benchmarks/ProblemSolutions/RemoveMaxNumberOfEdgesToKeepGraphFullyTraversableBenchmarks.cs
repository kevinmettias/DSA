using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Max Number of Edges to Keep Graph Fully Traversable (LC 1579): a BFS flood
// fill answering "are these two nodes already connected?" from scratch on an
// incrementally-built adjacency list (baseline - the textbook substitute for a
// union-find's near-O(1) connectivity check, the same DFS-flood-fill-vs-Union-Find
// contrast NumberOfOperationsToMakeNetworkConnectedBenchmarks already establishes,
// here needing a per-edge reachability check rather than one final component count)
// vs. two of this repo's own DisjointSet instances
// (RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableTests precedent). A spanning tree
// of type-3 edges is generated first so both traversers are always fully connected in
// the end (the interesting, non-trivial case), then extra random single-owner edges
// give both strategies real redundant-edge-rejecting work to do.
[MemoryDiagnoser]
public class RemoveMaxNumberOfEdgesToKeepGraphFullyTraversableBenchmarks
{
    [Params(50, 300)]
    public int NodeCount;

    private int[][] _edges = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1579);
        var edges = new List<int[]>();

        for (var i = 2; i <= NodeCount; i++)
        {
            edges.Add([3, random.Next(1, i), i]);
        }

        for (var e = 0; e < NodeCount * 2; e++)
        {
            var u = random.Next(1, NodeCount + 1);
            var v = random.Next(1, NodeCount + 1);

            if (u == v)
            {
                continue;
            }

            edges.Add([random.Next(1, 4), u, v]);
        }

        _edges = [.. edges];
    }

    [Benchmark(Baseline = true)]
    public int BreadthFirstReachabilityCheck()
    {
        var alice = new List<int>[NodeCount];
        var bob = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            alice[i] = [];
            bob[i] = [];
        }

        var usedEdges = 0;

        foreach (var edge in _edges)
        {
            if (edge[0] != 3)
            {
                continue;
            }

            var (u, v) = (edge[1] - 1, edge[2] - 1);
            if (CanReach(alice, u, v))
            {
                continue;
            }

            Connect(alice, u, v);
            Connect(bob, u, v);
            usedEdges++;
        }

        usedEdges += ConnectOwnEdges(alice, ownerType: 1);
        usedEdges += ConnectOwnEdges(bob, ownerType: 2);

        return IsFullyConnected(alice) && IsFullyConnected(bob) ? _edges.Length - usedEdges : -1;

        int ConnectOwnEdges(List<int>[] adjacency, int ownerType)
        {
            var used = 0;

            foreach (var edge in _edges)
            {
                if (edge[0] != ownerType)
                {
                    continue;
                }

                var (u, v) = (edge[1] - 1, edge[2] - 1);
                if (CanReach(adjacency, u, v))
                {
                    continue;
                }

                Connect(adjacency, u, v);
                used++;
            }

            return used;
        }
    }

    private static void Connect(List<int>[] adjacency, int u, int v)
    {
        adjacency[u].Add(v);
        adjacency[v].Add(u);
    }

    private bool CanReach(List<int>[] adjacency, int source, int target)
    {
        var visited = new bool[NodeCount];
        var queue = new Queue<int>();
        queue.Enqueue(source);
        visited[source] = true;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == target)
            {
                return true;
            }

            foreach (var next in adjacency[current])
            {
                if (!visited[next])
                {
                    visited[next] = true;
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }

    private bool IsFullyConnected(List<int>[] adjacency) => CanReachEveryNode(adjacency);

    private bool CanReachEveryNode(List<int>[] adjacency)
    {
        var visited = new bool[NodeCount];
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited[0] = true;
        var visitedCount = 1;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var next in adjacency[current])
            {
                if (!visited[next])
                {
                    visited[next] = true;
                    visitedCount++;
                    queue.Enqueue(next);
                }
            }
        }

        return visitedCount == NodeCount;
    }

    [Benchmark]
    public int DisjointSetUnionFind()
    {
        var alice = new DisjointSet(NodeCount);
        var bob = new DisjointSet(NodeCount);
        var usedEdges = 0;

        foreach (var edge in _edges)
        {
            if (edge[0] != 3)
            {
                continue;
            }

            var (u, v) = (edge[1] - 1, edge[2] - 1);
            if (alice.IsConnected(u, v))
            {
                continue;
            }

            alice.Union(u, v);
            bob.Union(u, v);
            usedEdges++;
        }

        usedEdges += UnionOwnEdges(alice, ownerType: 1);
        usedEdges += UnionOwnEdges(bob, ownerType: 2);

        return IsFullyConnected(alice) && IsFullyConnected(bob) ? _edges.Length - usedEdges : -1;
    }

    private int UnionOwnEdges(DisjointSet components, int ownerType)
    {
        var used = 0;

        foreach (var edge in _edges)
        {
            if (edge[0] != ownerType)
            {
                continue;
            }

            var (u, v) = (edge[1] - 1, edge[2] - 1);
            if (components.IsConnected(u, v))
            {
                continue;
            }

            components.Union(u, v);
            used++;
        }

        return used;
    }

    private bool IsFullyConnected(DisjointSet components)
    {
        var root = components.Find(0);

        for (var i = 1; i < NodeCount; i++)
        {
            if (components.Find(i) != root)
            {
                return false;
            }
        }

        return true;
    }
}
