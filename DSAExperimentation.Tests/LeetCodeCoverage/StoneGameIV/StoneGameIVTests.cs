using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIV;

// Harness only: both strategies live in StoneGameIVSolution and are asserted against
// the same examples. The un-memoized recursion is the definition and the memoized one
// is the same recurrence with a cache, so pinning both to one example set is exactly
// the check that memoization changed nothing - the benchmark previously compared them
// with only the memoized arm under test.
public sealed class StoneGameIVTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, true },
            { 2, false },
            { 3, true },
            { 4, true },
            { 5, false },
            { 6, true },
            { 7, false },
            { 8, true },
            { 9, true },
            { 10, false },
            { 17, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByUnmemoizedRecursion_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        int n, bool expected) =>
        Assert.Equal(expected, StoneGameIVSolution.AliceWinsByUnmemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByMemoizedRecursion_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        int n, bool expected) =>
        Assert.Equal(expected, StoneGameIVSolution.AliceWinsByMemoizedRecursion(n));
}
