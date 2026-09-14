namespace DSAExperimentation.LeetCode.LongestCycleInAGraph;

// One node of LC 2360's graph, holding the single successor edges[i] names - or no
// successor at all when edges[i] is -1, which is the one way this differs from LC
// 2127's out-degree-exactly-one EmployeeNode. Successors is a list rather than a
// dedicated at-most-one-child contract because ListChildren already reads a
// List<T>, and the out-degree bound is a caller discipline FunctionalGraph.Build
// enforces rather than something the type itself states.
//
// Answers this problem alone - a plain adjacency-list graph node fixing no vertex
// set and no modulus - so it lives beside the solution rather than in Domain/ or
// DataStructures/ (ARCHITECTURE.md #17.3/#17.6), the same placement EmployeeNode
// has. It previously existed as a Tests fixture and again in Benchmarks/Fixtures;
// this is the declaration this problem now uses for both.
internal sealed class FunctionalGraphNode(int id)
{
    public int Id { get; } = id;

    public List<FunctionalGraphNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
