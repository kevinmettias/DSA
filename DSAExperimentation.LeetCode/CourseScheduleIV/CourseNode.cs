namespace DSAExperimentation.LeetCode.CourseScheduleIV;

// One node per course, with Edges pointing prerequisite -> dependent. The weight
// is carried only because the all-pairs primitive this problem composes is a
// shortest-path engine (IEdgeTopology/IEdges are weighted contracts); every
// prerequisite edge costs the same, so the distances it produces are only ever
// read as "finite or absent", never compared.
//
// Answers LC 1462 alone - a plain adjacency-list graph node with no fixed vertex
// set or modulus, so it lives beside the solution rather than in Domain/
// (ARCHITECTURE.md #17.6), the same call CourseSchedule's and CourseScheduleII's
// own CourseNode already made and the same one CityNode makes for the weighted
// shape LC 1334 needs.
internal sealed class CourseNode(int id)
{
    public int Id { get; } = id;

    public List<(int Weight, CourseNode Dependent)> Edges { get; } = [];

    public override string ToString() => Id.ToString();
}
