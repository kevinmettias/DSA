using System.Collections.Immutable;
using System.Numerics;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Algorithms.GameSearch.PositionalGames;

// A static evaluation for a positional game, read off its winning sets: a set only one player has
// claimed cells of is still open to that player, and the fewer cells it lacks, the closer that
// player is to winning on it. A set one cell short is a threat, Connect Four's open three; two
// short is an open two. Each such set counts for its holder, the mover's for and the opponent's
// against, and a set both players have claimed in is dead and counts for nobody. A position whose
// threats run along several sets is worth the sum, so a line of four with two ends open in Gomoku,
// which lies on two sets of five, counts twice. Every set is a mask of up to 64 cells, so a set's
// count for each player is one population count. The sum stays far below BitboardGame.WinValue:
// even 130 sets, the most an 8 by 8 board of four in a row has, reach only a few thousand.
/// <summary>
/// Estimates a positional game's position from the winning sets each player alone has claimed
/// cells of: one cell short and two cells short of a win.
/// </summary>
public readonly struct OpenLinesEvaluation : IStaticEvaluation<BitboardPosition, int>
{
    // What a set one cell short of a win is worth to its holder, a threat, and one two cells short.
    // Internal, so that a solution's own 16 or 4 is not read as a copy of them (ARCHITECTURE 17.9).
    internal const int OneShortWeight = 16;
    internal const int TwoShortWeight = 4;

    // Every winning set as a mask, in the sets' order.
    private readonly ImmutableArray<ulong> _sets;

    // A set held by one player alone is worth its weight by how many cells it lacks: one or two.
    private static ReadOnlySpan<int> WeightByShortfall => [0, OneShortWeight, TwoShortWeight];

    /// <summary>Initializes the evaluation over a game's winning sets.</summary>
    /// <param name="winningSets">The winning sets, on a board of at most 64 cells.</param>
    public OpenLinesEvaluation(WinningSets winningSets)
    {
        var sets = new ulong[winningSets.Count];

        for (var set = 0; set < sets.Length; set++)
        {
            sets[set] = BitboardPosition.MaskOf(winningSets.CellsOf(set));
        }

        _sets = [.. sets];
    }

    /// <inheritdoc/>
    public int Evaluate(BitboardPosition state)
    {
        var score = 0;

        foreach (var set in _sets)
        {
            var size = BitOperations.PopCount(set);
            var mine = BitOperations.PopCount(state.Mover & set);
            var theirs = BitOperations.PopCount(state.Opponent & set);

            if (theirs == 0)
            {
                score += WeightOf(size, mine);
            }

            if (mine == 0)
            {
                score -= WeightOf(size, theirs);
            }
        }

        return score;
    }

    // The weight of a set of this size holding this many of one player's cells and none of the
    // other's; nothing when it holds none.
    private static int WeightOf(int size, int held)
    {
        var shortfall = size - held;
        var isWeighed = held > 0 && shortfall < WeightByShortfall.Length;

        if (!isWeighed)
        {
            return 0;
        }

        return WeightByShortfall[shortfall];
    }
}
