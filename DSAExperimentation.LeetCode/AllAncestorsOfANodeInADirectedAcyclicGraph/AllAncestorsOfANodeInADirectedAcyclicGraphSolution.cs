using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.AllAncestorsOfANodeInADirectedAcyclicGraph;

// LeetCode 2192. All Ancestors of a Node in a Directed Acyclic Graph: for every
// node, the ascending list of nodes that can reach it.
//
// The naive baseline re-derives every node's contribution from scratch with one
// fresh DFS PER start node, O(V * (V+E)), since a deeply nested node sits inside
// many other nodes' walks. The composed strategy orders the graph source-to-sink
// with this repo's own Kahn's-algorithm TopologicalSort.TrySort and then threads
// each node's own ancestor set forward across its outgoing edges in one linear
// pass: by the time a node is reached in topological order every one of ITS
// ancestors is already final, so unioning {node} + node's ancestors onto each
// child once is enough.
internal static class AllAncestorsOfANodeInADirectedAcyclicGraphSolution
{
    // The textbook answer: walk forward from every node in turn and record that
    // node as an ancestor of everything the walk reaches. Deliberately written
    // with a BCL Stack and HashSet and no ordering machinery - it is the arm the
    // composed strategy below has to justify itself against.
    public static List<List<int>> GetAncestorsByPerNodeForwardWalk(int n, int[][] edges)
    {
        var nodes = BuildNodes(n, edges);

        return GetAncestorsByPerNodeForwardWalk(nodes);
    }

    public static List<List<int>> GetAncestorsByPerNodeForwardWalk(List<AncestorNode> nodes)
    {
        var ancestors = CreateEmptyAncestorLists(nodes.Count);

        foreach (var start in nodes)
        {
            RecordStartAsAncestorOfItsDescendants(start, ancestors);
        }

        foreach (var list in ancestors)
        {
            list.Sort();
        }

        return ancestors;
    }

    private static void RecordStartAsAncestorOfItsDescendants(AncestorNode start, List<List<int>> ancestors)
    {
        var visited = new HashSet<AncestorNode>();
        var stack = new Stack<AncestorNode>();

        PushUnvisitedChildren(start, visited, stack);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            ancestors[node.Id].Add(start.Id);

            PushUnvisitedChildren(node, visited, stack);
        }
    }

    private static void PushUnvisitedChildren(
        AncestorNode node, HashSet<AncestorNode> visited, Stack<AncestorNode> stack)
    {
        foreach (var child in node.Children)
        {
            if (visited.Add(child))
            {
                stack.Push(child);
            }
        }
    }

    private static List<List<int>> CreateEmptyAncestorLists(int count)
    {
        var ancestors = new List<List<int>>(count);

        for (var id = 0; id < count; id++)
        {
            ancestors.Add([]);
        }

        return ancestors;
    }

    // One Kahn's-algorithm ordering plus one linear DP pass over it, O(V+E) walks
    // rather than V of them. Ancestor sets are this repo's own HashMap<int,bool>
    // rather than Set<Element>, because the answer has to be read back out and
    // HashMap exposes Keys while Set does not - which is also how Set itself is
    // built.
    public static List<List<int>> GetAncestorsByTopologicalDpPass(int n, int[][] edges)
    {
        var nodes = BuildNodes(n, edges);

        return GetAncestorsByTopologicalDpPass(nodes);
    }

    public static List<List<int>> GetAncestorsByTopologicalDpPass(List<AncestorNode> nodes)
    {
        TopologicalSort.TrySort<
            AncestorNode, AncestorTopology, ListChildren<AncestorNode>,
            NaturalChildOrder<AncestorNode, ListChildren<AncestorNode>>, ListChildren<AncestorNode>>(
            nodes, out var ordering);

        var ancestorSets = CreateEmptyAncestorSets(nodes.Count);
        PropagateAncestors(ordering, ancestorSets);

        return SortedAncestorLists(ancestorSets);
    }

    private static HashMap<int, bool>[] CreateEmptyAncestorSets(int count)
    {
        var ancestorSets = new HashMap<int, bool>[count];

        for (var id = 0; id < count; id++)
        {
            ancestorSets[id] = new HashMap<int, bool>();
        }

        return ancestorSets;
    }

    private static void PropagateAncestors(List<AncestorNode> ordering, HashMap<int, bool>[] ancestorSets)
    {
        foreach (var node in ordering)
        {
            // Snapshotted once per node, not once per child: HashMap<TKey,TValue>.Keys
            // materializes a fresh List on every access (see its own doc comment), and
            // this node's own ancestor set never changes while its children are being
            // updated below.
            var nodeAncestors = ancestorSets[node.Id].Keys.ToList();

            foreach (var child in node.Children)
            {
                PropagateToChild(ancestorSets[child.Id], node.Id, nodeAncestors);
            }
        }
    }

    private static void PropagateToChild(HashMap<int, bool> childAncestors, int nodeId, List<int> nodeAncestors)
    {
        childAncestors.Set(nodeId, true);

        foreach (var ancestor in nodeAncestors)
        {
            childAncestors.Set(ancestor, true);
        }
    }

    private static List<List<int>> SortedAncestorLists(HashMap<int, bool>[] ancestorSets)
    {
        var ancestors = new List<List<int>>(ancestorSets.Length);

        foreach (var set in ancestorSets)
        {
            var list = set.Keys.ToList();
            list.Sort();
            ancestors.Add(list);
        }

        return ancestors;
    }

    // LeetCode's own input shape: edges[i] = [fromi, toi], an edge from ancestor
    // fromi to descendant toi over nodes 0..n-1.
    private static List<AncestorNode> BuildNodes(int n, int[][] edges)
    {
        var nodes = new List<AncestorNode>(n);

        for (var id = 0; id < n; id++)
        {
            nodes.Add(new AncestorNode(id));
        }

        foreach (var edge in edges)
        {
            nodes[edge[0]].Children.Add(nodes[edge[1]]);
        }

        return nodes;
    }
}
