using DSAExperimentation.Domain.Games.Turns;
using DSAExperimentation.Tests.Domain.Games.Turns.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// Under simultaneous moves every player moves each turn: a turn is P moves, written seat by seat,
// that share the turn's number.
public sealed partial class SimultaneousTests
{
    private const int Turns = 3;
    private const int Moves = Turns * ThreePlayers.Count;

    [Fact]
    public void PlayerCount_IsThePlayersCount() =>
        Assert.Equal(ThreePlayers.Count, Simultaneous<ThreePlayers>.PlayerCount);

    [Fact]
    public void MovesPerTurn_IsOneMovePerPlayer() =>
        Assert.Equal(
            [TwoPlayers.Count, ThreePlayers.Count],
            [Simultaneous<TwoPlayers>.MovesPerTurn, Simultaneous<ThreePlayers>.MovesPerTurn]);

    [Fact]
    public void SeatMaking_ATurnsMovesRunSeatBySeat() =>
        Assert.Equal(
            [0, 1, 2, 0, 1, 2, 0, 1, 2],
            TurnOrders.Seats<Simultaneous<ThreePlayers>>(Moves));

    [Fact]
    public void TurnOf_ATurnsMovesShareItsNumber() =>
        Assert.Equal(
            [0, 0, 0, 1, 1, 1, 2, 2, 2],
            TurnOrders.Turns<Simultaneous<ThreePlayers>>(Moves));
}
