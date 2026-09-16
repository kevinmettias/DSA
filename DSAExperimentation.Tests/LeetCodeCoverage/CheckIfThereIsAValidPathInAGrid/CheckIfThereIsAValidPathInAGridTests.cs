using DSAExperimentation.LeetCode.CheckIfThereIsAValidPathInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidPathInAGrid;

// Harness only. Both strategies are CheckIfThereIsAValidPathInAGridSolution's -
// this file just pins them to LeetCode's published examples plus the corner cases
// the original coverage carried, one theory per strategy so a failure names the
// strategy that broke.
public sealed partial class CheckIfThereIsAValidPathInAGridTests
{
    public static TheoryData<StreetGridCase> Examples =>
        new()
        {
            // LeetCode example 1: down, right, up, right, down through the corner.
            { new StreetGridCase([[2, 4, 3], [6, 5, 2]], Expected: true) },

            // LeetCode example 2: two vertical streets that never connect sideways.
            { new StreetGridCase([[1, 2, 1], [1, 2, 1]], Expected: false) },

            // LeetCode example 3: the last street opens up and down, not left.
            { new StreetGridCase([[1, 1, 2]], Expected: false) },

            // (0,0)-R->(0,1)-D->(1,1)-D->(2,1)-R->(2,2): two turns via a right-down
            // street (3), a straight vertical street (2), then a right-up street (6).
            { new StreetGridCase([[1, 3, 1], [2, 2, 2], [1, 6, 1]], Expected: true) },

            // A single row of horizontal streets is one straight corridor.
            { new StreetGridCase([[1, 1, 1]], Expected: true) },

            // A one-cell grid: the start already is the corner.
            { new StreetGridCase([[1]], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByRecursiveDfs_LeetCodeExamples_ReportsWhetherTheCornerIsReachable(
        StreetGridCase example) =>
        Assert.Equal(
            example.Expected,
            CheckIfThereIsAValidPathInAGridSolution.HasValidPathByRecursiveDfs(example.Grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByDepthFirstTraverse_LeetCodeExamples_ReportsWhetherTheCornerIsReachable(
        StreetGridCase example) =>
        Assert.Equal(
            example.Expected,
            CheckIfThereIsAValidPathInAGridSolution.HasValidPathByDepthFirstTraverse(example.Grid));

    // One LeetCode example: the street grid and whether its corner is reachable. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct StreetGridCase(int[][] Grid, bool Expected);
}
