namespace DSAExperimentation.LeetCode.Solutions.ParallelCoursesIII;

// Time is the course's own duration in months, read once per node during the
// forward relaxation DP. Edges point prerequisite -> dependent (CourseNode/
// ColorGraphNode's own precedent), matching Kahn's algorithm's own in-degree
// bookkeeping.
//
// Answers LC 2050 alone - DataStructures' AdjacencyNode plus the course's duration,
// which only this problem reads - so it lives beside the solution rather than in
// DataStructures/ (ARCHITECTURE.md #17.3/#17.6). It previously existed twice: once
// as a Tests fixture and once again as a private copy inside
// ParallelCoursesIIIBenchmarks; this is the single surviving declaration.
internal sealed class CourseTimeNode(int id, int time)
{
    public int Id { get; } = id;

    public int Time { get; } = time;

    public List<CourseTimeNode> Successors { get; } = [];

    public override string ToString() => Id.ToString();
}
