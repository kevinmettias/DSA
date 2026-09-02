namespace DSAExperimentation.LeetCode.CourseSchedule;

// Edges point prerequisite -> dependent course, matching Kahn's algorithm's own
// in-degree bookkeeping: a course's in-degree is its remaining unresolved
// prerequisite count. Answers LC 207 alone - a plain adjacency-list graph node
// with no fixed vertex set or modulus, so it lives beside the solution rather
// than in Domain/ (ARCHITECTURE.md #17.6).
internal sealed class CourseNode(int id)
{
    public int Id { get; } = id;

    public List<CourseNode> EnabledCourses { get; } = [];

    public override string ToString() => Id.ToString();
}
