namespace DSAExperimentation.LeetCode.ShortestPathVisitingAllNodes;

// The whole (node, visited-mask) state space of one LC 847 input graph: every
// node paired with every subset of nodes, wired so that taking an input edge
// from `node` lands on the neighbor with that neighbor's bit set. StartNodes
// holds the n states a walk may begin in - node i having visited only itself -
// and FullMask is the state every walk is trying to reach.
//
// The domain model, not an answer to any one query about it: it knows which
// states are one step apart and nothing about what a caller wants to find out,
// the same framing DigitStepGraph uses for LC 3377. It is also the prepared
// input ShortestPathVisitingAllNodesSolution's hoisted overload takes, so a
// benchmark can charge this construction to [GlobalSetup] (ARCHITECTURE.md
// 17.4) - and it is deliberately not an IEnumerable, so the two overloads can
// never be ambiguous.
internal sealed class VisitStateGraph
{
    private VisitStateGraph(VisitStateNode[] startNodes, int fullMask)
    {
        StartNodes = startNodes;
        FullMask = fullMask;
    }

    public VisitStateNode[] StartNodes { get; }

    public int FullMask { get; }

    public static VisitStateGraph Build(int[][] graph)
    {
        var nodesByState = CreateStateNodes(graph);

        WireNeighbors(graph, nodesByState);

        return new VisitStateGraph(CreateStartNodes(graph, nodesByState), (1 << graph.Length) - 1);
    }

    private static Dictionary<(int Node, int Mask), VisitStateNode> CreateStateNodes(int[][] graph)
    {
        var stateCount = 1 << graph.Length;
        var nodesByState = new Dictionary<(int Node, int Mask), VisitStateNode>();

        for (var node = 0; node < graph.Length; node++)
        {
            for (var mask = 0; mask < stateCount; mask++)
            {
                nodesByState[(node, mask)] = new VisitStateNode(node, mask);
            }
        }

        return nodesByState;
    }

    private static void WireNeighbors(
        int[][] graph, Dictionary<(int Node, int Mask), VisitStateNode> nodesByState)
    {
        foreach (var state in nodesByState.Values)
        {
            foreach (var neighbor in graph[state.Node])
            {
                state.Neighbors.Add(nodesByState[(neighbor, state.Mask | (1 << neighbor))]);
            }
        }
    }

    private static VisitStateNode[] CreateStartNodes(
        int[][] graph, Dictionary<(int Node, int Mask), VisitStateNode> nodesByState)
    {
        var startNodes = new VisitStateNode[graph.Length];

        for (var start = 0; start < graph.Length; start++)
        {
            startNodes[start] = nodesByState[(start, 1 << start)];
        }

        return startNodes;
    }
}
