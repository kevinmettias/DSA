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
    public static TheoryData<PossibleBipartitionCase> Examples =>
        new()
        {
            // LC example 1: {1,4} and {2,3}.
            { new PossibleBipartitionCase(N: 4, Dislikes: [[1, 2], [1, 3], [2, 4]], Expected: true) },

            // LC example 2: 1-2-3-1 is a triangle, the smallest odd cycle.
            { new PossibleBipartitionCase(N: 3, Dislikes: [[1, 2], [1, 3], [2, 3]], Expected: false) },

            // LC example 3: 1-2-3-4-5-1 is a 5-cycle, also odd.
            { new PossibleBipartitionCase(N: 5, Dislikes: [[1, 2], [2, 3], [3, 4], [4, 5], [1, 5]], Expected: false) },

            // Nobody dislikes anybody: every person is an isolated vertex.
            { new PossibleBipartitionCase(N: 3, Dislikes: [], Expected: true) },

            // A single person, with nothing to split.
            { new PossibleBipartitionCase(N: 1, Dislikes: [], Expected: true) },

            // Two disjoint pairs: bipartite across more than one component.
            { new PossibleBipartitionCase(N: 4, Dislikes: [[1, 2], [3, 4]], Expected: true) },

            // A star: every dislike crosses from one person to a leaf.
            { new PossibleBipartitionCase(N: 4, Dislikes: [[1, 2], [1, 3], [1, 4]], Expected: true) },

            // A clean 4-cycle of dislikes, which is even and so splits.
            { new PossibleBipartitionCase(N: 4, Dislikes: [[1, 2], [2, 3], [3, 4], [1, 4]], Expected: true) },

            // One splittable component and one triangle, plus an isolated person.
            { new PossibleBipartitionCase(N: 6, Dislikes: [[1, 2], [3, 4], [4, 5], [3, 5]], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBipartitionByColorArrayDfs_LeetCodeExamples_ReturnsWhetherDislikesSplitInTwo(
        PossibleBipartitionCase example)
    {
        var actual = PossibleBipartitionSolution.CanBipartitionByColorArrayDfs(example.N, example.Dislikes);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanBipartitionByBipartiteCheck_LeetCodeExamples_ReturnsWhetherDislikesSplitInTwo(
        PossibleBipartitionCase example)
    {
        var actual = PossibleBipartitionSolution.CanBipartitionByBipartiteCheck(example.N, example.Dislikes);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the number of people, the dislike pairs between them, and
    // whether those dislikes split in two. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare
    // `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PossibleBipartitionCase(int N, int[][] Dislikes, bool Expected);
}
