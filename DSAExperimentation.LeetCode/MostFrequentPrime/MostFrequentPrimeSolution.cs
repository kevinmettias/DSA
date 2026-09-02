using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MostFrequentPrime;

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
internal static class MostFrequentPrimeSolution
{
    // A generated number only qualifies once it exceeds this LeetCode-defined
    // threshold - single- and some two-digit numbers never count, even if prime.
    private const int MinimumQualifyingValue = 10;

    private static readonly (int RowDelta, int ColDelta)[] Directions =
    [
        (-1, 0), (1, 0), (0, -1), (0, 1),
        (-1, -1), (-1, 1), (1, -1), (1, 1),
    ];

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

    // This repo's own Sieve of Eratosthenes, run once over a DynamicArray<bool>
    // composite tracker sized to the largest value the grid can possibly
    // produce (10^max(rows, cols) - 1) - the same composition
    // ClosestPrimeNumbersInRangeTests already builds for LC 2523, reused here
    // so every candidate is a single O(1) array lookup instead of a fresh
    // trial-division walk.
    public static int MostFrequentPrimeBySieve(int[][] mat)
    {
        var isComposite = BuildSieve(MaxPossibleValue(mat));
        var frequency = new Dictionary<int, int>();

        foreach (var value in GenerateCandidateNumbers(mat))
        {
            if (!isComposite.Get(value))
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
            bound *= 10;
        }

        return bound - 1;
    }

    private static DynamicArray<bool> BuildSieve(int bound)
    {
        var isComposite = new DynamicArray<bool>();

        for (var i = 0; i <= bound; i++)
        {
            isComposite.Add(i < 2);
        }

        for (var i = 2; (long)i * i <= bound; i++)
        {
            if (isComposite.Get(i))
            {
                continue;
            }

            for (var multiple = i * i; multiple <= bound; multiple += i)
            {
                isComposite.Set(multiple, true);
            }
        }

        return isComposite;
    }

    // Every straight-line path of length >= 2 from every cell, in every
    // direction, yielded at each step it becomes a qualifying candidate.
    // Digits are 1-9 (LC's own constraint - no leading-zero cells exist), so
    // the running value only ever grows and crosses MinimumQualifyingValue
    // exactly once per path.
    private static IEnumerable<int> GenerateCandidateNumbers(int[][] mat)
    {
        var rows = mat.Length;
        var cols = mat[0].Length;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                foreach (var (rowDelta, colDelta) in Directions)
                {
                    var value = 0;
                    var r = row;
                    var c = col;

                    while (r >= 0 && r < rows && c >= 0 && c < cols)
                    {
                        value = (value * 10) + mat[r][c];

                        if (value > MinimumQualifyingValue)
                        {
                            yield return value;
                        }

                        r += rowDelta;
                        c += colDelta;
                    }
                }
            }
        }
    }

    private static int MostFrequent(Dictionary<int, int> frequency)
    {
        var best = LeetCodeAnswer.None;
        var bestCount = 0;

        foreach (var (value, count) in frequency)
        {
            if (count > bestCount || (count == bestCount && value > best))
            {
                best = value;
                bestCount = count;
            }
        }

        return best;
    }
}
