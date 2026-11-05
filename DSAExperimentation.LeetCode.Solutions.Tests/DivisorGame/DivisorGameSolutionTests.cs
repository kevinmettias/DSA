using DSAExperimentation.LeetCode.Solutions.DivisorGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DivisorGame;

// Harness only: both strategies live in DivisorGameSolution and are asserted against
// the same cases. The memoized recursion is the definition and the parity formula
// is the closed form it reduces to, so pinning both to one set of cases is exactly the
// check that the reduction holds - the benchmark previously compared them with only
// the recursion under test. The remaining cases are two larger even n, both wins for
// Alice. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class DivisorGameSolutionTests
{
    public static TheoryData<DivisorGameCase> Cases =>
        new()
        {
            { new DivisorGameCase(12, AliceWins: true) },
            { new DivisorGameCase(20, AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByMemoizedRecursion_Cases_MatchesExpectedOutcome(
        DivisorGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, DivisorGameSolution.CanAliceWinByMemoizedRecursion(testCase.N));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinBySpragueGrundy_Cases_MatchesExpectedOutcome(
        DivisorGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, DivisorGameSolution.CanAliceWinBySpragueGrundy(testCase.N));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByParityFormula_Cases_MatchesExpectedOutcome(
        DivisorGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, DivisorGameSolution.CanAliceWinByParityFormula(testCase.N));

    // One case: the starting n and whether Alice wins it. The expected value
    // is named at every construction site, so a row reads as the case it is rather than
    // as a bare `true` whose meaning is its position. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct DivisorGameCase(int N, bool AliceWins);
}
