using DSAExperimentation.Domain.Games.Turns;
using DSAExperimentation.Tests.Domain.Games.Turns.Fixtures;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// Round robin cycles its P players in seat order, one move a turn: over three players the seats run
// 0, 1, 2, 0, 1, 2, ..., and over four 0, 1, 2, 3, 0, ....
public sealed partial class RoundRobinTests
{
    private const int Rounds = 3;
    private const int ThreeRounds = Rounds * ThreePlayers.Count;
    private const int FourRounds = Rounds * FourPlayers.Count;

    [Fact]
    public void PlayerCount_IsThePlayersCount() =>
        Assert.Equal(
            [ThreePlayers.Count, FourPlayers.Count],
            [RoundRobin<ThreePlayers>.PlayerCount, RoundRobin<FourPlayers>.PlayerCount]);

    [Fact]
    public void MovesPerTurn_IsOne() => Assert.Equal(1, RoundRobin<FourPlayers>.MovesPerTurn);

    [Fact]
    public void SeatMaking_ThreePlayers_CyclesThroughEverySeatInOrder() =>
        Assert.Equal(
            [0, 1, 2, 0, 1, 2, 0, 1, 2],
            TurnOrders.Seats<RoundRobin<ThreePlayers>>(ThreeRounds));

    [Fact]
    public void SeatMaking_FourPlayers_CyclesThroughEverySeatInOrder() =>
        Assert.Equal(
            [0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3],
            TurnOrders.Seats<RoundRobin<FourPlayers>>(FourRounds));

    [Fact]
    public void TurnOf_EveryMoveIsATurnOfItsOwn() =>
        Assert.Equal(
            Enumerable.Range(0, ThreeRounds),
            TurnOrders.Turns<RoundRobin<ThreePlayers>>(ThreeRounds));
}
