using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIV;

// Harness only: both strategies live in StoneGameIVSolution and are asserted against
// the same examples. The un-memoized recursion is the definition and the memoized one
// is the same recurrence with a cache, so pinning both to one example set is exactly
// the check that memoization changed nothing - the benchmark previously compared them
// with only the memoized arm under test.
public sealed partial class StoneGameIVTests
{
    public static TheoryData<TakeAwaySquaresExample> Examples =>
        new()
        {
            new TakeAwaySquaresExample(1, AliceWins: true),
            new TakeAwaySquaresExample(2, AliceWins: false),
            new TakeAwaySquaresExample(3, AliceWins: true),
            new TakeAwaySquaresExample(4, AliceWins: true),
            new TakeAwaySquaresExample(5, AliceWins: false),
            new TakeAwaySquaresExample(6, AliceWins: true),
            new TakeAwaySquaresExample(7, AliceWins: false),
            new TakeAwaySquaresExample(8, AliceWins: true),
            new TakeAwaySquaresExample(9, AliceWins: true),
            new TakeAwaySquaresExample(10, AliceWins: false),
            new TakeAwaySquaresExample(17, AliceWins: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByUnmemoizedRecursion_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByMemoizedRecursion_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIVSolution.CanAliceWinByMemoizedRecursion(example.N));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TakeAwaySquaresExample(int N, bool AliceWins);
}
