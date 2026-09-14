namespace DSAExperimentation.LeetCode.CountVisitedNodesInADirectedGraph;

// One node of LC 2876's graph, holding the single successor edges[i] names.
// Successors is a list rather than a dedicated exactly-one-child contract because
// ListChildren already reads a List<T>, and the out-degree bound is a caller
// discipline FunctionalGraph.Build enforces rather than something the type states.
//
// Unlike LC 2360's node of the same name, there is no "no outgoing edge" case here:
// LC 2876 guarantees every node has exactly one successor, and forbids a self-loop.
// It answers this problem alone - a plain adjacency-list graph node fixing no
// vertex set and no modulus - so it lives beside the solution rather than in
// Domain/ or DataStructures/ (ARCHITECTURE.md #17.3/#17.6), the same per-problem
// placement CourseTopology has in three folders and PersonTopology in two. It
// previously existed as a Tests fixture and again in Benchmarks/Fixtures; this is
// the declaration this problem now uses for both.
internal sealed class FunctionalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<FunctionalGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
