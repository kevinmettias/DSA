using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.LongestCycleInAGraph;

// LeetCode 2360. Longest Cycle in a Graph: edges[i] gives node i its single
// outgoing edge, or -1 for none, so the input is a functional graph - every node
// has out-degree at most one. Report the length of the longest cycle, or -1 when
// there is no cycle anywhere.
//
// That out-degree bound is what collapses the problem: with at most one edge
// leaving each node, a strongly connected component can hold no more structure
// than one simple cycle, so "longest cycle" is just "largest component with more
// than one member".
//
// The two strategies differ in how they find the cycles. LongestCycleByPerStartWalk
// is the textbook forward walk from every node in turn, with no memoization across
// starts - a shared cycle gets re-walked once from each node that reaches it.
// LongestCycleByTarjanComponents hands the graph to this repo's own
// Algorithms.Connectivity.StronglyConnectedComponents.Tarjan and reads the answer
// off the component sizes in one O(V+E) pass.
internal static class LongestCycleInAGraphSolution
{
    // A component of one node is a node on no cycle: edges[i] != i is guaranteed,
    // so there are no self-loops to make a singleton component a length-1 cycle.
    private const int SmallestCycle = 2;

    // The textbook answer: walk forward from each start, recording the step at
    // which each node was first seen, and read the cycle length off the step
    // difference when the walk revisits a node. Deliberately written with a BCL
    // Dictionary over raw int indices and nothing else - it is the arm the composed
    // strategy below has to justify itself against.
    public static int LongestCycleByPerStartWalk(int[] edges)
    {
        var longest = LeetCodeAnswer.None;

        for (var start = 0; start < edges.Length; start++)
        {
            var cycleLength = CycleLengthFrom(start, edges);
            longest = Math.Max(longest, cycleLength);
        }

        return longest;
    }

    // The length of the cycle this walk falls into, or -1 if it runs off the end of
    // the graph first. A walk that enters a cycle it did not start on still measures
    // that cycle correctly, because the step counter is only ever differenced
    // against the step of the node the walk repeats.
    private static int CycleLengthFrom(int start, int[] edges)
    {
        var stepOf = new Dictionary<int, int>();
        var current = start;
        var step = 0;

        while (current != FunctionalGraphEdges.NoOutgoingEdge && !stepOf.ContainsKey(current))
        {
            stepOf[current] = step++;
            current = edges[current];
        }

        return current == FunctionalGraphEdges.NoOutgoingEdge
            ? LeetCodeAnswer.None
            : ElapsedSteps(step, stepOf[current]);
    }

    // The cycle's length is how many steps the walk took between first seeing the
    // repeated node and arriving back at it.
    private static int ElapsedSteps(int step, int stepAtRepeat) => step - stepAtRepeat;

    // LeetCode's own input shape: edges[i] is node i's single successor, or -1.
    public static int LongestCycleByTarjanComponents(int[] edges) =>
        LongestCycleByTarjanComponents(FunctionalGraph.Build(edges));

    public static int LongestCycleByTarjanComponents(FunctionalGraph graph)
    {
        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>, ListChildren<FunctionalGraphNode>>(
            graph.Nodes);

        var longest = LeetCodeAnswer.None;

        foreach (var component in components)
        {
            if (component.Count >= SmallestCycle)
            {
                longest = Math.Max(longest, component.Count);
            }
        }

        return longest;
    }
}
