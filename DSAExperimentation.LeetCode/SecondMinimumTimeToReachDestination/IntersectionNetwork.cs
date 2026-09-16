namespace DSAExperimentation.LeetCode.SecondMinimumTimeToReachDestination;

// LC 2045's city as a plain per-intersection adjacency list. LeetCode numbers the n
// intersections from 1 and states every road as a two-element [a, b] row; both
// strategies want them zero-based and recorded from both ends, and neither wants to
// say so itself.
//
// It exists so the naive baseline has a prepared-input overload of its own
// (ARCHITECTURE.md section 17.4): the benchmark builds this once in [GlobalSetup]
// instead of charging the adjacency build to every measured walk. It is deliberately
// not an IEnumerable, so it can never be bound by the LeetCode-shaped overloads.
internal sealed class IntersectionNetwork(List<int>[] roads)
{
    // LeetCode states each road as one [a, b] row of 1-based intersection numbers.
    private const int FromColumn = 0;
    private const int ToColumn = 1;
    private const int FirstIntersection = 1;

    public int IntersectionCount => roads.Length;

    // LeetCode always asks for the trip from intersection 1 to intersection n.
    public int Start => 0;

    public int Destination => IntersectionCount - 1;

    public List<int> RoadsFrom(int intersection) => roads[intersection];

    public static IntersectionNetwork Build(int intersectionCount, int[][] edges)
    {
        var roads = new List<int>[intersectionCount];

        for (var intersection = 0; intersection < intersectionCount; intersection++)
        {
            roads[intersection] = [];
        }

        foreach (var edge in edges)
        {
            var (a, b) = (edge[FromColumn] - FirstIntersection, edge[ToColumn] - FirstIntersection);

            roads[a].Add(b);
            roads[b].Add(a);
        }

        return new IntersectionNetwork(roads);
    }
}
