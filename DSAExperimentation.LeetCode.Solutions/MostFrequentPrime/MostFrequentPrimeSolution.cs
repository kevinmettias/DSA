using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.Lattices;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.LeetCode.Solutions.MostFrequentPrime;

// LeetCode 3044. Most Frequent Prime: from every cell, walk each of the 8
// directions in a straight line, generating a number at every step by
// concatenating the digits seen so far. Return the most frequent prime
// number greater than 10 across every generated number, largest wins ties,
// -1 if none qualify.
//
// Both strategies share the same directional walk (GenerateCandidateNumbers)
// and only differ in how they test primality - the two strategies exist to
// contrast that one choice, the same way AddBinarySolution's two strategies
// share DigitWalk and only differ in how they undo the digit reversal.
[LeetCodeQuestion("most-frequent-prime", LeetCodeAnswerOrder.AsWritten, Reach = typeof(MostFrequentPrimeReach))]
internal static class MostFrequentPrimeSolution
{
    // A generated number only qualifies once it exceeds this LeetCode-defined
    // threshold - single- and some two-digit numbers never count, even if prime.
    private const int MinimumQualifyingValue = 10;

    // Textbook baseline: test every candidate with O(sqrt(value)) trial
    // division. The arm the sieve strategy below has to beat.
    public static int MostFrequentPrimeByTrialDivision(int[][] mat)
    {
        var frequency = new Dictionary<int, int>();

        foreach (var value in GenerateCandidateNumbers(mat))
        {
            if (IsPrimeByTrialDivision(value))
            {
                frequency[value] = frequency.GetValueOrDefault(value) + 1;
            }
        }

        return MostFrequent(frequency);
    }

    private static bool IsPrimeByTrialDivision(int value)
    {
        for (var divisor = 2; (long)divisor * divisor <= value; divisor++)
        {
            if (value % divisor == 0)
            {
                return false;
            }
        }

        return true;
    }

    // This repo's own Sieve of Eratosthenes, run once through the shared
    // PrimeSieve over a tracker sized to the largest value the grid can
    // possibly produce (10^max(rows, cols) - 1), so every candidate is a single
    // O(1) array lookup instead of a fresh trial-division walk. The bound is
    // this problem's own; the construction is not, so only the bound is written
    // here.
    public static int MostFrequentPrimeBySieve(int[][] mat)
    {
        var isComposite = PrimeSieve.BuildCompositeTracker(MaxPossibleValue(mat));
        var frequency = new Dictionary<int, int>();

        foreach (var value in GenerateCandidateNumbers(mat))
        {
            if (!isComposite[value])
            {
                frequency[value] = frequency.GetValueOrDefault(value) + 1;
            }
        }

        return MostFrequent(frequency);
    }

    private static int MaxPossibleValue(int[][] mat)
    {
        var maxLength = Math.Max(mat.Length, mat[0].Length);
        var bound = 1;

        for (var i = 0; i < maxLength; i++)
        {
            bound *= DecimalDigits.Size;
        }

        return bound - 1;
    }

    // Every straight-line path of length >= 2 from every cell, in every
    // direction, yielded at each step it becomes a qualifying candidate.
    // Digits are 1-9 (LC's own constraint - no leading-zero cells exist), so
    // the running value only ever grows and crosses MinimumQualifyingValue
    // exactly once per path.
    // Both coordinates have to land inside the grid for the walk to keep reading a digit.
    private static bool IsInsideGrid(int row, int col, int rows, int cols) =>
        row >= 0 && row < rows && col >= 0 && col < cols;

    private static IEnumerable<int> GenerateCandidateNumbers(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                foreach (var value in WalkAllDirectionsFrom(mat, row, col))
                {
                    yield return value;
                }
            }
        }
    }

    // Every direction's straight-line walk out of one cell, in GridDirections.Octilinear's
    // order. The answer counts the numbers walked, so the order does not change it.
    private static IEnumerable<int> WalkAllDirectionsFrom(int[][] mat, int row, int col)
    {
        for (var line = 0; line < GridDirections.Octilinear.Length; line++)
        {
            var direction = GridDirections.Octilinear[line];
            foreach (var value in WalkOneDirection(mat, (row, col), direction))
            {
                yield return value;
            }
        }
    }

    // One straight-line walk: keep reading a digit while the cell stays on the grid,
    // yielding the running number as soon as it passes MinimumQualifyingValue.
    private static IEnumerable<int> WalkOneDirection(
        int[][] mat, (int Row, int Col) origin, GridStep direction)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;
        var (row, col) = origin;
        var value = 0;

        while (IsInsideGrid(row, col, rows, cols))
        {
            value = (value * DecimalDigits.Size) + mat[row][col];

            if (value > MinimumQualifyingValue)
            {
                yield return value;
            }

            row += direction.DeltaRow;
            col += direction.DeltaCol;
        }
    }

    private static int MostFrequent(Dictionary<int, int> frequency)
    {
        var best = LeetCodeAnswer.None;
        var bestCount = 0;

        foreach (var (value, count) in frequency)
        {
            if (IsMoreFrequent(count, bestCount, value, best))
            {
                best = value;
                bestCount = count;
            }
        }

        return best;
    }

    // A number takes the lead when it has been seen more often than the current best, or
    // matches its count and is the larger number - LeetCode's own tie-break.
    private static bool IsMoreFrequent(int count, int bestCount, int value, int best) =>
        count > bestCount || (count == bestCount && value > best);
}
