using DSAExperimentation.LeetCode.Solutions.CircularArrayLoop;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CircularArrayLoop;

// Harness only. Both strategies are CircularArrayLoopSolution's - this file pins
// them to two cases: a loop that runs through the whole array in one direction, and
// a cycle that alternates direction at every step, which does not count as a loop.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CircularArrayLoopSolutionTests
{
    public static TheoryData<CircularArrayCase> Cases =>
        new()
        {
            { new CircularArrayCase([1, 1, 1, 1], Expected: true) },
            { new CircularArrayCase([1, -1, 1, -1], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasLoopByHashSetWalk_Cases_MatchesExpected(CircularArrayCase testCase) =>
        Assert.Equal(testCase.Expected, CircularArrayLoopSolution.HasLoopByHashSetWalk(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void HasLoopByLinkedListFloyd_Cases_MatchesExpected(CircularArrayCase testCase) =>
        Assert.Equal(testCase.Expected, CircularArrayLoopSolution.HasLoopByLinkedListFloyd(testCase.Nums));

    // One case: the circular array and whether it contains a loop. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct CircularArrayCase(int[] Nums, bool Expected);
}
