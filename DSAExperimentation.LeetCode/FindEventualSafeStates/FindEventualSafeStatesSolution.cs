using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.FindEventualSafeStates;

// LeetCode 802. Find Eventual Safe States: a node is safe iff every path out of it
// eventually reaches a terminal node (no path loops forever). The answer is the
// ascending list of safe node ids.
//
// The composed strategy reframes the question as Kahn's algorithm over the REVERSED
// graph - each node's "children" are its predecessors - seeded from terminal nodes
// (out-degree zero, i.e. reversed in-degree zero) and peeled backward: this repo's
// own TopologicalSort.TrySort already IS that peel. Its `ordering` out-parameter is
// exactly the safe nodes regardless of TrySort's own bool result (which only says
// whether every node was safe, i.e. the graph was acyclic) - the leftover,
// never-peeled nodes are exactly the unsafe ones, the same "cycle announces itself
// as leftover in-degree" property TrySort's own doc comment already names.
internal static class FindEventualSafeStatesSolution
{
    private const int ColorUnvisited = 0;
    private const int ColorInProgress = 1;
    private const int ColorSafe = 2;
    private const int ColorUnsafe = 3;

    // The textbook answer: a per-node three-color DFS (unvisited / in-progress /
    // resolved), where meeting an in-progress node is the cycle that makes the whole
    // branch unsafe. Deliberately written without this repo's TopologicalSort - it is
    // the arm the composed solution below has to justify itself against. Nodes are
    // scanned in ascending id order, so the collected ids come out sorted for free.
    public static int[] EventualSafeNodesByDfsThreeColoring(int[][] graph)
    {
        var color = new int[graph.Length];
        var safe = new List<int>();

        for (var id = 0; id < graph.Length; id++)
        {
            if (IsSafe(graph, id, color))
            {
                safe.Add(id);
            }
        }

        return safe.ToArray();
    }

    private static bool IsSafe(int[][] graph, int node, int[] color)
    {
        if (color[node] != ColorUnvisited)
        {
            return color[node] == ColorSafe;
        }

        color[node] = ColorInProgress;

        foreach (var next in graph[node])
        {
            if (!IsSafe(graph, next, color))
            {
                color[node] = ColorUnsafe;
                return false;
            }
        }

        color[node] = ColorSafe;
        return true;
    }

    // LeetCode's own input shape: graph[i] lists the nodes reachable in one step
    // from i.
    public static int[] EventualSafeNodesByReversedKahnsTopologicalSort(int[][] graph) =>
        EventualSafeNodesByReversedKahnsTopologicalSort(BuildReversedGraph(graph));

    public static int[] EventualSafeNodesByReversedKahnsTopologicalSort(List<SafeStateNode> nodes)
    {
        TopologicalSort.TrySort<
            SafeStateNode, SafeStateTopology, ListChildren<SafeStateNode>,
            NaturalChildOrder<SafeStateNode, ListChildren<SafeStateNode>>, ListChildren<SafeStateNode>>(
            nodes, out var ordering);

        return ordering.Select(node => node.Id).OrderBy(id => id).ToArray();
    }

    // Reverses every edge as it goes, so the peel below starts from the terminal
    // nodes: after this, node i's children are the nodes that point AT i.
    public static List<SafeStateNode> BuildReversedGraph(int[][] graph)
    {
        var nodes = new List<SafeStateNode>(graph.Length);

        for (var id = 0; id < graph.Length; id++)
        {
            nodes.Add(new SafeStateNode(id));
        }

        for (var id = 0; id < graph.Length; id++)
        {
            foreach (var next in graph[id])
            {
                nodes[next].Predecessors.Add(nodes[id]);
            }
        }

        return nodes;
    }
}
