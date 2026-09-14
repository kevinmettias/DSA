namespace DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

// Edges point "must come first" -> "must come after", the same in-degree-is-
// unresolved-prerequisite-count shape LeetCode/CourseScheduleII's own CourseNode
// establishes: a value's in-degree is how many values still have to be placed
// ahead of it. One fresh node set is built per axis (row order, column order),
// since rowConditions and colConditions are otherwise-unrelated edge sets over
// the same 1..k value range.
//
// A plain adjacency-list graph node with no fixed vertex set, modulus or other
// pinned content, answering LC 2392 alone - so it lives beside the solution
// rather than in Domain/ (ARCHITECTURE.md #17.6).
internal sealed class ValueNode(int id)
{
    public int Id { get; } = id;

    public List<ValueNode> After { get; } = [];

    public override string ToString() => Id.ToString();
}
