using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game IV (LC 1345): the textbook BFS directly over the array - a Queue<int>,
// a same-value index map, and the standard "clear the group once every member has
// been enqueued" trick that keeps it from re-scanning a large equal-value run on
// every visit - vs. modeling the exact same i+1/i-1/same-value reachability as an
// implicit unweighted-hop graph (this repo's WeightedGraphNode/WeightedGraphTopology,
// the same fixtures JumpGameIIBenchmarks uses) and answering it with this repo's own
// ShortestPath.Dijkstra. Values are drawn from a range five times narrower than
// Length so equal-value groups stay small (dense enough to matter, not so dense the
// hop graph's edge count explodes past O(n)).
[MemoryDiagnoser]
public class JumpGameIVBenchmarks
{
    private const int RandomSeed = 1345; // LeetCode problem number

    private const int ValueRangeDivisor = 5;

    [Params(200, 2_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            var upperBoundExclusive = Math.Max(1, Length / ValueRangeDivisor);
            _arr[i] = random.Next(0, upperBoundExclusive);
        }
    }

    private readonly record struct BfsContext(int N, Dictionary<int, List<int>> IndicesByValue, bool[] Visited, Queue<int> Frontier);

    [Benchmark(Baseline = true)]
    public int BfsWithGroupPruning()
    {
        var context = CreateBfsContext(_arr.Length);
        return RunBfs(context);
    }

    private BfsContext CreateBfsContext(int n)
    {
        var indicesByValue = GroupIndicesByValue(n);
        var visited = new bool[n];
        visited[0] = true;
        var frontier = new Queue<int>();
        frontier.Enqueue(0);

        return new BfsContext(n, indicesByValue, visited, frontier);
    }

    private int RunBfs(BfsContext context)
    {
        var steps = 0;

        while (context.Frontier.Count > 0)
        {
            var levelSize = context.Frontier.Count;

            for (var k = 0; k < levelSize; k++)
            {
                var i = context.Frontier.Dequeue();

                if (ProcessFrontierNode(i, context))
                {
                    return steps;
                }
            }

            steps++;
        }

        return -1;
    }

    private Dictionary<int, List<int>> GroupIndicesByValue(int n)
    {
        var indicesByValue = new Dictionary<int, List<int>>();

        for (var i = 0; i < n; i++)
        {
            if (!indicesByValue.TryGetValue(_arr[i], out var indices))
            {
                indices = [];
                indicesByValue[_arr[i]] = indices;
            }

            indices.Add(i);
        }

        return indicesByValue;
    }

    private bool ProcessFrontierNode(int i, BfsContext context)
    {
        if (i == context.N - 1)
        {
            return true;
        }

        if (context.IndicesByValue.TryGetValue(_arr[i], out var sameValue))
        {
            foreach (var j in sameValue)
            {
                ExpandNeighbor(j, context);
            }

            context.IndicesByValue.Remove(_arr[i]);
        }

        ExpandNeighbor(i + 1, context);
        ExpandNeighbor(i - 1, context);

        return false;
    }

    private static void ExpandNeighbor(int target, BfsContext context)
    {
        if (target < 0 || target >= context.N || context.Visited[target])
        {
            return;
        }

        context.Visited[target] = true;
        context.Frontier.Enqueue(target);
    }

    [Benchmark]
    public int DijkstraOverHopGraph()
    {
        var n = _arr.Length;
        var indicesByValue = GroupIndicesByValue(n);
        var nodes = BuildHopGraph(n, indicesByValue);

        var distances = ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }

    private WeightedGraphNode[] BuildHopGraph(int n, Dictionary<int, List<int>> indicesByValue)
    {
        var nodes = CreateNodes(n);

        for (var i = 0; i < n; i++)
        {
            ConnectSequentialNeighbors(nodes, i, n);
            ConnectSameValueNeighbors(nodes, i, indicesByValue[_arr[i]]);
        }

        return nodes;
    }

    private static WeightedGraphNode[] CreateNodes(int n)
    {
        var nodes = new WeightedGraphNode[n];

        for (var i = 0; i < n; i++)
        {
            nodes[i] = new WeightedGraphNode(i);
        }

        return nodes;
    }

    private static void ConnectSequentialNeighbors(WeightedGraphNode[] nodes, int i, int n)
    {
        if (i + 1 < n)
        {
            nodes[i].Edges.Add((1, nodes[i + 1]));
        }

        if (i - 1 >= 0)
        {
            nodes[i].Edges.Add((1, nodes[i - 1]));
        }
    }

    private static void ConnectSameValueNeighbors(WeightedGraphNode[] nodes, int i, List<int> sameValueIndices)
    {
        foreach (var j in sameValueIndices)
        {
            if (j != i)
            {
                nodes[i].Edges.Add((1, nodes[j]));
            }
        }
    }
}
