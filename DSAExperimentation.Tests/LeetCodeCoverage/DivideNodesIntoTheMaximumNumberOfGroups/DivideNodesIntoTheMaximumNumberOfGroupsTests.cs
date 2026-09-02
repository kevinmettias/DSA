using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.KeyedDisjointSet;
using DSAExperimentation.Tests.LeetCodeCoverage.DivideNodesIntoTheMaximumNumberOfGroups.Fixtures;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivideNodesIntoTheMaximumNumberOfGroups;

// LeetCode 2493. Divide Nodes Into the Maximum Number of Groups: -1 unless every
// connected component is bipartite (this repo's own BipartiteCheck.IsBipartite,
// the exact PossibleBipartitionTests/IsGraphBipartiteTests composition, run once
// over every node so it can fail fast on the first bad component). Otherwise the
// best grouping for a bipartite component is its largest BFS eccentricity plus
// one, maximized over every possible root - the same Reduce.Graph +
// DistanceMapReduceAlgebra + BreadthFirstReduceOrder composition
// WordLadderTests/MinimumGeneticMutationTests already use for shortest-path
// distance maps, just read for its max value instead of one target's value, and
// replayed once per node instead of once per query. KeyedDisjointSet<int> groups
// nodes into components (union over the edge list) so each component's best
// grouping is tracked and summed separately, the same "union on shared
// membership" shape FindAllPeopleWithSecretTests already uses for a different key
// space.
public sealed partial class DivideNodesIntoTheMaximumNumberOfGroupsTests
{
    [Fact]
    public void MagnificentSets_ClassicExample_ReturnsFourGroups()
    {
        int[][] edges = [[1, 2], [1, 4], [1, 5], [2, 6], [2, 3], [4, 6]];

        Assert.Equal(4, MagnificentSets(n: 6, edges));
    }

    [Fact]
    public void MagnificentSets_OddCycle_ReturnsNegativeOne()
    {
        int[][] edges = [[1, 2], [2, 3], [3, 1]];

        Assert.Equal(-1, MagnificentSets(n: 3, edges));
    }

    [Fact]
    public void MagnificentSets_TwoDisjointBipartiteComponents_SumsEachComponentsBestGrouping()
    {
        // Component {1,2,3} is a 3-node path (best grouping: 3, rooted at either
        // end). Component {4,5,6,7,8} is a 5-node path (best grouping: 5, rooted
        // at either end) - the answer sums both components' independent bests
        // rather than taking one BFS root's depth across the whole graph.
        int[][] edges = [[1, 2], [2, 3], [4, 5], [5, 6], [6, 7], [7, 8]];

        Assert.Equal(8, MagnificentSets(n: 8, edges));
    }

    private static int MagnificentSets(int n, int[][] edges)
    {
        var nodes = BuildGraph(n, edges);

        if (!BipartiteCheckOperations.IsBipartite<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>>(nodes))
        {
            return -1;
        }

        var components = new KeyedDisjointSet<int>(Enumerable.Range(1, n));
        foreach (var edge in edges)
        {
            components.TryUnion(edge[0], edge[1]);
        }

        var bestGroupsByComponent = new Dictionary<int, int>();
        foreach (var node in nodes)
        {
            var candidateGroups = EccentricityFrom(node) + 1;
            components.TryFind(node.Id, out var componentRoot);

            if (!bestGroupsByComponent.TryGetValue(componentRoot, out var best) || candidateGroups > best)
            {
                bestGroupsByComponent[componentRoot] = candidateGroups;
            }
        }

        return bestGroupsByComponent.Values.Sum();
    }

    private static int EccentricityFrom(GroupNode root)
    {
        var distances = Reduce.Graph<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>,
            BreadthFirstReduceOrder<GroupNode>,
            DistanceMapReduceAlgebra<GroupNode>, Dictionary<GroupNode, int>>(root);

        return distances.Values.Max();
    }

    private static List<GroupNode> BuildGraph(int n, int[][] edges)
    {
        // Index 0 is an unused placeholder so node ids can stay 1-indexed,
        // matching LeetCode's own numbering; it carries no edges and is dropped
        // before the graph is returned.
        var nodes = Enumerable.Range(0, n + 1).Select(id => new GroupNode(id)).ToList();

        foreach (var edge in edges)
        {
            nodes[edge[0]].Neighbors.Add(nodes[edge[1]]);
            nodes[edge[1]].Neighbors.Add(nodes[edge[0]]);
        }

        return nodes.Skip(1).ToList();
    }
}
