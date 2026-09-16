using DSAExperimentation.LeetCode.DivisorGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DivisorGame;

// Harness only: both strategies live in DivisorGameSolution and are asserted against
// the same examples. The memoized recursion is the definition and the parity formula
// is the closed form it reduces to, so pinning both to one example set is exactly the
// check that the reduction holds - the benchmark previously compared them with only
// the recursion under test.
public sealed partial class DivisorGameTests
{
    public static TheoryData<DivisorGameCase> Examples =>
        new()
        {
            { new DivisorGameCase(1, AliceWins: false) },
            { new DivisorGameCase(2, AliceWins: true) },
            { new DivisorGameCase(3, AliceWins: false) },
            { new DivisorGameCase(4, AliceWins: true) },
            { new DivisorGameCase(5, AliceWins: false) },
            { new DivisorGameCase(6, AliceWins: true) },
            { new DivisorGameCase(7, AliceWins: false) },
            { new DivisorGameCase(12, AliceWins: true) },
            { new DivisorGameCase(17, AliceWins: false) },
            { new DivisorGameCase(20, AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByMemoizedRecursion_LeetCodeExamples_MatchesExpectedOutcome(
        DivisorGameCase example) =>
        Assert.Equal(example.AliceWins, DivisorGameSolution.CanAliceWinByMemoizedRecursion(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByParityFormula_LeetCodeExamples_MatchesExpectedOutcome(
        DivisorGameCase example) =>
        Assert.Equal(example.AliceWins, DivisorGameSolution.CanAliceWinByParityFormula(example.N));

    // One LeetCode example: the starting n and whether Alice wins it. The expected value
    // is named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct DivisorGameCase(int N, bool AliceWins);
}
