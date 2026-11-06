namespace DSAExperimentation.Domain.Games.Turns;

// Any number of players taking turns in seat order and starting again from the first, one move a
// turn, as in three-player Chinese checkers or a four-player game of Blokus. The count is a
// constant of TPlayers, so the remainder below is a division by a constant, which the JIT turns
// into a multiplication; it is taken unsigned because a move's number is never negative.
/// <summary>
/// Players taking turns in seat order, one move a turn, the first seat again after the last.
/// </summary>
/// <typeparam name="TPlayers">How many players there are.</typeparam>
public readonly struct RoundRobin<TPlayers> : ITurnOrder
    where TPlayers : struct, IPlayerCount
{
    // One player moves in each turn.
    private const int MovesEachTurn = 1;

    /// <inheritdoc/>
    public static int PlayerCount => TPlayers.Players;

    /// <inheritdoc/>
    public static int MovesPerTurn => MovesEachTurn;

    /// <inheritdoc/>
    public static int SeatMaking(int move) => (int)((uint)move % (uint)TPlayers.Players);

    /// <inheritdoc/>
    public static int TurnOf(int move) => move;
}
