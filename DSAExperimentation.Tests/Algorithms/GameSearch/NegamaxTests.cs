using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// A win-or-lose game against its known pattern, and a scoring game against LeetCode's worked examples
// of the same rule (LC 486: [1, 5, 2] gives the first player 3 against 5, so -2; [1, 5, 233, 7] gives
// 234 against 12, so 222). To a fixed depth, with an estimate of nothing at the limit, the take-away
// game's value sees exactly the wins within the depth, and to the end it is the exact value.
public sealed partial class NegamaxTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 40;

    // Plain negamax searches every line of play, about 1.84^n positions from n stones, so it is
    // checked to the end on smaller piles.
    private const int LargestPlainPile = 14;

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

    [Fact]
    public void ValueToDepth_DepthZero_IsTheEstimate() =>
        Assert.Equal(0, TakeAwayToDepth(1, 0));

    // Three stones, one ply: taking all three wins at once.
    [Fact]
    public void ValueToDepth_AWinWithinTheDepth_IsSeen() =>
        Assert.Equal(1, TakeAwayToDepth(3, 1));

    // Four stones, one ply: every move leaves a pile still in play, so all the search sees is the
    // estimate, although the position is lost.
    [Fact]
    public void ValueToDepth_ALossBeyondTheDepth_IsTheEstimate() =>
        Assert.Equal(0, TakeAwayToDepth(4, 1));

    [Fact]
    public void ValueToDepth_ToTheEnd_IsTheExactValue() =>
        Assert.All(
            Enumerable.Range(0, LargestPlainPile + 1),
            pile => Assert.Equal(
                ExpectedTakeAwayValue(pile),
                Negamax.ValueToDepth<TakeAwayGame, UnreachedEvaluation<int>, int, int>(
                    TakeAway(), default, pile, pile)));

    private static TakeAwayGame TakeAway() => new(MostPerTurn);

    // Plain negamax to the depth, with an estimate of nothing at the limit.
    private static int TakeAwayToDepth(int pile, int depth) =>
        Negamax.ValueToDepth<TakeAwayGame, ZeroEvaluation<int>, int, int>(
            TakeAway(), default, pile, depth);

    // A pile that is a multiple of four loses, any other wins.
    private static int ExpectedTakeAwayValue(int pile)
    {
        var losing = pile % (MostPerTurn + 1) == 0;

        return losing ? -1 : 1;
    }
}
