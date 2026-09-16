using DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// Harness only. ConsecutiveRunGraph is
// ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution's own domain
// model and both search strategies are its methods - this file just pins them
// to LeetCode's published examples, including the unreachable case where every
// path would need three consecutive 'a's but k only allows two.
public sealed class ShortestPathWithAtMostKConsecutiveIdenticalCharactersTests
{
    public static TheoryData<ConsecutiveRunExample> Examples =>
        new()
        {
            new ConsecutiveRunExample(
                NodeCount: 3, Edges: [[0, 1, 1], [1, 2, 1], [0, 2, 3]], Labels: "aab", MaxConsecutive: 1, Expected: 3),
            new ConsecutiveRunExample(
                NodeCount: 3, Edges: [[0, 1, 1], [1, 2, 1], [0, 2, 3]], Labels: "aab", MaxConsecutive: 2, Expected: 2),
            new ConsecutiveRunExample(
                NodeCount: 3, Edges: [[0, 1, 1], [1, 2, 1]], Labels: "aaa", MaxConsecutive: 2, Expected: -1),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumPathWeightByBclPriorityQueue_LeetCodeExamples_ReturnsMinimumWeight(
        ConsecutiveRunExample example)
    {
        var actual = ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByBclPriorityQueue(
            example.NodeCount, example.Edges, example.Labels, example.MaxConsecutive);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumPathWeightByReduceGraph_LeetCodeExamples_ReturnsMinimumWeight(
        ConsecutiveRunExample example)
    {
        var actual = ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByReduceGraph(
            example.NodeCount, example.Edges, example.Labels, example.MaxConsecutive);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the node count, the weighted edges, the per-node labels,
    // the longest run of identical labels a path may contain, and the minimum weight
    // (-1 when no path qualifies). The five are one case, so the signature carries
    // one parameter rather than five positions.
    public readonly record struct ConsecutiveRunExample(
        int NodeCount,
        int[][] Edges,
        string Labels,
        int MaxConsecutive,
        int Expected);
}
