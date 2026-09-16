using DSAExperimentation.LeetCode.StoneGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGame;

// Harness only. Both strategies are StoneGameSolution's - this file just pins them
// to LeetCode's published examples plus a few more even-length pile rows.
public sealed class StoneGameTests
{
    public static TheoryData<PileGameExample> Examples =>
        new()
        {
            new PileGameExample([5, 3, 4, 5], AliceWins: true),
            new PileGameExample([3, 7, 2, 3], AliceWins: true),
            new PileGameExample([3, 2], AliceWins: true),
            new PileGameExample([2, 3], AliceWins: true),
            new PileGameExample([1, 100, 3, 4, 5, 6], AliceWins: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByUnmemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(PileGameExample example) =>
        Assert.Equal(example.AliceWins, StoneGameSolution.CanAliceWinByUnmemoizedRecursion(example.Piles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(PileGameExample example) =>
        Assert.Equal(example.AliceWins, StoneGameSolution.CanAliceWinByMemoizedRecursion(example.Piles));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PileGameExample(int[] Piles, bool AliceWins);
}
