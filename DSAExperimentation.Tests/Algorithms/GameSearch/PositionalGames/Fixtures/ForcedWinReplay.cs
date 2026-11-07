using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;

// Replays a forced win from a drawn Gomoku position, test-only, against MnkGame's lines read one
// at a time (LineOracle) rather than the shifts a search reads, and lists what is wrong with it:
// a move on a held cell; a reply of the defender's while the defender could win at once, or while
// the attacker had no winning cell, or other than the one winning cell the attacker had; an
// attacker's move that wins before the last, or a last that does not.
internal sealed class ForcedWinReplay<TOverline>(string[] rows, int[] sequence)
    where TOverline : struct, IOverlineRule
{
    private const int Win = 1;

    private readonly bool[] _attacker = Owned(GomokuDiagram.Cells(rows).Mover);
    private readonly bool[] _defender = Owned(GomokuDiagram.Cells(rows).Opponent);
    private readonly List<string> _problems = [];

    private static BoardMask Board => MnkGame<GomokuBoard>.Board;

    // What is wrong with the sequence; none when it is a forced win.
    public string[] Problems()
    {
        for (var at = 0; at < sequence.Length; at++)
        {
            var cell = sequence[at];

            if (_attacker[cell] || _defender[cell])
            {
                _problems.Add($"move {at} takes a held cell");
            }

            if (at % 2 == 0)
            {
                Attack(at, cell);
            }
            else
            {
                Reply(at, cell);
            }
        }

        return [.. _problems];
    }

    private static bool[] Owned(int[] cells)
    {
        var owned = new bool[MnkGame<GomokuBoard>.CellCount];
        Array.ForEach(cells, cell => owned[cell] = true);

        return owned;
    }

    private void Attack(int at, int cell)
    {
        _attacker[cell] = true;
        var isWon = LineOracle<GomokuBoard, TOverline>.HasRun(Board, _attacker);
        var isLast = at == sequence.Length - 1;

        if (isWon != isLast)
        {
            _problems.Add($"move {at} wins early or not at the end");
        }
    }

    // The reply is forced: the defender cannot win at once, and the attacker would win next move,
    // at this cell when it is the only one.
    private void Reply(int at, int cell)
    {
        var defenderWins = LineOracle<GomokuBoard, TOverline>.CellsLacking(
            Board, _defender, _attacker, Win);
        var attackerWins = LineOracle<GomokuBoard, TOverline>.CellsLacking(
            Board, _attacker, _defender, Win);
        _defender[cell] = true;

        if (defenderWins.Length > 0)
        {
            _problems.Add($"reply {at} could have won");
        }

        if (attackerWins.Length == 0)
        {
            _problems.Add($"reply {at} was not forced");
        }

        if (attackerWins.Length == 1 && attackerWins[0] != cell)
        {
            _problems.Add($"reply {at} missed the block");
        }
    }
}
