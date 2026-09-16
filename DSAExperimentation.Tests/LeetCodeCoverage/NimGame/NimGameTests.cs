using DSAExperimentation.LeetCode.NimGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NimGame;

// Harness only: both strategies live in NimGameSolution and are asserted
// against the same examples.
public sealed class NimGameTests
{
    public static TheoryData<NimExample> Examples =>
        new()
        {
            { new NimExample(N: 1, Expected: true) },
            { new NimExample(N: 2, Expected: true) },
            { new NimExample(N: 3, Expected: true) },
            { new NimExample(N: 4, Expected: false) },
            { new NimExample(N: 7, Expected: true) },
            { new NimExample(N: 8, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(NimExample example) =>
        Assert.Equal(example.Expected, NimGameSolution.CanWinByMemoizedRecursion(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByModuloFormula_LeetCodeExamples_MatchesExpectedOutcome(NimExample example) =>
        Assert.Equal(example.Expected, NimGameSolution.CanWinByModuloFormula(example.N));

    // One LeetCode example: the heap size and whether the player to move wins from it.
    // The outcome is the datum under test, so the row names it rather than leaving a
    // bare `bool` next to the heap size where the read is `CanWin(n, true)` - true
    // meaning what?
    public readonly record struct NimExample(int N, bool Expected);
}
