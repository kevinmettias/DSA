namespace DSAExperimentation.LeetCode.ModifyGraphEdgeWeights;

// LC 2699's graph: the n nodes plus, for every input edge, the two List slots
// (one per direction) that carry its weight, so SetWeight can move both halves
// together and keep the graph undirected. Every -1 edge starts at FloorWeight,
// the smallest positive weight the problem allows, which makes the first search
// over a freshly built graph the shortest distance any legal assignment can
// produce.
internal sealed class AssignableEdgeGraph
{
    // LC 2699 writes -1 for "you choose this edge's weight".
    public const int Unassigned = -1;

    // Weights must be positive integers, so 1 is the floor every -1 edge starts at.
    public const int FloorWeight = 1;

    private readonly AssignableEdgeNode[] _nodes;
    private readonly EdgeSlots[] _slots;

    private AssignableEdgeGraph(AssignableEdgeNode[] nodes, EdgeSlots[] slots)
    {
        _nodes = nodes;
        _slots = slots;
    }

    public int EdgeCount => _slots.Length;

    public AssignableEdgeNode Node(int id) => _nodes[id];

    public bool IsAssignable(int edgeIndex) => _slots[edgeIndex].Assignable;

    public (int From, int To) Endpoints(int edgeIndex) => (_slots[edgeIndex].From, _slots[edgeIndex].To);

    public static AssignableEdgeGraph Build(int n, int[][] edges)
    {
        var nodes = new AssignableEdgeNode[n];

        for (var id = 0; id < n; id++)
        {
            nodes[id] = new AssignableEdgeNode(id);
        }

        var slots = new EdgeSlots[edges.Length];

        for (var index = 0; index < edges.Length; index++)
        {
            slots[index] = Link(nodes, edges[index]);
        }

        return new AssignableEdgeGraph(nodes, slots);
    }

    private static EdgeSlots Link(AssignableEdgeNode[] nodes, int[] edge)
    {
        var (from, to, declared) = (edge[0], edge[1], edge[2]);
        var weight = declared == Unassigned ? FloorWeight : declared;
        var fromSlot = nodes[from].Edges.Count;
        nodes[from].Edges.Add((weight, nodes[to]));
        var toSlot = nodes[to].Edges.Count;
        nodes[to].Edges.Add((weight, nodes[from]));

        return new EdgeSlots(from, fromSlot, to, toSlot, declared == Unassigned);
    }

    // Both directions move together, so the graph never becomes asymmetric
    // between two searches.
    public void SetWeight(int edgeIndex, int weight)
    {
        var slots = _slots[edgeIndex];
        _nodes[slots.From].Edges[slots.FromSlot] = (weight, _nodes[slots.To]);
        _nodes[slots.To].Edges[slots.ToSlot] = (weight, _nodes[slots.From]);
    }

    // The answer LC 2699 asks for: the original edge list with every weight -
    // assigned or not - read back out of the graph in its current state.
    public int[][] Weights()
    {
        var weights = new int[_slots.Length][];

        for (var index = 0; index < _slots.Length; index++)
        {
            var slots = _slots[index];
            weights[index] = [slots.From, slots.To, _nodes[slots.From].Edges[slots.FromSlot].Weight];
        }

        return weights;
    }

    private readonly record struct EdgeSlots(int From, int FromSlot, int To, int ToSlot, bool Assignable);
}
