namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// LC 2699's graph: the n nodes plus, for every input edge, the two List slots
// (one per direction) that carry its weight, so SetWeight can move both halves
// together and keep the graph undirected. Every -1 edge starts at
// AssignableEdgeWeights.FloorWeight, the smallest positive weight the problem
// allows, which makes the first search over a freshly built graph the shortest
// distance any legal assignment can produce.
internal sealed class AssignableEdgeGraph(
    AssignableEdgeNode[] nodes, AssignableEdgeGraph.EdgeSlots[] slots)
{
    public int EdgeCount => slots.Length;

    public AssignableEdgeNode Node(int id) => nodes[id];

    public bool IsAssignable(int edgeIndex) => slots[edgeIndex].Assignable;

    public (int From, int To) Endpoints(int edgeIndex) => (slots[edgeIndex].From, slots[edgeIndex].To);

    public static AssignableEdgeGraph Build(int n, int[][] edges)
    {
        var nodes = CreateNodes(n);
        var slots = LinkEdges(nodes, edges);

        return new AssignableEdgeGraph(nodes, slots);
    }

    private static AssignableEdgeNode[] CreateNodes(int n)
    {
        var nodes = new AssignableEdgeNode[n];

        for (var id = 0; id < n; id++)
        {
            nodes[id] = new AssignableEdgeNode(id);
        }

        return nodes;
    }

    private static EdgeSlots[] LinkEdges(AssignableEdgeNode[] nodes, int[][] edges)
    {
        var slots = new EdgeSlots[edges.Length];

        for (var index = 0; index < edges.Length; index++)
        {
            slots[index] = Link(nodes, edges[index]);
        }

        return slots;
    }

    private static EdgeSlots Link(AssignableEdgeNode[] nodes, int[] edge)
    {
        var (from, to, declared) = (edge[0], edge[1], edge[2]);
        var weight = declared == AssignableEdgeWeights.Unassigned
            ? AssignableEdgeWeights.FloorWeight
            : declared;
        var fromSlot = AppendDirectedEdge(nodes[from], nodes[to], weight);
        var toSlot = AppendDirectedEdge(nodes[to], nodes[from], weight);

        return new EdgeSlots(from, fromSlot, to, toSlot, declared == AssignableEdgeWeights.Unassigned);
    }

    // One directed half of an edge lands at the end of its endpoint's list; the
    // slot it landed in is how SetWeight finds that half again.
    private static int AppendDirectedEdge(AssignableEdgeNode from, AssignableEdgeNode to, int weight)
    {
        var slot = from.Edges.Count;
        from.Edges.Add((weight, to));

        return slot;
    }

    // Both directions move together, so the graph never becomes asymmetric
    // between two searches.
    public void SetWeight(int edgeIndex, int weight)
    {
        var slot = slots[edgeIndex];
        nodes[slot.From].Edges[slot.FromSlot] = (weight, nodes[slot.To]);
        nodes[slot.To].Edges[slot.ToSlot] = (weight, nodes[slot.From]);
    }

    // The answer LC 2699 asks for: the original edge list with every weight -
    // assigned or not - read back out of the graph in its current state.
    public int[][] Weights()
    {
        var weights = new int[slots.Length][];

        for (var index = 0; index < slots.Length; index++)
        {
            var slot = slots[index];
            weights[index] = [slot.From, slot.To, nodes[slot.From].Edges[slot.FromSlot].Weight];
        }

        return weights;
    }

    // Internal rather than private: the graph's primary constructor names this
    // type, and a primary constructor is never less accessible than its type.
    internal readonly record struct EdgeSlots(
        int From, int FromSlot, int To, int ToSlot, bool Assignable);
}
