using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Domain.Games.PositionalGames;

// A positional game: a board, the winning sets on it, and a turn order. Players claim open cells in
// the order TTurns gives, and the first to claim every cell of some winning set wins: the strong
// positional game, Maker-Maker in Beck's terms, which tic-tac-toe, Gomoku and Connect Four all are.
// When several players complete a set in the same turn none of them was first, so the game is
// drawn, as it is when the board fills with no set completed. A winning set through a hole could
// never be claimed whole, so the game drops every set through one. How a strategy notices a
// completed set (tallies, bitboards, rescans) is its own; these are the rules it must agree with.
/// <summary>
/// A positional game: a board, its winning sets and a turn order, with its moves judged.
/// </summary>
/// <typeparam name="TTurns">Who makes each move, and which moves make a turn.</typeparam>
public sealed class PositionalGame<TTurns>
    where TTurns : struct, ITurnOrder
{
    /// <summary>Gets the board's cells and holes.</summary>
    public BoardMask Board { get; }

    /// <summary>Gets the winning sets, every one lying wholly on the board.</summary>
    public WinningSets WinningSets { get; }

    /// <summary>
    /// Initializes a game on a board, keeping the winning sets that lie wholly on it.
    /// </summary>
    /// <param name="board">The board's cells and holes.</param>
    /// <param name="winningSets">
    /// The winning sets on the board's cells; any through a hole is dropped.
    /// </param>
    /// <exception cref="ArgumentException">The sets number a different count of cells.</exception>
    public PositionalGame(BoardMask board, WinningSets winningSets)
    {
        Board = board;
        WinningSets = winningSets.Avoiding(board);
    }

    /// <summary>Whether a cell can be claimed: it is on the board and nobody holds it.</summary>
    /// <param name="owners">
    /// The position: each cell's owner, <see cref="BoardMask.Unclaimed"/> or
    /// <see cref="BoardMask.Hole"/>.
    /// </param>
    /// <param name="cell">The cell's number.</param>
    /// <returns><see langword="true"/> when the cell is open.</returns>
    public bool IsClaimable(ReadOnlySpan<int> owners, int cell) =>
        Board.HasCell(cell) && owners[cell] == BoardMask.Unclaimed;

    /// <summary>
    /// Whether a seat holds every cell of some winning set through a cell, as after claiming it.
    /// </summary>
    /// <param name="owners">The position.</param>
    /// <param name="cell">The cell, such as the one just claimed.</param>
    /// <param name="seat">The seat whose sets to look for.</param>
    /// <returns>
    /// <see langword="true"/> when one of the sets through the cell is wholly the seat's.
    /// </returns>
    public bool HasCompletedASet(ReadOnlySpan<int> owners, int cell, int seat)
    {
        foreach (var set in WinningSets.SetsThrough(cell))
        {
            if (IsHeldBy(owners, set, seat))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Whether a seat holds every cell of some winning set anywhere on the board.
    /// </summary>
    /// <param name="owners">The position.</param>
    /// <param name="seat">The seat whose sets to look for.</param>
    /// <returns><see langword="true"/> when some winning set is wholly the seat's.</returns>
    public bool IsWonBy(ReadOnlySpan<int> owners, int seat)
    {
        for (var set = 0; set < WinningSets.Count; set++)
        {
            if (IsHeldBy(owners, set, seat))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Plays moves from the empty board and says how the game stands after them, judging each turn
    /// once its moves are in.
    /// </summary>
    /// <typeparam name="TPlacement">Which cell a move claims.</typeparam>
    /// <param name="moves">
    /// The moves, in order, numbered as the placement rule numbers them.
    /// </param>
    /// <returns>The outcome after the last move: won, drawn, or pending.</returns>
    /// <exception cref="ArgumentException">
    /// A move claims no open cell, or follows the end of the game.
    /// </exception>
    public PositionalOutcome Replay<TPlacement>(ReadOnlySpan<int> moves)
        where TPlacement : struct, IPlacementRule
    {
        var owners = Board.EmptyPosition();
        var outcome = PositionalOutcome.Pending;
        var verdict = PositionalOutcome.Pending;

        for (var move = 0; move < moves.Length; move++)
        {
            if (outcome.IsOver)
            {
                throw new ArgumentException(
                    $"Move {move} follows the end of the game.", nameof(moves));
            }

            var seat = TTurns.SeatMaking(move);
            var cell = ClaimedCell<TPlacement>(owners, moves, move, seat);
            verdict = WithClaim(verdict, owners, cell, seat);
            outcome = OutcomeAfter(move, verdict);
        }

        return outcome;
    }

    // A turn's verdict once a seat has claimed a cell: unchanged unless the claim completed a set;
    // then the seat's win if it is the first in the turn to complete one, or the seat that already
    // has, and a draw if another seat already has.
    private PositionalOutcome WithClaim(PositionalOutcome verdict, int[] owners, int cell, int seat)
    {
        if (!HasCompletedASet(owners, cell, seat))
        {
            return verdict;
        }

        var isAnotherSeatsToo = verdict.IsOver && verdict.Winner != seat;

        return isAnotherSeatsToo ? PositionalOutcome.Drawn : PositionalOutcome.WonBy(seat);
    }

    // How the game stands after a move: the turn's verdict once the turn is over, a draw when the
    // board is full, and otherwise still pending. A full board ends a turn early.
    private PositionalOutcome OutcomeAfter(int move, PositionalOutcome verdict)
    {
        var claimed = move + 1;
        var isBoardFull = claimed == Board.PlayableCount;
        var endsTurn = isBoardFull || TTurns.TurnOf(claimed) != TTurns.TurnOf(move);

        if (endsTurn && verdict.IsOver)
        {
            return verdict;
        }

        return isBoardFull ? PositionalOutcome.Drawn : PositionalOutcome.Pending;
    }

    // Puts the seat's piece on the cell its move places it in, refusing a move that names no open
    // cell.
    private int ClaimedCell<TPlacement>(int[] owners, ReadOnlySpan<int> moves, int move, int seat)
        where TPlacement : struct, IPlacementRule
    {
        if (!TPlacement.TryPlace(owners, moves[move], out var cell) || !IsClaimable(owners, cell))
        {
            throw new ArgumentException($"Move {move} claims no open cell.", nameof(moves));
        }

        owners[cell] = seat;

        return cell;
    }

    private bool IsHeldBy(ReadOnlySpan<int> owners, int set, int seat)
    {
        foreach (var cell in WinningSets.CellsOf(set))
        {
            if (owners[cell] != seat)
            {
                return false;
            }
        }

        return true;
    }
}
