using DSAExperimentation.LeetCode.DivisorGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivisorGame;

// Harness only: both strategies live in DivisorGameSolution and are asserted against
// the same examples. The memoized recursion is the definition and the parity formula
// is the closed form it reduces to, so pinning both to one example set is exactly the
// check that the reduction holds - the benchmark previously compared them with only
// the recursion under test.
public sealed class DivisorGameTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, false },
            { 2, true },
            { 3, false },
            { 4, true },
            { 5, false },
            { 6, true },
            { 7, false },
            { 12, true },
            { 17, false },
            { 20, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected) =>
        Assert.Equal(expected, DivisorGameSolution.AliceWinsByMemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByParityFormula_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected) =>
        Assert.Equal(expected, DivisorGameSolution.AliceWinsByParityFormula(n));
}
