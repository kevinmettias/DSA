
// One node per LeetCode vertex id (0..nodeCount-1), its weighted edges wired in after
// construction.
using ComponentNode = DSAExperimentation.DataStructures.Graph.Adjacency.WeightedAdjacencyNode<int>;

namespace DSAExperimentation.LeetCode.MinimizeMaximumComponentCost;

// Builds LeetCode's own (nodeCount, edges) shape into ComponentNodes wired both
// ways - the same "materialize once, hand the algorithm a vertex list" role
// Domain.Locks.LockGraph.Build plays for OpenTheLock, just for an arbitrary
// weighted graph instead of a wheel-turn Cayley graph. Kept problem-local
// (LeetCode/MinimizeMaximumComponentCost, not Domain) per this pass's
// instruction not to grow new shared production primitives - see
// MinimizeMaximumComponentCostSolution's own doc comment.
internal sealed class ComponentGraph
{
    public IReadOnlyList<ComponentNode> Vertices { get; }

    private ComponentGraph(IReadOnlyList<ComponentNode> vertices) => Vertices = vertices;

    public static ComponentGraph Build(int nodeCount, int[][] edges)
    {
        var vertices = LeetCodeAdjacency.ZeroBased<ComponentNode>(
            nodeCount, edges, id => new ComponentNode(id), (vertex, _, farVertex, edgeIndex) => vertex.Edges.Add((edges[edgeIndex][2], farVertex)));

        return new ComponentGraph(vertices);
    }
}
