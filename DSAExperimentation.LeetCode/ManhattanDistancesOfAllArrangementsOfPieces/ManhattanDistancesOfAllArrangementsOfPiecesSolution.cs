using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.LeetCode.ManhattanDistancesOfAllArrangementsOfPieces;

// LeetCode 3426. Manhattan Distances of All Arrangements of Pieces: sum, over
// every way to place pieceCount identical pieces on a rowCount x columnCount
// grid (at most one piece per cell), of the Manhattan distance between every
// pair of placed pieces - modulo 1e9+7.
//
// Summing over arrangements first is the expensive order. Swap it: a fixed pair
// of cells contributes its distance once for every arrangement that occupies
// both of them, and there are C(cellCount-2, pieceCount-2) of those (the
// remaining pieceCount-2 pieces choosing freely among the other cellCount-2
// cells). Rows and columns are independent, so the total pairwise-distance sum
// over every cell pair splits into a row term and a column term, each a closed
// form over 0..rowCount-1 / 0..columnCount-1.
internal static class ManhattanDistancesOfAllArrangementsOfPiecesSolution
{
    // The textbook answer: actually generate every pieceCount-cell arrangement
    // and, for each one, every pair inside it, and add up their Manhattan
    // distances. Combinatorially explosive - only tractable for small grids -
    // which is exactly the case the closed form below exists to avoid.
    public static long SumByBruteForceArrangements(int rowCount, int columnCount, int pieceCount)
    {
        var cellCount = rowCount * columnCount;
        var combination = new int[pieceCount];
        var total = 0L;

        void SearchArrangements(int start, int depth)
        {
            if (depth == pieceCount)
            {
                total += ArrangementDistanceSum(combination, columnCount);
                return;
            }

            for (var cell = start; cell < cellCount; cell++)
            {
                combination[depth] = cell;
                SearchArrangements(cell + 1, depth + 1);
            }
        }

        SearchArrangements(0, 0);
        return total % LeetCodeModulus.Value;
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
    // pieceCount-arrangements contain both of its cells.
    public static long SumByPairwiseDistanceFormula(int rowCount, int columnCount, int pieceCount)
    {
        var cellCount = rowCount * columnCount;
        var pairDistanceSum = ((long)columnCount * columnCount % LeetCodeModulus.Value * PairwiseIndexSum(rowCount) +
                                (long)rowCount * rowCount % LeetCodeModulus.Value * PairwiseIndexSum(columnCount)) % LeetCodeModulus.Value;
        var arrangementsPerPair = BinomialCoefficient(cellCount - 2, pieceCount - 2);

        return pairDistanceSum * arrangementsPerPair % LeetCodeModulus.Value;
    }

    // Sum of (j - i) over every 0 <= i < j < length: the pairwise-index-distance
    // sum along one axis of the grid. Closed form length*(length^2-1)/6, division
    // done as multiplication by 6's modular inverse.
    private static long PairwiseIndexSum(long length)
    {
        var lengthMod = length % LeetCodeModulus.Value;
        var squareMinusOne = ((length * length - 1) % LeetCodeModulus.Value + LeetCodeModulus.Value) % LeetCodeModulus.Value;
        var sixInverse = ModularInverse.ByFermat<LeetCodeModulus>(6);

        return lengthMod * squareMinusOne % LeetCodeModulus.Value * sixInverse % LeetCodeModulus.Value;
    }

    private static long BinomialCoefficient(int totalCount, int selectedCount)
    {
        if (selectedCount < 0 || selectedCount > totalCount)
        {
            return 0;
        }

        var factorial = new long[totalCount + 1];
        factorial[0] = 1;

        for (var i = 1; i <= totalCount; i++)
        {
            factorial[i] = factorial[i - 1] * i % LeetCodeModulus.Value;
        }

        var inverseSelected = ModularInverse.ByFermat<LeetCodeModulus>(factorial[selectedCount]);
        var inverseUnselected = ModularInverse.ByFermat<LeetCodeModulus>(factorial[totalCount - selectedCount]);

        return factorial[totalCount] * inverseSelected % LeetCodeModulus.Value * inverseUnselected % LeetCodeModulus.Value;
    }
}
