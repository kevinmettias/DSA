using DSAExperimentation.LeetCode.PossibleBipartition;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PossibleBipartition;

// Harness only. Both strategies are PossibleBipartitionSolution's - this file
// pins them to LeetCode's published examples plus the shapes the composed arm has
// to answer without a single dislike to walk: nobody disliking anybody, and a
// split that only works because the components are considered separately. The
// color-array arm was previously only a benchmark baseline and nothing asserted
// it; it is under test here for the first time.
public sealed class PossibleBipartitionTests
{
    public static TheoryData<int, int[][], bool> Examples =>
        new()
        {
            // LC example 1: {1,4} and {2,3}.
            { 4, [[1, 2], [1, 3], [2, 4]], true },

            // LC example 2: 1-2-3-1 is a triangle, the smallest odd cycle.
            { 3, [[1, 2], [1, 3], [2, 3]], false },

            // LC example 3: 1-2-3-4-5-1 is a 5-cycle, also odd.
            { 5, [[1, 2], [2, 3], [3, 4], [4, 5], [1, 5]], false },

            // Nobody dislikes anybody: every person is an isolated vertex.
            { 3, [], true },

            // A single person, with nothing to split.
            { 1, [], true },

            // Two disjoint pairs: bipartite across more than one component.
            { 4, [[1, 2], [3, 4]], true },

            // A star: every dislike crosses from one person to a leaf.
            { 4, [[1, 2], [1, 3], [1, 4]], true },

            // A clean 4-cycle of dislikes, which is even and so splits.
            { 4, [[1, 2], [2, 3], [3, 4], [1, 4]], true },

            // One splittable component and one triangle, plus an isolated person.
            { 6, [[1, 2], [3, 4], [4, 5], [3, 5]], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PossibleBipartitionByColorArrayDfs_LeetCodeExamples_ReturnsWhetherDislikesSplitInTwo(
        int n, int[][] dislikes, bool expected) =>
        Assert.Equal(expected, PossibleBipartitionSolution.PossibleBipartitionByColorArrayDfs(n, dislikes));

    [Theory]
    [MemberData(nameof(Examples))]
    public void PossibleBipartitionByBipartiteCheck_LeetCodeExamples_ReturnsWhetherDislikesSplitInTwo(
        int n, int[][] dislikes, bool expected) =>
        Assert.Equal(expected, PossibleBipartitionSolution.PossibleBipartitionByBipartiteCheck(n, dislikes));
}
