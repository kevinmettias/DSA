using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

using AncestorNode = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyNode;
using AncestorTopology = DSAExperimentation.DataStructures.Graph.Adjacency.AdjacencyTopology;

namespace DSAExperimentation.LeetCode.Solutions.AllAncestorsOfANodeInADirectedAcyclicGraph;

// LC 2192's promise about its input: the graph is directed and acyclic. LeetCode's judge never poses an
// input that breaks it. On a cycle the forward walk counts a node among its own ancestors and the
// topological pass leaves the cycle's nodes out. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class AllAncestorsOfANodeInADirectedAcyclicGraphPromise
{
    // A graph is acyclic exactly when Kahn's algorithm can order all of its nodes.
    public static bool IsKeptBy(int nodeCount, int[][] edges)
    {
        var nodes = AllAncestorsOfANodeInADirectedAcyclicGraphSolution.BuildNodes(nodeCount, edges);
        return TopologicalSort.TrySort<AncestorNode, AncestorTopology, ListChildren<AncestorNode>>(nodes, out _);
    }
}
