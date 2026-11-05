using DSAExperimentation.LeetCode.Solutions.ChalkboardXorGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ChalkboardXorGame;

// Harness only. All three strategies are ChalkboardXorGameSolution's - the
// single-element and already-zero-xor boundaries, plus short boards of even and odd
// length, are stated once and replayed against each, so a failure names the strategy
// that broke rather than reporting a disagreement between an anonymous test helper
// and an anonymous benchmark arm. The closed form in particular was never asserted
// before: it existed only as a benchmark arm. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class ChalkboardXorGameSolutionTests
{
    public static TheoryData<GameCase> Cases =>
        new()
        {
            { new GameCase(Nums: [0], AliceWins: true) },
            { new GameCase(Nums: [2], AliceWins: false) },
            { new GameCase(Nums: [1, 2], AliceWins: true) },
            { new GameCase(Nums: [3, 3, 3], AliceWins: false) },
            { new GameCase(Nums: [1, 2, 3, 4], AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByBruteForceRecursion_Cases_ReturnsWhetherAliceWins(GameCase testCase) =>
        Assert.Equal(testCase.AliceWins, ChalkboardXorGameSolution.CanAliceWinByBruteForceRecursion(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByMemoizedRecursion_Cases_ReturnsWhetherAliceWins(GameCase testCase) =>
        Assert.Equal(testCase.AliceWins, ChalkboardXorGameSolution.CanAliceWinByMemoizedRecursion(testCase.Nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByXorParityFormula_Cases_ReturnsWhetherAliceWins(GameCase testCase) =>
        Assert.Equal(testCase.AliceWins, ChalkboardXorGameSolution.CanAliceWinByXorParityFormula(testCase.Nums));

    // One case: the chalkboard and whether Alice wins from it. The win
    // flag is named at the row that states it, so a reader of `Cases` never has to
    // remember which position `false` sits in.
    public readonly record struct GameCase(int[] Nums, bool AliceWins);
}
