using DSAExperimentation.LeetCode.Solutions.PossibleBipartition;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PossibleBipartition;

// Harness only. Both strategies are PossibleBipartitionSolution's - this file
// pins them to the shapes the composed arm has
// to answer without a single dislike to walk: nobody disliking anybody, and a
// split that only works because the components are considered separately. The
// color-array arm was previously only a benchmark baseline and nothing asserted
// it; it is under test here for the first time.
public sealed partial class PossibleBipartitionSolutionTests
{
    public static TheoryData<PossibleBipartitionCase> Cases =>
        new()
        {
            // Nobody dislikes anybody: every person is an isolated vertex.
            { new PossibleBipartitionCase(N: 3, Dislikes: [], Expected: true) },

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
    [MemberData(nameof(Cases))]
    public void CanBipartitionByColorArrayDfs_Cases_ReturnsWhetherDislikesSplitInTwo(
        PossibleBipartitionCase testCase)
    {
        var actual = PossibleBipartitionSolution.CanBipartitionByColorArrayDfs(testCase.N, testCase.Dislikes);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanBipartitionByBipartiteCheck_Cases_ReturnsWhetherDislikesSplitInTwo(
        PossibleBipartitionCase testCase)
    {
        var actual = PossibleBipartitionSolution.CanBipartitionByBipartiteCheck(testCase.N, testCase.Dislikes);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the number of people, the dislike pairs between them, and
    // whether those dislikes split in two. The expected value is named at every
    // construction site, so a row reads as the case it is rather than as a bare
    // `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct PossibleBipartitionCase(int N, int[][] Dislikes, bool Expected);
}
