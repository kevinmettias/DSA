using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.ManhattanDistancesOfAllArrangementsOfPieces;

// LeetCode 3426. Manhattan Distances of All Arrangements of Pieces: sum, over
// every way to place k identical pieces on an m x n grid (at most one piece per
// cell), of the Manhattan distance between every pair of placed pieces - modulo
// 1e9+7.
//
// Summing over arrangements first is the expensive order. Swap it: a fixed pair
// of cells contributes its distance once for every arrangement that occupies
// both of them, and there are C(mn-2, k-2) of those (the remaining k-2 pieces
// choosing freely among the other mn-2 cells). Rows and columns are independent,
// so the total pairwise-distance sum over every cell pair splits into a row term
// and a column term, each a closed form over 0..m-1 / 0..n-1.
internal static class ManhattanDistancesOfAllArrangementsOfPiecesSolution
{
    private const long Modulo = ModularArithmetic.Modulo;

    // The textbook answer: actually generate every k-cell arrangement and, for
    // each one, every pair inside it, and add up their Manhattan distances.
    // Combinatorially explosive - only tractable for small grids - which is
    // exactly the case the closed form below exists to avoid.
    public static long SumByBruteForceArrangements(int m, int n, int k)
    {
        var cellCount = m * n;
        var combination = new int[k];
        var total = 0L;

        void Search(int start, int depth)
        {
            if (depth == k)
            {
                total += ArrangementDistanceSum(combination, n);
                return;
            }

            for (var cell = start; cell < cellCount; cell++)
            {
                combination[depth] = cell;
                Search(cell + 1, depth + 1);
            }
        }

        Search(0, 0);
        return total % Modulo;
    }

    private static long ArrangementDistanceSum(int[] combination, int columns)
    {
        var sum = 0L;

        for (var i = 0; i < combination.Length; i++)
        {
            var (row1, col1) = Math.DivRem(combination[i], columns);

            for (var j = i + 1; j < combination.Length; j++)
            {
                var (row2, col2) = Math.DivRem(combination[j], columns);
                sum += Math.Abs(row1 - row2) + Math.Abs(col1 - col2);
            }
        }

        return sum;
    }

    // The closed form: every cell pair's distance, weighted by how many
    // k-arrangements contain both of its cells.
    public static long SumByPairwiseDistanceFormula(int m, int n, int k)
    {
        var cellCount = m * n;
        var pairDistanceSum = ((long)n * n % Modulo * PairwiseIndexSum(m) +
                                (long)m * m % Modulo * PairwiseIndexSum(n)) % Modulo;
        var arrangementsPerPair = BinomialCoefficient(cellCount - 2, k - 2);

        return pairDistanceSum * arrangementsPerPair % Modulo;
    }

    // Sum of (j - i) over every 0 <= i < j < length: the pairwise-index-distance
    // sum along one axis of the grid. Closed form length*(length^2-1)/6, division
    // done as multiplication by 6's modular inverse.
    private static long PairwiseIndexSum(long length)
    {
        var lengthMod = length % Modulo;
        var squareMinusOne = ((length * length - 1) % Modulo + Modulo) % Modulo;
        var sixInverse = ModularArithmetic.Inverse(6);

        return lengthMod * squareMinusOne % Modulo * sixInverse % Modulo;
    }

    private static long BinomialCoefficient(int n, int r)
    {
        if (r < 0 || r > n)
        {
            return 0;
        }

        var factorial = new long[n + 1];
        factorial[0] = 1;

        for (var i = 1; i <= n; i++)
        {
            factorial[i] = factorial[i - 1] * i % Modulo;
        }

        var inverseR = ModularArithmetic.Inverse(factorial[r]);
        var inverseNMinusR = ModularArithmetic.Inverse(factorial[n - r]);

        return factorial[n] * inverseR % Modulo * inverseNMinusR % Modulo;
    }
}
