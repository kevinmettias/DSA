namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseScheduleII.Fixtures;

// Edges point prerequisite -> dependent course, matching Kahn's algorithm's own
// in-degree bookkeeping: a course's in-degree is its remaining unresolved
// prerequisite count. Mirrors CourseSchedule/Fixtures/CourseNode.cs (LC 207) - LC
// 210 asks for the actual ordering that same TopologicalSort.TrySort call already
// produces, not just whether one exists.
internal sealed class CourseNode(int id)
{
    public int Id { get; } = id;

    public List<CourseNode> EnabledCourses { get; } = [];

    public override string ToString() => Id.ToString();
}
