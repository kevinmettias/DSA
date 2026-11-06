using DSAExperimentation.Domain.Games.Turns;
using DSAExperimentation.Tests.Domain.Games.Turns.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// Two players alternating is round robin over two players, checked move for move, plus the one fact
// only two players have: each seat's single opponent.
public sealed partial class TwoPlayersAlternatingTests
{
    private const int MovesChecked = 20;

    public static TheoryData<int, int> MovesToTheirSeats => new()
    {
        { 0, TwoPlayersAlternating.First },
        { 1, TwoPlayersAlternating.Second },
        { 2, TwoPlayersAlternating.First },
        { 9, TwoPlayersAlternating.Second },
    };

    public static TheoryData<int, int> SeatsToTheirOpponents => new()
    {
        { TwoPlayersAlternating.First, TwoPlayersAlternating.Second },
        { TwoPlayersAlternating.Second, TwoPlayersAlternating.First },
    };

    [Fact]
    public void PlayerCount_IsTwo() => Assert.Equal(2, TwoPlayersAlternating.PlayerCount);

    [Fact]
    public void MovesPerTurn_IsOne() => Assert.Equal(1, TwoPlayersAlternating.MovesPerTurn);

    [Theory]
    [MemberData(nameof(MovesToTheirSeats))]
    public void SeatMaking_TheFirstSeatMovesFirstAndTheTwoAlternate(int move, int expected) =>
        Assert.Equal(expected, TwoPlayersAlternating.SeatMaking(move));

    [Fact]
    public void SeatMaking_IsRoundRobinOverTwoPlayers() =>
        Assert.Equal(
            TurnOrders.Seats<RoundRobin<TwoPlayers>>(MovesChecked),
            TurnOrders.Seats<TwoPlayersAlternating>(MovesChecked));

    [Fact]
    public void TurnOf_EveryMoveIsATurnOfItsOwn() =>
        Assert.Equal(
            Enumerable.Range(0, MovesChecked),
            TurnOrders.Turns<TwoPlayersAlternating>(MovesChecked));

    [Theory]
    [MemberData(nameof(SeatsToTheirOpponents))]
    public void OpponentOf_IsTheOtherSeat(int seat, int expected) =>
        Assert.Equal(expected, TwoPlayersAlternating.OpponentOf(seat));
}
