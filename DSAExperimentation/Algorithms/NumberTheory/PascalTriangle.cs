namespace DSAExperimentation.Algorithms.NumberTheory;

// The rows of Pascal's triangle up to a chosen row, built once by C(n, k) = C(n-1, k-1) + C(n-1, k), so
// every Choose afterwards is an O(1) read and a caller that wants a whole row reads it as a span.
// Exact keeps the coefficients themselves, which long holds through row 66 (C(66, 33) is about 7.2e18;
// C(67, 33) is past long.MaxValue). Modulo keeps them reduced by any modulus the caller names -
// composite ones included, which is the case FactorialTable cannot serve: it divides by factorials
// through modular inverses, and mod 10 or mod 4 has no inverse for most of them, while the addition
// recurrence never divides.
//
// It sits in Algorithms/NumberTheory beside ModularPower and FactorialTable, not at the LeetCode
// tier, because the modulus here is the caller's, not a convention this type fixes (§17.6):
// FactorialTable is the prime-modulus form of a binomial table, this is the general one. The two
// modes are one type because they are one recurrence: an exact triangle is the triangle reduced by
// long.MaxValue, which every coefficient through row 66 stays below, so reducing by it changes
// nothing. Every query afterwards is the same read.
//
// Complexity law: O(maxRow^2) time and memory to build. A caller that needs one large row, or C(n, k)
// for n far past what a table can hold, wants a rolling row or Lucas's theorem instead.
/// <summary>
/// Binomial coefficients C(n, k) for every row up to a chosen one, exact or reduced by a caller's modulus.
/// </summary>
public sealed class PascalTriangle
{
    public const int MaxExactRow = 66;

    // Two residues below the modulus must add without leaving long.
    private const long MaxModulus = 1L << 62;

    private const string RowOutOfRangeMessage = "The row is past the last row this triangle was built with.";

    private readonly long[][] _rows;

    /// <summary>The last row this triangle holds, which bounds the rows Choose and Row can read.</summary>
    public int MaxRow => _rows.Length - 1;

    private PascalTriangle(long[][] rows) => _rows = rows;

    /// <summary>Builds a triangle of exact coefficients, which fit in a long through row 66.</summary>
    /// <param name="maxRow">The last row to build, from 0 through <see cref="MaxExactRow"/>.</param>
    /// <returns>A triangle whose entries are the coefficients themselves.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxRow"/> is negative or greater than <see cref="MaxExactRow"/>.
    /// </exception>
    public static PascalTriangle Exact(int maxRow)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxRow);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(maxRow, MaxExactRow);

        return new PascalTriangle(Build(maxRow, long.MaxValue));
    }

    /// <summary>Builds a triangle of coefficients reduced by a modulus, composite moduli included.</summary>
    /// <param name="maxRow">The last row to build, from 0 up.</param>
    /// <param name="modulus">The modulus every entry is reduced by, from 1 through 2^62.</param>
    /// <returns>A triangle whose entries are the coefficients modulo <paramref name="modulus"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="maxRow"/> is negative, or <paramref name="modulus"/> is below 1 or above 2^62.
    /// </exception>
    public static PascalTriangle Modulo(int maxRow, long modulus)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maxRow);
        ArgumentOutOfRangeException.ThrowIfLessThan(modulus, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(modulus, MaxModulus);

        return new PascalTriangle(Build(maxRow, modulus));
    }

    // C(n, k), or 0 when k is outside [0, n] - a subset that cannot exist, as FactorialTable.Choose
    // answers it. A row the triangle was not built to is the caller's sizing mistake and throws.
    /// <summary>Reads C(n, k), the number of ways to choose k items from n.</summary>
    /// <param name="totalCount">The row n, the number of items chosen from.</param>
    /// <param name="chosenCount">The column k, the number of items chosen.</param>
    /// <returns>
    /// C(n, k), reduced by the modulus for a triangle built by <see cref="Modulo"/>; 0 when <paramref
    /// name="chosenCount"/> is outside [0, <paramref name="totalCount"/>].
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="totalCount"/> is past <see cref="MaxRow"/> and <paramref name="chosenCount"/> is
    /// within [0, <paramref name="totalCount"/>].
    /// </exception>
    public long Choose(int totalCount, int chosenCount)
    {
        if (IsImpossibleChoice(totalCount, chosenCount))
        {
            return 0;
        }

        return RowOf(totalCount)[chosenCount];
    }

    // C(n, k) counts subsets of k taken from n, so a negative count, or a k past n, names a subset
    // that cannot exist.
    private static bool IsImpossibleChoice(int totalCount, int chosenCount) =>
        totalCount < 0 || chosenCount < 0 || chosenCount > totalCount;

    /// <summary>Reads a whole row of the triangle, C(n, 0) through C(n, n).</summary>
    /// <param name="totalCount">The row n to read.</param>
    /// <returns>
    /// The row's n + 1 coefficients in order of k, as a read-only view of the triangle's own storage.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="totalCount"/> is negative or past <see cref="MaxRow"/>.
    /// </exception>
    public ReadOnlySpan<long> Row(int totalCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(totalCount);

        return RowOf(totalCount);
    }

    private long[] RowOf(int totalCount)
    {
        if (totalCount > MaxRow)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCount), RowOutOfRangeMessage);
        }

        return _rows[totalCount];
    }

    // Each entry is the sum of the two above it, reduced by the modulus; the two edges are 1, reduced
    // too, so a modulus of 1 gives a triangle of zeros as it should.
    private static long[][] Build(int maxRow, long modulus)
    {
        var rows = new long[maxRow + 1][];
        var one = 1 % modulus;

        for (var row = 0; row <= maxRow; row++)
        {
            rows[row] = new long[row + 1];
            rows[row][0] = one;
            rows[row][row] = one;

            for (var column = 1; column < row; column++)
            {
                rows[row][column] = ReducedSum(rows[row - 1][column - 1], rows[row - 1][column], modulus);
            }
        }

        return rows;
    }

    // Both terms are already residues, so their sum is below twice the modulus - which MaxModulus keeps
    // inside long - and one subtraction reduces it where a % would divide. That division, a 64-bit one
    // per entry, made an exact triangle cost twice what plain addition does, and the exact triangle
    // never needs it: no sum through row 66 reaches long.MaxValue.
    private static long ReducedSum(long left, long right, long modulus)
    {
        var sum = left + right;

        if (sum >= modulus)
        {
            sum -= modulus;
        }

        return sum;
    }
}
