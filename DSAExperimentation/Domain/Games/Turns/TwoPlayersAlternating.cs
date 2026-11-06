namespace DSAExperimentation.Domain.Games.Turns;

// Two players taking turns, the first player first: the turn order of tic-tac-toe, Connect Four,
// chess and Go, and the one IAlternatingGame's solvers assume. It is round robin over two players,
// and answers as that order does, so a move's seat is the move's low bit (RoundRobin's unsigned
// remainder by 2 folds to a mask). What it adds is what only two players have: each seat has one
// opponent, and the two seats have names.
/// <summary>Two players taking turns, one move a turn, the first player first.</summary>
public readonly struct TwoPlayersAlternating : ITurnOrder
{
    /// <summary>The seat of the player who moves first, 0.</summary>
    public const int First = 0;

    /// <summary>The seat of the player who moves second, 1.</summary>
    public const int Second = First + 1;

    /// <inheritdoc/>
    public static int PlayerCount => RoundRobin<TwoPlayers>.PlayerCount;

    /// <inheritdoc/>
    public static int MovesPerTurn => RoundRobin<TwoPlayers>.MovesPerTurn;

    /// <inheritdoc/>
    public static int SeatMaking(int move) => RoundRobin<TwoPlayers>.SeatMaking(move);

    /// <inheritdoc/>
    public static int TurnOf(int move) => RoundRobin<TwoPlayers>.TurnOf(move);

    /// <summary>
    /// Finds a seat's opponent: the first player's is the second's, and the second's the first's.
    /// </summary>
    /// <param name="seat"><see cref="First"/> or <see cref="Second"/>.</param>
    /// <returns>The other seat.</returns>
    public static int OpponentOf(int seat) => First + Second - seat;
}
