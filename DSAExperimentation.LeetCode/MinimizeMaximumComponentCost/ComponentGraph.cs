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
        var vertices = new List<ComponentNode>(nodeCount);

        for (var id = 0; id < nodeCount; id++)
        {
            vertices.Add(new ComponentNode(id));
        }

        foreach (var edge in edges)
        {
            var (u, v, weight) = (edge[0], edge[1], edge[2]);
            vertices[u].Edges.Add((weight, vertices[v]));
            vertices[v].Edges.Add((weight, vertices[u]));
        }

        return new ComponentGraph(vertices);
    }
}
