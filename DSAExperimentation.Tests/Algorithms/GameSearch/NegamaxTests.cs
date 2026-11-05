using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// A win-or-lose game against its known pattern, and a scoring game against LeetCode's worked examples
// of the same rule (LC 486: [1, 5, 2] gives the first player 3 against 5, so -2; [1, 5, 233, 7] gives
// 234 against 12, so 222).
public sealed partial class NegamaxTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 40;

    public static TheoryData<int[], int> EndPickRows => new()
    {
        { [1, 5, 2], -2 },
        { [1, 5, 233, 7], 222 },
        { [7], 7 },
    };

    [Fact]
    public void Value_TakeAway_LosesExactlyOnMultiplesOfFour() =>
        Assert.All(
            Enumerable.Range(0, LargestPile + 1),
            pile => Assert.Equal(ExpectedTakeAwayValue(pile), Negamax.Value<TakeAwayGame, int, int>(new TakeAwayGame(MostPerTurn), pile)));

    [Theory]
    [MemberData(nameof(EndPickRows))]
    public void Value_EndPick_IsTheDifferenceTheFirstPlayerCanForce(int[] row, int expected) =>
        Assert.Equal(expected, Negamax.Value<EndPickGame, (int, int), int>(new EndPickGame(row), (0, row.Length - 1)));

    // A pile that is a multiple of four loses, any other wins.
    private static int ExpectedTakeAwayValue(int pile)
    {
        var losing = pile % (MostPerTurn + 1) == 0;

        return losing ? -1 : 1;
    }
}
