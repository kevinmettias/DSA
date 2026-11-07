using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Algorithms.GameSearch.PositionalGames;

// Victory by continuous fours (VCF; Allis, 1994, "Searching for Solutions in Games and Artificial
// Intelligence", the threat-space search that solved Gomoku): a forced win in which every move of
// the attacker, the player to move, makes a four, a line one cell short of a win. The defender must
// then take that cell or lose next move, so the defender's every reply is forced and the search
// branches only on the attacker's fours, a handful a position where alpha-beta has every open
// cell. It therefore finds wins far deeper than a search of every move can look.
//
// The search, with the attacker to move:
// - a cell that wins now ends it;
// - otherwise, when the defender would win next move at two cells, nothing can stop both, and the
//   line fails; at one, the attacker's four must take that cell, since any other move loses at
//   once: the defender's counter-four, the reason a VCF is more than a chain of fours;
// - otherwise each four is tried in turn. A four with two winning cells, an open four or a double
//   four, wins: the defender takes one and the attacker the other, and the defender cannot win
//   first, having no winning cell (a stone of the attacker's never gives the defender one). A four
//   with one winning cell forces the defender there, and the search goes on from that position,
//   where the defender's block may have made a four of its own.
// The search deepens a four at a time, each pass allowing the attacker one four more, so the first
// win it finds is a shortest, and a pass that cut no line short has searched them all. Taken depth
// first, the first four tried can lead a long way: on one position a 37-ply win came back where a
// 3-ply one was there. A position found lost is remembered with the fours it was searched with,
// since fours made in another order reach it again, and it answers only a search allowing no more.
// Every move adds a stone, so the search ends. What counts as a four or a win is the game's lines'
// (InARowLines.CellsLacking), under its overline rule: under standard gomoku's exactly five, a cell
// that would make six is no winning cell. Threats of three, which a full threat-space search adds
// (victory by continuous threats), are not searched.
/// <summary>
/// Searches an m,n,k game for a victory by continuous fours: a forced win for the player to move
/// in which every one of that player's moves makes a four.
/// </summary>
public static class ContinuousFours
{
    /// <summary>Searches for a victory by continuous fours from a position.</summary>
    /// <typeparam name="TShape">The board's shape, and how many in a row win.</typeparam>
    /// <typeparam name="TOverline">Whether a run longer than k wins.</typeparam>
    /// <param name="game">The game's rules.</param>
    /// <param name="position">The position, the attacker to move.</param>
    /// <param name="sequence">
    /// When found, the cells played in order from the position: the attacker's four, the
    /// defender's forced block, and so on, ending with the attacker's winning move; empty
    /// otherwise.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the player to move has a victory by continuous fours.
    /// </returns>
    public static bool TryFindWin<TShape, TOverline>(
        InARowGame<TShape, TOverline> game, Bitboard256Position position, out int[] sequence)
        where TShape : struct, IMnkShape
        where TOverline : struct, IOverlineRule
        => TryFindWin(game, position, out sequence, out _);

    // The search, also reporting how many positions it looked at.
    internal static bool TryFindWin<TShape, TOverline>(
        InARowGame<TShape, TOverline> game,
        Bitboard256Position position,
        out int[] sequence,
        out long positions)
        where TShape : struct, IMnkShape
        where TOverline : struct, IOverlineRule
    {
        var search = new FourSearch<TShape, TOverline>(game.Lines);
        var isWon = !position.IsLost && search.IsWon(position.Mover, position.Opponent);
        sequence = search.Sequence();
        positions = search.Positions;

        return isWon;
    }

