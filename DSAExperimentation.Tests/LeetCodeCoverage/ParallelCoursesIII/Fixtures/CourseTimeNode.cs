namespace DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesIII.Fixtures;

// Edges point prerequisite -> dependent (CourseNode/ColorGraphNode's own
// precedent), matching Kahn's algorithm's own in-degree bookkeeping. Time is the
// course's own duration in months, read once per node during the forward
// relaxation DP.
internal sealed class CourseTimeNode(int id, int time)
{
    public int Id { get; } = id;

    public int Time { get; } = time;

    public List<CourseTimeNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
