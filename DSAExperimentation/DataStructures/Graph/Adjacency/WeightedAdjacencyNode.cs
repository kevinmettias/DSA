namespace DSAExperimentation.DataStructures.Graph.Adjacency;

// AdjacencyNode with a weight on every out-edge: the shape ShortestPath, BellmanFord and the other
// edge-aware engines walk through IEdgeTopology. Generic over the weight because callers need int,
// long and double weights and every engine that reads them is already generic over TWeight; each
// value-type weight gets its own JIT-specialized node and topology.
//
// The tuple is (Weight, Target), the order ListEdges' (Data, Target) expects, so GetEdges wraps the
// list as-is.
internal sealed class WeightedAdjacencyNode<TWeight>(int id)
{
    public int Id { get; } = id;

    public List<(TWeight Weight, WeightedAdjacencyNode<TWeight> Target)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
