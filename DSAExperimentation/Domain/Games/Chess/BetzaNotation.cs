using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Domain.Games.Chess;

// Ralph Betza's "funny notation", which writes a piece of chess or of a chess variant as a run of
// moves, each an atom (the leap it makes) after any modifiers and before any range. The atoms read
// here are W (the wazir, one step orthogonally), F (the ferz, one step diagonally), D (the dabbaba,
// two steps orthogonally), A (the alfil, two steps diagonally) and N (the knight), with K, R, B and Q
// short for WF, WW, FF and WWFF. An atom written twice rides, and one followed by a number rides at
// most that far. The modifiers read are m (moves only), c (captures only), i (first move only), and f
// and b, which keep the steps forward or backward for the side the piece plays for. The notation has
// more - left and right, sideways, lame and hopping moves among them - and a letter this does not read
// throws, rather than giving the piece a move it does not have.
//
// It reads notation to build and check pieces, never on a search's path: ChessPiece's presets are
// tables fixed in code, and the tests hold each to its Betza string.
/// <summary>Reads Betza notation into a piece's moves.</summary>
public static class BetzaNotation
{
    private const string UnreadLetterMessage = "Betza notation letter this reader does not read";
    private const string MissingAtomMessage = "Betza notation modifiers with no atom after them";

    private static readonly ImmutableArray<GridStep> Wazir = GridDirections.Orthogonal;
    private static readonly ImmutableArray<GridStep> Ferz = GridDirections.Diagonal;
    private static readonly ImmutableArray<GridStep> Knight = GridDirections.LShaped;
    private static readonly ImmutableArray<GridStep> Dabbaba = [.. GridDirections.Orthogonal.Select(step => step + step)];
    private static readonly ImmutableArray<GridStep> Alfil = [.. GridDirections.Diagonal.Select(step => step + step)];

    /// <summary>Reads a piece's notation into its moves, facing the given side.</summary>
    /// <param name="notation">The piece's Betza notation, such as "mfWcfFifmW2" for a pawn.</param>
    /// <param name="side">The side the piece plays for, which says which way is forward.</param>
    /// <returns>The piece's moves, one for each atom the notation names.</returns>
    /// <exception cref="ArgumentException">The notation holds a letter this reader does not read, or modifiers with no atom after them.</exception>
    public static ImmutableArray<PieceMove> Parse(string notation, ChessSide side)
    {
        var moves = ImmutableArray.CreateBuilder<PieceMove>();
        var position = 0;

        while (position < notation.Length)
        {
            var (when, facing) = ReadModifiers(notation, ref position);
            var atomIndex = position;
            var atom = ReadAtom(notation, ref position);
            var (written, range) = ReadRange(notation, ref position, atom);

            foreach (var (steps, rides) in Expand(notation, atomIndex))
            {
                // No range written: a shorthand that rides has none, and any other atom leaps once.
                var reach = range;

                if (!written && !rides)
                {
                    reach = 1;
                }

                moves.Add(new PieceMove(Facing(steps, facing, side), reach, when));
            }
        }

        return moves.ToImmutable();
    }

    private static (MoveConditions When, int Facing) ReadModifiers(string notation, ref int position)
    {
        var moves = false;
        var captures = false;
        var firstMoveOnly = false;
        var facing = 0;

        for (; position < notation.Length && char.IsLower(notation[position]); position++)
        {
            switch (notation[position])
            {
                case 'm': moves = true; break;
                case 'c': captures = true; break;
                case 'i': firstMoveOnly = true; break;
                case 'f': facing = 1; break;
                case 'b': facing = -1; break;
                default: throw Unread(notation, position);
            }
        }

        // Both m and c, or neither, is the usual move that may go to an empty square or capture.
        var when = MoveConditions.MovesOrCaptures;

        if (moves != captures)
        {
            when = moves ? MoveConditions.Moves : MoveConditions.Captures;
        }

        if (firstMoveOnly)
        {
            when |= MoveConditions.FirstMoveOnly;
        }

        return (when, facing);
    }

    private static char ReadAtom(string notation, ref int position)
    {
        if (position >= notation.Length)
        {
            throw new ArgumentException($"{MissingAtomMessage}: \"{notation}\".", nameof(notation));
        }

        return notation[position++];
    }

    // An atom written again rides with no limit, and a number after it rides at most that far; whether
    // a range was written at all is apart from the range, since a rider's is none.
    private static (bool Written, int? Range) ReadRange(string notation, ref int position, char atom)
    {
        if (position < notation.Length && notation[position] == atom)
        {
            position++;
            return (true, null);
        }

        var start = position;

        while (position < notation.Length && char.IsAsciiDigit(notation[position]))
        {
            position++;
        }

        if (position == start)
        {
            return (false, null);
        }

        var digits = notation.AsSpan(start, position - start);

        return (true, int.Parse(digits));
    }

    // An atom's leaps, and whether each rides when no range is written: the shorthands R, B and Q ride.
    private static IEnumerable<(ImmutableArray<GridStep> Steps, bool Rides)> Expand(string notation, int atomIndex) => notation[atomIndex] switch
    {
        'W' => [(Wazir, false)],
        'F' => [(Ferz, false)],
        'D' => [(Dabbaba, false)],
        'A' => [(Alfil, false)],
        'N' => [(Knight, false)],
        'K' => [(Wazir, false), (Ferz, false)],
        'R' => [(Wazir, true)],
        'B' => [(Ferz, true)],
        'Q' => [(Wazir, true), (Ferz, true)],
        _ => throw Unread(notation, atomIndex),
    };

    // Forward and backward are along the side's forward step; a step with no part along it is neither.
    private static ImmutableArray<GridStep> Facing(ImmutableArray<GridStep> steps, int facing, ChessSide side)
    {
        if (facing == 0)
        {
            return steps;
        }

        var wanted = facing * Math.Sign(side.Forward.DeltaRow);

        return [.. steps.Where(step => Math.Sign(step.DeltaRow) == wanted)];
    }

    private static ArgumentException Unread(string notation, int position) =>
        new($"{UnreadLetterMessage}: '{notation[position]}' at {position} in \"{notation}\".", nameof(notation));
}
