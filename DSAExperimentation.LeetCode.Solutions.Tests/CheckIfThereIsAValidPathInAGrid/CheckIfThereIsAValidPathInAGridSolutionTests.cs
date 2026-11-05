using DSAExperimentation.LeetCode.Solutions.CheckIfThereIsAValidPathInAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfThereIsAValidPathInAGrid;

// Harness only. Both strategies are CheckIfThereIsAValidPathInAGridSolution's -
// this file just pins them to the corner cases
// the original coverage carried, one theory per strategy so a failure names the
// strategy that broke.
public sealed partial class CheckIfThereIsAValidPathInAGridSolutionTests
{
    public static TheoryData<StreetGridCase> Cases =>
        new()
        {
            // (0,0)-R->(0,1)-D->(1,1)-D->(2,1)-R->(2,2): two turns via a right-down
            // street (3), a straight vertical street (2), then a right-up street (6).
            { new StreetGridCase([[1, 3, 1], [2, 2, 2], [1, 6, 1]], Expected: true) },

            // A single row of horizontal streets is one straight corridor.
            { new StreetGridCase([[1, 1, 1]], Expected: true) },

            // A one-cell grid: the start already is the corner.
            { new StreetGridCase([[1]], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasValidPathByRecursiveDfs_Cases_ReportsWhetherTheCornerIsReachable(
        StreetGridCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            CheckIfThereIsAValidPathInAGridSolution.HasValidPathByRecursiveDfs(testCase.Grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasValidPathByDepthFirstTraverse_Cases_ReportsWhetherTheCornerIsReachable(
        StreetGridCase testCase) =>
        Assert.Equal(
            testCase.Expected,
            CheckIfThereIsAValidPathInAGridSolution.HasValidPathByDepthFirstTraverse(testCase.Grid));

    // One case: the street grid and whether its corner is reachable. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct StreetGridCase(int[][] Grid, bool Expected);
}
