using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Domain.Games.Life;

// Whether a cell is live in the next generation under a Life-like rule, asked one way for a dead cell
// and another for a live one, so a caller that knows its cell's state picks one rather than passing
// it in as a flag. The rulestring is read once per rule into two masks, bit n standing for n live
// neighbours: primitive static readonly fields of an instantiation over a struct, which the JIT reads
// as constants once the type is initialized, so each test is a shift and a mask. A malformed
// rulestring is a broken definition, not input to recover from, so it fails when the rule is first
// asked. A count is the number of live neighbours, 0 through 8; the board's encoding of live and dead,
// and which cells count as neighbours, stay with each problem.
/// <summary>The next-generation rule of a Life-like automaton.</summary>
/// <typeparam name="TRule">The rule, such as <see cref="ConwayLife"/>.</typeparam>
public static class LifeLike<TRule>
    where TRule : struct, ILifeLikeRule
{
    private const char BirthMark = 'B';
    private const char SurvivalMark = 'S';
    private const char Divider = '/';
    private const int MostNeighbors = 8;
    private const int HalfCount = 2;
    private const string MalformedMessage = "A Life-like rulestring is B, birth counts, '/', S, survival counts, each count 0 to 8.";

    private static readonly int BirthMask = MaskOf(Half(0), BirthMark);
    private static readonly int SurvivalMask = MaskOf(Half(1), SurvivalMark);

    /// <summary>Whether a dead cell with this many live neighbours is born.</summary>
    /// <param name="liveNeighbors">How many of the cell's neighbours are live, 0 through 8.</param>
    /// <returns><see langword="true"/> when the rule's birth counts hold the count; otherwise <see langword="false"/>.</returns>
    public static bool IsBirthCount(int liveNeighbors) => ((BirthMask >> liveNeighbors) & 1) != 0;

    /// <summary>Whether a live cell with this many live neighbours survives.</summary>
    /// <param name="liveNeighbors">How many of the cell's neighbours are live, 0 through 8.</param>
    /// <returns><see langword="true"/> when the rule's survival counts hold the count; otherwise <see langword="false"/>.</returns>
    public static bool IsSurvivalCount(int liveNeighbors) => ((SurvivalMask >> liveNeighbors) & 1) != 0;

    // One side of the '/': the birth half, then the survival half.
    private static string Half(int index)
    {
        var halves = TRule.Rulestring.Split(Divider);

        if (halves.Length != HalfCount)
        {
            throw new InvalidOperationException(MalformedMessage);
        }

        return halves[index];
    }

    // A half's mark, then its counts, each setting its own bit.
    private static int MaskOf(string half, char mark)
    {
        if (half.Length == 0 || half[0] != mark)
        {
            throw new InvalidOperationException(MalformedMessage);
        }

        var mask = 0;

        foreach (var digit in half.AsSpan(1))
        {
            mask |= 1 << CountOf(digit);
        }

        return mask;
    }

    private static int CountOf(char digit)
    {
        if (!DecimalDigits.TryIndexOf(digit, out var count) || count > MostNeighbors)
        {
            throw new InvalidOperationException(MalformedMessage);
        }

        return count;
    }
}
