namespace DSAExperimentation.Domain.Games.Turns;

// Who moves when. The players sit in seats numbered from 0 in the order they first move, and a
// game's moves are numbered from 0 in the order they are written down, the moves of one turn seat
// by seat. A turn order says which seat makes each move and which turn the move belongs to. The
// moves of one turn are made together, none seeing another, so a game judges a turn once all its
// moves are in: one move a turn when the players take turns, every player's at once when they move
// simultaneously.
//
// Each order is a struct witness with static members, so a game generic over it compiles its own
// arithmetic: two players alternating is a mask of the move's low bit.
/// <summary>
/// A turn order: how many players there are, and which seat makes each move in which turn.
/// </summary>
public interface ITurnOrder
{
    /// <summary>Gets how many players take part.</summary>
    static abstract int PlayerCount { get; }

    /// <summary>
    /// Gets how many moves make up one turn: one when players take turns, one per player when they
    /// move together.
    /// </summary>
    static abstract int MovesPerTurn { get; }

    /// <summary>Finds the seat that makes a move.</summary>
    /// <param name="move">The move's number, from 0.</param>
    /// <returns>The seat making it, from 0.</returns>
    static abstract int SeatMaking(int move);

    /// <summary>Finds the turn a move belongs to.</summary>
    /// <param name="move">The move's number, from 0.</param>
    /// <returns>The turn's number, from 0.</returns>
    static abstract int TurnOf(int move);
}
