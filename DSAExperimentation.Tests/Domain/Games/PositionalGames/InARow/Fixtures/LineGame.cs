using System.Collections.Immutable;
using System.Numerics;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

// A game won by holding a whole winning set, as the game contract sees it, test-only, for boards
// of up to 32 cells: a position is the mover's cells in the low 32 bits and the opponent's in the
// high, so a move claims the move-th open cell and swaps the halves. The player who just moved is
// the opponent; when they hold a whole set the mover has lost, and a full board with no set is a
// draw. Negamax over it solves the game exactly from its sets, which tests the contract and
// MnkGame's lines together.
internal readonly struct LineGame(WinningSets lines) : IAlternatingGame<long, int>
{
    private const int Loss = -1;
    private const int Draw = 0;
    private const int HalfBits = 32;

    private readonly ImmutableArray<long> _setMasks = MasksOf(lines);
    private readonly long _board = (1L << lines.CellCount) - 1;

    public int MoveCount(long state)
    {
        var open = OpenCells(state);

        if (HasSet(Opponents(state)))
        {
            return 0;
        }

        return BitOperations.PopCount((ulong)open);
    }

    public long Next(long state, int move)
    {
        var open = OpenCells(state);

        for (var skipped = 0; skipped < move; skipped++)
        {
            open &= open - 1;
        }

        var claimed = Movers(state) | (open & -open);

        return Opponents(state) | (claimed << HalfBits);
    }

    public int Gain(long state, int move) => 0;

    public int ValueWhenOver(long state) => HasSet(Opponents(state)) ? Loss : Draw;

    private static ImmutableArray<long> MasksOf(WinningSets sets)
    {
        var masks = new long[sets.Count];

        for (var set = 0; set < masks.Length; set++)
        {
            foreach (var cell in sets.CellsOf(set))
            {
                masks[set] |= 1L << cell;
            }
        }

        return [.. masks];
    }

    private static long Movers(long state) => state & uint.MaxValue;

    private static long Opponents(long state) => (long)((ulong)state >> HalfBits);

    private long OpenCells(long state) => ~(Movers(state) | Opponents(state)) & _board;

    private bool HasSet(long cells)
    {
        foreach (var mask in _setMasks)
        {
            if ((cells & mask) == mask)
            {
                return true;
            }
        }

        return false;
    }
}
