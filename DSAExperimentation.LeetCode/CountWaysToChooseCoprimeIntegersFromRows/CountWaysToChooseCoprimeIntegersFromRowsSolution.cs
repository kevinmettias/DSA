using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountWaysToChooseCoprimeIntegersFromRows;

// LeetCode 3725. Count Ways to Choose Coprime Integers from Rows: choose exactly
// one integer from each row of an m x n matrix (values 1..150) so the GCD of every
// chosen integer is 1; return the number of ways to do that, modulo 1e9+7.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class CountWaysToChooseCoprimeIntegersFromRowsSolution
{
    // Textbook DFS: pick one value per row, thread the running GCD of every pick
    // made so far, and count the leaves where that running GCD is 1. Correct at
    // any matrix size, just exponential in the row count (n^m) - exactly the arm
    // the row-by-row GCD-counting DP below has to beat.
    public static long CountWaysByBruteForce(int[][] mat) => CountFromRow(mat, rowIndex: 0, runningGcd: 0);

    // Composed: every GCD reachable after any prefix of rows is itself a divisor
    // of some mat[i][j] <= 150, so there are never more than 150 distinct "running
    // GCD so far" states to track - a Dictionary<int,long> from that GCD to the
    // number of ways to reach it stands in for the whole n^m combination space.
    // Each row folds every current state against every value in that row
    // (at most 150 states * 150 values = 22,500 transitions per row, <=150 rows),
    // which is polynomial where the brute-force DFS above is exponential.
    public static long CountWaysByGcdCountingDp(int[][] mat)
    {
        var waysByGcd = new Dictionary<int, long>();

        foreach (var value in mat[0])
        {
            waysByGcd[value] = (waysByGcd.GetValueOrDefault(value) + 1) % ModularArithmetic.Modulo;
        }

        for (var row = 1; row < mat.Length; row++)
        {
            waysByGcd = FoldRow(waysByGcd, mat[row]);
        }

        return waysByGcd.GetValueOrDefault(1);
    }

    private static Dictionary<int, long> FoldRow(Dictionary<int, long> waysByGcd, int[] row)
    {
        var next = new Dictionary<int, long>();

        foreach (var (gcdSoFar, ways) in waysByGcd)
        {
            foreach (var value in row)
            {
                var nextGcd = Gcd(gcdSoFar, value);
                next[nextGcd] = (next.GetValueOrDefault(nextGcd) + ways) % ModularArithmetic.Modulo;
            }
        }

        return next;
    }

    private static long CountFromRow(int[][] mat, int rowIndex, int runningGcd)
    {
        if (rowIndex == mat.Length)
        {
            return runningGcd == 1 ? 1 : 0;
        }

        var ways = 0L;

        foreach (var value in mat[rowIndex])
        {
            var gcdSoFar = Gcd(runningGcd, value);
            ways += CountFromRow(mat, rowIndex + 1, gcdSoFar);
        }

        return ways % ModularArithmetic.Modulo;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
