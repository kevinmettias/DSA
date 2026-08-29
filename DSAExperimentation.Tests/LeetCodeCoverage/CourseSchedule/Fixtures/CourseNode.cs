namespace DSAExperimentation.Tests.LeetCodeCoverage.CourseSchedule.Fixtures;

// Edges point prerequisite -> dependent course, matching Kahn's algorithm's own
// in-degree bookkeeping: a course's in-degree is its remaining unresolved
// prerequisite count.
internal sealed class CourseNode(int id)
{
    public int Id { get; } = id;

    public List<CourseNode> EnabledCourses { get; } = [];

    public override string ToString() => Id.ToString();
}
