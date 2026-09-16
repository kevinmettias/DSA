using DSAExperimentation.LeetCode.CircularArrayLoop;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CircularArrayLoop;

// Harness only. Both strategies are CircularArrayLoopSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class CircularArrayLoopTests
{
    public static TheoryData<CircularArrayCase> Examples =>
        new()
        {
            { new CircularArrayCase([1, 1, 1, 1], Expected: true) },
            { new CircularArrayCase([2, -1, 1, 2, 2], Expected: true) },
            { new CircularArrayCase([-1, -2, -3, -4, -5, 6], Expected: false) },
            { new CircularArrayCase([1, -1, 1, -1], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasLoopByHashSetWalk_LeetCodeExamples_MatchesExpected(CircularArrayCase example) =>
        Assert.Equal(example.Expected, CircularArrayLoopSolution.HasLoopByHashSetWalk(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasLoopByLinkedListFloyd_LeetCodeExamples_MatchesExpected(CircularArrayCase example) =>
        Assert.Equal(example.Expected, CircularArrayLoopSolution.HasLoopByLinkedListFloyd(example.Nums));

    // One LeetCode example: the circular array and whether it contains a loop. The
    // expected value is named at every construction site, so a row reads as the case
    // it is rather than as a bare `true` whose meaning is its position. Nested because
    // it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct CircularArrayCase(int[] Nums, bool Expected);
}
