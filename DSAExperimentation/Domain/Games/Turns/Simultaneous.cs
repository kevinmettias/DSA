namespace DSAExperimentation.Domain.Games.Turns;

// Every player moving at once, each turn, as in rock-paper-scissors or Diplomacy: a turn is one
// move from each seat, written down seat by seat, and none of them sees the others. Moves of the
// same turn share its number, which is what tells a game to judge them together. As in RoundRobin,
// the count is a constant of TPlayers and the divisions are unsigned.
/// <summary>Every player moving together each turn, a turn's moves written seat by seat.</summary>
/// <typeparam name="TPlayers">How many players there are.</typeparam>
public readonly struct Simultaneous<TPlayers> : ITurnOrder
    where TPlayers : struct, IPlayerCount
{
    /// <inheritdoc/>
    public static int PlayerCount => TPlayers.Players;

    /// <inheritdoc/>
    public static int MovesPerTurn => TPlayers.Players;

    /// <inheritdoc/>
    public static int SeatMaking(int move) => (int)((uint)move % (uint)TPlayers.Players);

    /// <inheritdoc/>
    public static int TurnOf(int move) => (int)((uint)move / (uint)TPlayers.Players);
}