    // One search: the line of play it is following, and the positions it found lost for the
    // attacker with how many fours.
    private sealed class FourSearch<TShape, TOverline>(InARowLines<TShape, TOverline> lines)
        where TShape : struct, IMnkShape
        where TOverline : struct, IOverlineRule
    {
        // The cells of lines one short of a win, where a move wins, and of lines two short, where
        // a move makes a four.
        private const int WinningCells = InARowLines<TShape, TOverline>.OneShort;
        private const int FourCells = InARowLines<TShape, TOverline>.TwoShort;

        // Each four the search tries plays two moves: the four, and the defender's block.
        private const int MovesPerFour = 2;

        // Each position found lost, with the most fours it was searched with.
        private readonly Dictionary<(Bitboard256 Attacker, Bitboard256 Defender), int> _failed = [];
        private readonly List<int> _line = [];

        // Whether the pass running stopped a line of play for want of fours.
        private bool _isCut;

        // How many positions the search reached: every four and every block it played.
        public long Positions { get; private set; }

        // Whether the attacker, to move, wins by continuous fours; when so, the line ends in it.
        // Each pass allows one four more than the last, so the first win found is a shortest; a
        // pass that cut no line short has searched every line, and then there is none.
        public bool IsWon(Bitboard256 attacker, Bitboard256 defender)
        {
            for (var fours = 0; ; fours++)
            {
                _isCut = false;

                if (IsWonWithin(attacker, defender, fours))
                {
                    return true;
                }

                if (!_isCut)
                {
                    return false;
                }
            }
        }

        // The cells played, as the board numbers them.
        public int[] Sequence() => [.. _line.Select(InARowLines<TShape, TOverline>.CellOf)];

        // Whether the attacker wins with at most this many fours before the winning move.
        private bool IsWonWithin(Bitboard256 attacker, Bitboard256 defender, int fours)
        {
            var wins = lines.CellsLacking(attacker, defender, WinningCells);

            if (!wins.IsEmpty)
            {
                _line.Add(wins.LowestCell);

                return true;
            }

            var threats = lines.CellsLacking(defender, attacker, WinningCells);
            var isLost = threats.Count > 1 || HasFailedWith(attacker, defender, fours);

            if (isLost || IsWonByAFour(attacker, defender, threats, fours))
            {
                return !isLost;
            }

            _failed[(attacker, defender)] = fours;

            return false;
        }

        private bool HasFailedWith(Bitboard256 attacker, Bitboard256 defender, int fours) =>
            _failed.TryGetValue((attacker, defender), out var searched) && searched >= fours;

        // Tries each four in turn; with no four left to play, notes that the line was cut.
        private bool IsWonByAFour(
            Bitboard256 attacker, Bitboard256 defender, Bitboard256 threats, int fours)
        {
            var candidates = FoursToTry(attacker, defender, threats);

            if (fours == 0)
            {
                _isCut |= !candidates.IsEmpty;

                return false;
            }

            for (; !candidates.IsEmpty; candidates = candidates.WithoutLowestCell())
            {
                if (IsWonAfter(attacker, defender, candidates.LowestCell, fours - 1))
                {
                    return true;
                }
            }

            return false;
        }

        // The attacker's fours that take the defender's one winning cell when there is one, since
        // any other move loses at once; else every four.
        private Bitboard256 FoursToTry(Bitboard256 attacker, Bitboard256 defender, Bitboard256 threats)
        {
            var fours = lines.CellsLacking(attacker, defender, FourCells);

            if (!threats.IsEmpty)
            {
                fours &= threats;
            }

            return fours;
        }

        // Whether the four at a cell wins: at once when it leaves two winning cells, else after
        // the defender's forced block of the one.
        private bool IsWonAfter(Bitboard256 attacker, Bitboard256 defender, int four, int fours)
        {
            var after = attacker | Bitboard256.Cell(four);
            var replies = lines.CellsLacking(after, defender, WinningCells);
            var block = replies.LowestCell;
            _line.AddRange([four, block]);
            Positions += MovesPerFour;

            if (replies.Count > 1)
            {
                _line.Add(replies.WithoutLowestCell().LowestCell);

                return true;
            }

            if (IsWonWithin(after, defender | Bitboard256.Cell(block), fours))
            {
                return true;
            }

            _line.RemoveRange(_line.Count - MovesPerFour, MovesPerFour);

            return false;
        }
    }
}
