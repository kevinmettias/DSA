namespace DSAExperimentation.Tests.LeetCodeCoverage.BuildAMatrixWithConditions.Fixtures;

// Edges point "must come before" -> "must come after", the same in-degree-is-
// unresolved-prerequisite-count shape CourseSchedule/Fixtures/CourseNode.cs
// already establishes. One fresh node set is built per axis (row order, column
// order) since the two condition lists are otherwise-unrelated edge sets over the
// same 1..k value range.
internal sealed class ValueNode(int id)
{
    public int Id { get; } = id;

    public List<ValueNode> After { get; } = [];

    public override string ToString() => Id.ToString();
}
