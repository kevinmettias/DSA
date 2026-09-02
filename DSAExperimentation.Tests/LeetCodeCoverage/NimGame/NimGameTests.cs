using DSAExperimentation.LeetCode.NimGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NimGame;

// Harness only: both strategies live in NimGameSolution and are asserted
// against the same examples.
public sealed class NimGameTests
{
    public static TheoryData<int, bool> Examples =>
        new()
        {
            { 1, true },
            { 2, true },
            { 3, true },
            { 4, false },
            { 7, true },
            { 8, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected) =>
        Assert.Equal(expected, NimGameSolution.CanWinByMemoizedRecursion(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByModuloFormula_LeetCodeExamples_MatchesExpectedOutcome(int n, bool expected) =>
        Assert.Equal(expected, NimGameSolution.CanWinByModuloFormula(n));
}
