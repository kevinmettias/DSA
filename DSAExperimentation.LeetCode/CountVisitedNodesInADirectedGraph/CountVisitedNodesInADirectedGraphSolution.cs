using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

// LeetCode 2876. Count Visited Nodes in a Directed Graph: edges[i] gives node i its
// single outgoing edge, so the input is a functional graph and every walk
// eventually falls into a cycle. Report, for each start, how many DISTINCT nodes
// the walk visits before it first repeats one.
//
// CountVisitedNodesByPerStartWalk is the textbook answer: start again at every
// node and follow successors until a node repeats, with no memoization across
// starts - a shared cycle gets re-walked once from each node that reaches it, so
// O(n^2) in the worst case.
//
// CountVisitedNodesBySccAndReverseBfs reads the answer off the graph's structure
// instead, in two O(V+E) passes. Every strongly connected component with more than
// one member is a simple cycle (out-degree is exactly one, so nothing richer can
// fit), and each of its members visits exactly component.Count distinct nodes - so
// this repo's own Algorithms.Connectivity.StronglyConnectedComponents.Tarjan
// settles every on-cycle node at once, the same reduction Longest Cycle in a Graph
// (LC 2360) uses. A node NOT on a cycle answers "1 + its successor's own answer",
// which is a multi-source BFS seeded from the settled cycle nodes and walking the
// REVERSED edges outward with this repo's own Queue<int> as the frontier - the same
// "peel a Queue<int> frontier layer by layer" idiom Minimum Height Trees and
// TopologicalSort already establish, just seeded from known answers rather than
// from zero-in-degree leaves.
internal static class CountVisitedNodesInADirectedGraphSolution
{
    // A component of one node is a node on no cycle: LC 2876 guarantees
    // edges[i] != i, so there is no self-loop to make a singleton a length-1 cycle.
    private const int SmallestCycle = 2;

    // The textbook answer: walk forward from each start into a BCL HashSet and stop
    // the first time it refuses a node. Deliberately written with BCL types and
    // nothing else (ARCHITECTURE.md #17.5) - it is the arm the composed strategy
    // below has to justify itself against.
    public static int[] CountVisitedNodesByPerStartWalk(int[] edges)
    {
        var answer = new int[edges.Length];

        for (var start = 0; start < edges.Length; start++)
        {
            answer[start] = DistinctNodesFrom(start, edges);
        }

        return answer;
    }

    private static int DistinctNodesFrom(int start, int[] edges)
    {
        var visited = new HashSet<int>();
        var current = start;

        while (visited.Add(current))
        {
            current = edges[current];
        }

        return visited.Count;
    }

    // LeetCode's own input shape: edges[i] is node i's single successor.
    public static int[] CountVisitedNodesBySccAndReverseBfs(int[] edges) =>
        CountVisitedNodesBySccAndReverseBfs(FunctionalGraph.Build(edges));

    public static int[] CountVisitedNodesBySccAndReverseBfs(FunctionalGraph graph)
    {
        var components = StronglyConnectedComponents.Tarjan<
            FunctionalGraphNode, FunctionalGraphTopology, ListChildren<FunctionalGraphNode>,
            NaturalChildOrder<FunctionalGraphNode, ListChildren<FunctionalGraphNode>>,
            ListChildren<FunctionalGraphNode>>(graph.Nodes);

        var walk = new ReverseWalk(new int[graph.Edges.Length], new bool[graph.Edges.Length], new RepoQueue());

        SeedCycleAnswers(components, walk);
        PropagateToTails(BuildPredecessors(graph.Edges), walk);

        return walk.Answer;
    }

    // Every member of a cycle sees exactly the cycle, so all of them are settled
    // before any tail is, which is what lets the BFS below run outward only.
    private static void SeedCycleAnswers(List<List<FunctionalGraphNode>> components, ReverseWalk walk)
    {
        foreach (var component in components)
        {
            if (component.Count < SmallestCycle)
            {
                continue;
            }

            foreach (var node in component)
            {
                walk.Answer[node.Id] = component.Count;
                walk.Settled[node.Id] = true;
                walk.Frontier.Enqueue(node.Id);
            }
        }
    }

    private static void PropagateToTails(List<int>[] predecessors, ReverseWalk walk)
    {
        while (walk.Frontier.TryDequeue(out var current))
        {
            foreach (var predecessor in predecessors[current])
            {
                if (walk.Settled[predecessor])
                {
                    continue;
                }

                walk.Answer[predecessor] = walk.Answer[current] + 1;
                walk.Settled[predecessor] = true;
                walk.Frontier.Enqueue(predecessor);
            }
        }
    }

    private static List<int>[] BuildPredecessors(int[] edges)
    {
        var predecessors = new List<int>[edges.Length];

        for (var id = 0; id < edges.Length; id++)
        {
            predecessors[id] = [];
        }

        for (var id = 0; id < edges.Length; id++)
        {
            predecessors[edges[id]].Add(id);
        }

        return predecessors;
    }

    private readonly record struct ReverseWalk(int[] Answer, bool[] Settled, RepoQueue Frontier);
}
