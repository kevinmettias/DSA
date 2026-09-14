using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DivideNodesIntoTheMaximumNumberOfGroups;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// DivideNodesIntoTheMaximumNumberOfGroupsSolution's, the same methods
// DivideNodesIntoTheMaximumNumberOfGroupsTests proves correct - the hand-rolled
// adjacency-array walks against BipartiteCheck + KeyedDisjointSet + Reduce.Graph.
// Each arm is handed the prepared input its hoisted overload takes, so building
// the graph is charged to [GlobalSetup] rather than to the search being measured.
//
// The workload is a random spanning tree: connected and (like every tree) always
// bipartite, so every node genuinely runs its own full BFS instead of any arm
// short-circuiting early, and there is exactly one component to sum over.
[MemoryDiagnoser]
public class DivideNodesIntoTheMaximumNumberOfGroupsBenchmarks
{
    // LC problem number, reused as the deterministic edge seed.
    private const int RandomSeed = 2493;

    private GroupAdjacency _adjacency = null!;
    private GroupGraph _graph = null!;

    [Params(50, 200)]
    public int NodeCount;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var edges = BuildRandomSpanningTreeEdges(NodeCount, random);

        _adjacency = GroupAdjacency.Build(NodeCount, edges);
        _graph = GroupGraph.Build(NodeCount, edges);
    }

    // Every node after the first links to a uniformly random earlier node - a
    // standard random-recursive-tree construction, over LeetCode's own 1..n
    // numbering.
    private static int[][] BuildRandomSpanningTreeEdges(int nodeCount, Random random)
    {
        var edges = new int[nodeCount - 1][];

        for (var node = 1; node < nodeCount; node++)
        {
            edges[node - 1] = [node + 1, random.Next(node) + 1];
        }

        return edges;
    }

    [Benchmark(Baseline = true)]
    public int ArrayAdjacencyBruteForce() =>
        DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByArrayAdjacencyBfs(_adjacency);

    [Benchmark]
    public int ReducePrimitivesComposition() =>
        DivideNodesIntoTheMaximumNumberOfGroupsSolution.MagnificentSetsByReducePrimitives(_graph);
}
