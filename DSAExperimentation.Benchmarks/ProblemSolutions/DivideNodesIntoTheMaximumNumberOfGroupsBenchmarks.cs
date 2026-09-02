using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.KeyedDisjointSet;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Divide Nodes Into the Maximum Number of Groups (LC 2493): a hand-rolled
// adjacency-array bipartite coloring (Queue<int>, sbyte[] color) plus a
// hand-rolled int[] union-find for connectivity plus a hand-rolled per-node BFS
// eccentricity scan (int[] distance array, Queue<int> frontier) vs. this repo's
// own composition - BipartiteCheck.IsBipartite (the same primitive
// PossibleBipartitionBenchmarks/IsGraphBipartiteBenchmarks already prove),
// KeyedDisjointSet<int> for component grouping (FindAllPeopleWithSecretTests' own
// primitive, used here as a benchmark for the first time), and Reduce.Graph +
// BreadthFirstReduceOrder + DistanceMapReduceAlgebra for each root's distance map
// (MinimumGeneticMutationBenchmarks'/WordLadderBenchmarks' own composition, tried
// once per node here instead of once per query). _nodes is a random spanning
// tree - connected and bipartite by construction, so every node genuinely runs
// its own full BFS instead of any strategy short-circuiting early, and there is
// exactly one component to sum over.
[MemoryDiagnoser]
public class DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks
{
    private const int RandomSeed = 2493;

    [Params(50, 200)]
    public int NodeCount;

    private int[][] _adjacency = null!;
    private List<GraphNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = BuildRandomSpanningTreeEdges(NodeCount, random);

        _adjacency = BuildAdjacency(NodeCount, edges);
        _nodes = BuildGraph(NodeCount, edges);
    }

    // Every node i > 0 links to a uniformly random earlier node - a standard
    // random-recursive-tree construction, connected and (like every tree) always
    // bipartite by construction.
    private static List<(int A, int B)> BuildRandomSpanningTreeEdges(int nodeCount, Random random)
    {
        var edges = new List<(int A, int B)>();

        for (var i = 1; i < nodeCount; i++)
        {
            edges.Add((i, random.Next(i)));
        }

        return edges;
    }

    private static int[][] BuildAdjacency(int nodeCount, List<(int A, int B)> edges)
    {
        var adjacency = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();

        foreach (var (a, b) in edges)
        {
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency.Select(neighbors => neighbors.ToArray()).ToArray();
    }

    private static List<GraphNode> BuildGraph(int nodeCount, List<(int A, int B)> edges)
    {
        var nodes = Enumerable.Range(0, nodeCount).Select(id => new GraphNode(id)).ToList();

        foreach (var (a, b) in edges)
        {
            nodes[a].Neighbors.Add(nodes[b]);
            nodes[b].Neighbors.Add(nodes[a]);
        }

        return nodes;
    }

    [Benchmark(Baseline = true)]
    public int ArrayAdjacencyBruteForce()
    {
        if (!IsBipartiteByArrayBfs())
        {
            return -1;
        }

        var componentRoots = ComputeComponentRoots();
        var bestByComponent = new Dictionary<int, int>();

        for (var node = 0; node < _adjacency.Length; node++)
        {
            var candidate = EccentricityByArrayBfs(node) + 1;
            var root = componentRoots[node];

            if (!bestByComponent.TryGetValue(root, out var best) || candidate > best)
            {
                bestByComponent[root] = candidate;
            }
        }

        return bestByComponent.Values.Sum();
    }

    private bool IsBipartiteByArrayBfs()
    {
        var color = new sbyte[_adjacency.Length];

        for (var start = 0; start < _adjacency.Length; start++)
        {
            if (color[start] != 0)
            {
                continue;
            }

            if (!ColorComponent(start, color))
            {
                return false;
            }
        }

        return true;
    }

    private bool ColorComponent(int start, sbyte[] color)
    {
        var queue = new Queue<int>();
        color[start] = 1;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var neighbor in _adjacency[node])
            {
                if (color[neighbor] == 0)
                {
                    color[neighbor] = (sbyte)-color[node];
                    queue.Enqueue(neighbor);
                }
                else if (color[neighbor] == color[node])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private int EccentricityByArrayBfs(int start)
    {
        var distance = new int[_adjacency.Length];
        Array.Fill(distance, -1);
        distance[start] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(start);
        var maxDistance = 0;

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();

            foreach (var neighbor in _adjacency[node])
            {
                if (distance[neighbor] != -1)
                {
                    continue;
                }

                distance[neighbor] = distance[node] + 1;
                maxDistance = Math.Max(maxDistance, distance[neighbor]);
                queue.Enqueue(neighbor);
            }
        }

        return maxDistance;
    }

    private int[] ComputeComponentRoots()
    {
        var parent = Enumerable.Range(0, _adjacency.Length).ToArray();

        for (var node = 0; node < _adjacency.Length; node++)
        {
            foreach (var neighbor in _adjacency[node])
            {
                Union(parent, node, neighbor);
            }
        }

        var roots = new int[_adjacency.Length];

        for (var node = 0; node < _adjacency.Length; node++)
        {
            roots[node] = Find(parent, node);
        }

        return roots;
    }

    private static int Find(int[] parent, int x)
    {
        while (parent[x] != x)
        {
            parent[x] = parent[parent[x]];
            x = parent[x];
        }

        return x;
    }

    private static void Union(int[] parent, int a, int b)
    {
        var rootA = Find(parent, a);
        var rootB = Find(parent, b);

        if (rootA != rootB)
        {
            parent[rootA] = rootB;
        }
    }

    [Benchmark]
    public int ReducePrimitivesComposition()
    {
        if (!BipartiteCheckOperations.IsBipartite<
            GraphNode, GraphTopology, ListChildren<GraphNode>,
            NaturalChildOrder<GraphNode, ListChildren<GraphNode>>, ListChildren<GraphNode>>(_nodes))
        {
            return -1;
        }

        var components = new KeyedDisjointSet<int>(Enumerable.Range(0, _nodes.Count));

        foreach (var node in _nodes)
        {
            foreach (var neighbor in node.Neighbors)
            {
                components.TryUnion(node.Id, neighbor.Id);
            }
        }

        var bestByComponent = new Dictionary<int, int>();

        foreach (var node in _nodes)
        {
            var distances = Reduce.Graph<
                GraphNode, GraphTopology, ListChildren<GraphNode>,
                NaturalChildOrder<GraphNode, ListChildren<GraphNode>>, ListChildren<GraphNode>,
                BreadthFirstReduceOrder<GraphNode>,
                DistanceMapReduceAlgebra<GraphNode>, Dictionary<GraphNode, int>>(node);

            var candidate = distances.Values.Max() + 1;
            components.TryFind(node.Id, out var root);

            if (!bestByComponent.TryGetValue(root, out var best) || candidate > best)
            {
                bestByComponent[root] = candidate;
            }
        }

        return bestByComponent.Values.Sum();
    }

    private sealed class GraphNode(int id)
    {
        public int Id { get; } = id;

        public List<GraphNode> Neighbors { get; } = [];
    }

    private readonly struct GraphTopology : IGraphTopology<GraphNode, ListChildren<GraphNode>>
    {
        public static ListChildren<GraphNode> GetChildren(GraphNode node) => new(node.Neighbors);
    }
}
