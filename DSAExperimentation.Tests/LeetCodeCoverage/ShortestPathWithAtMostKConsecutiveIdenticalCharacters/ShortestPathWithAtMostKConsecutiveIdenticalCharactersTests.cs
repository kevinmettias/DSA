using DSAExperimentation.LeetCode.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ShortestPathWithAtMostKConsecutiveIdenticalCharacters;

// Harness only. ConsecutiveRunGraph is
// ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution's own domain
// model and both search strategies are its methods - this file just pins them
// to LeetCode's published examples, including the unreachable case where every
// path would need three consecutive 'a's but k only allows two.
public sealed class ShortestPathWithAtMostKConsecutiveIdenticalCharactersTests
{
    public static TheoryData<int, int[][], string, int, int> Examples =>
        new()
        {
            { 3, [[0, 1, 1], [1, 2, 1], [0, 2, 3]], "aab", 1, 3 },
            { 3, [[0, 1, 1], [1, 2, 1], [0, 2, 3]], "aab", 2, 2 },
            { 3, [[0, 1, 1], [1, 2, 1]], "aaa", 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumPathWeightByBclPriorityQueue_LeetCodeExamples_ReturnsMinimumWeight(
        int n, int[][] edges, string labels, int k, int expected) =>
        Assert.Equal(
            expected,
            ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByBclPriorityQueue(
                n, edges, labels, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumPathWeightByReduceGraph_LeetCodeExamples_ReturnsMinimumWeight(
        int n, int[][] edges, string labels, int k, int expected) =>
        Assert.Equal(
            expected,
            ShortestPathWithAtMostKConsecutiveIdenticalCharactersSolution.MinimumPathWeightByReduceGraph(
                n, edges, labels, k));
}
