using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.Tests.LeetCodeCoverage.AllAncestorsOfANodeInADirectedAcyclicGraph.Fixtures;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllAncestorsOfANodeInADirectedAcyclicGraph;

// LeetCode 2192. All Ancestors of a Node in a Directed Acyclic Graph: order every node
// source-to-sink with this repo's own Kahn's-algorithm TopologicalSort.TrySort (the
// exact CourseScheduleII/LoudAndRich precedent), then thread each node's own ancestor
// set forward across its outgoing edges in one linear DP pass - by the time a node is
// popped in topological order, every one of ITS OWN ancestors is already finalized (they
// all precede it in the order), so unioning {node} + node's ancestor set onto each child
// once is enough; no need to re-walk from every node individually. Ancestor sets are
// this repo's own HashMap<int,bool>, not Set<Element> (which doesn't expose an
// enumerable view of its contents) - membership via HasKey/Set, read back out via Keys,
// exactly how Set<Element> itself is built on top of HashMap.
public sealed partial class AllAncestorsOfANodeInADirectedAcyclicGraphTests
{
    [Fact]
    public void GetAncestors_ClassicExample_ReturnsAncestorsSortedAscending()
    {
        int[][] edges = [[0, 3], [0, 4], [1, 3], [2, 4], [2, 7], [3, 5], [3, 6], [3, 7], [4, 6]];

        var ancestors = GetAncestors(8, edges);

        Assert.Equal([], ancestors[0]);
        Assert.Equal([], ancestors[1]);
        Assert.Equal([], ancestors[2]);
        Assert.Equal([0, 1], ancestors[3]);
        Assert.Equal([0, 2], ancestors[4]);
        Assert.Equal([0, 1, 3], ancestors[5]);
        Assert.Equal([0, 1, 2, 3, 4], ancestors[6]);
        Assert.Equal([0, 1, 2, 3], ancestors[7]);
    }

    [Fact]
    public void GetAncestors_NoEdges_EveryNodeHasNoAncestors()
    {
        var ancestors = GetAncestors(5, []);

        Assert.All(ancestors, Assert.Empty);
    }

    [Fact]
    public void GetAncestors_SingleChain_EachNodeInheritsEveryEarlierNode()
    {
        int[][] edges = [[0, 1], [1, 2], [2, 3]];

        var ancestors = GetAncestors(4, edges);

        Assert.Equal([], ancestors[0]);
        Assert.Equal([0], ancestors[1]);
        Assert.Equal([0, 1], ancestors[2]);
        Assert.Equal([0, 1, 2], ancestors[3]);
    }

    private static List<List<int>> GetAncestors(int n, int[][] edges)
    {
        var nodes = BuildNodes(n, edges);

        TopologicalSort.TrySort<
            AncestorNode, AncestorTopology, ListChildren<AncestorNode>,
            NaturalChildOrder<AncestorNode, ListChildren<AncestorNode>>, ListChildren<AncestorNode>>(
            nodes, out var ordering);

        var ancestorSets = BuildAncestorSets(n, ordering);

        return BuildSortedResult(n, ancestorSets);
    }

    private static HashMap<int, bool>[] BuildAncestorSets(int n, List<AncestorNode> ordering)
    {
        var ancestorSets = CreateEmptyAncestorSets(n);

        foreach (var node in ordering)
        {
            // Snapshotted once per node, not once per child: HashMap<TKey,TValue>.Keys
            // materializes a fresh List on every access (see its own doc comment), and
            // node's own ancestor set never changes while its children are being
            // updated below.
            var nodeAncestors = ancestorSets[node.Id].Keys.ToList();

            foreach (var child in node.Children)
            {
                PropagateAncestorsToChild(ancestorSets, node.Id, nodeAncestors, child);
            }
        }

        return ancestorSets;
    }

    private static HashMap<int, bool>[] CreateEmptyAncestorSets(int n)
    {
        var ancestorSets = new HashMap<int, bool>[n];

        for (var i = 0; i < n; i++)
        {
            ancestorSets[i] = new HashMap<int, bool>();
        }

        return ancestorSets;
    }

    private static void PropagateAncestorsToChild(
        HashMap<int, bool>[] ancestorSets, int nodeId, List<int> nodeAncestors, AncestorNode child)
    {
        ancestorSets[child.Id].Set(nodeId, true);

        foreach (var ancestor in nodeAncestors)
        {
            ancestorSets[child.Id].Set(ancestor, true);
        }
    }

    private static List<List<int>> BuildSortedResult(int n, HashMap<int, bool>[] ancestorSets)
    {
        var result = new List<List<int>>(n);

        for (var i = 0; i < n; i++)
        {
            var list = ancestorSets[i].Keys.ToList();
            list.Sort();
            result.Add(list);
        }

        return result;
    }

    private static List<AncestorNode> BuildNodes(int n, int[][] edges)
    {
        var nodes = new List<AncestorNode>(n);
        for (var i = 0; i < n; i++)
        {
            nodes.Add(new AncestorNode(i));
        }

        foreach (var edge in edges)
        {
            nodes[edge[0]].Children.Add(nodes[edge[1]]);
        }

        return nodes;
    }
}
