using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubsequences;

// LeetCode 3428. Maximum and Minimum Sums of at Most Size K Subsequences: over
// every subsequence of length 1..k, sum its maximum plus its minimum - modulo
// 1e9+7.
//
// Sorting turns "which subsequences" into "how many": once nums is ascending,
// element i is the maximum of a subsequence of size <=k exactly when the other
// members are chosen from the i smaller elements before it, at most k-1 of them
// - sum_{j=0}^{min(k-1,i)} C(i,j) such subsequences. Symmetrically, i is the
// minimum when the rest come from the n-1-i larger elements after it. One
// "row sum capped at k-1" table, indexed both forwards (max) and backwards
// (min), answers both. Both strategies build that table; they differ only in
// how each C(i,j) is produced.
internal static class MaximumAndMinimumSumsOfAtMostSizeKSubsequencesSolution
{
    private const long Modulo = ModularArithmetic.Modulo;

    // The textbook answer: Pascal's triangle, one row at a time, keeping only the
    // k columns the length cap ever needs. No modular inverse, no repo
    // primitive - the baseline SumByFactorialCombinatorics is measured against.
    public static long SumByPascalTriangle(int[] nums, int k)
    {
        var sorted = SortedCopy(nums);
        var capped = CappedRowSumsByPascalTriangle(sorted.Length, k);

        return Combine(sorted, capped);
    }

    private static long[] CappedRowSumsByPascalTriangle(int n, int k)
    {
        var capped = new long[n];
        var row = new long[k];
        row[0] = 1;
        capped[0] = 1;

        for (var i = 1; i < n; i++)
        {
            var cap = Math.Min(i, k - 1);

            for (var j = cap; j >= 1; j--)
            {
                row[j] = (row[j] + row[j - 1]) % Modulo;
            }

            var sum = 0L;

            for (var j = 0; j <= cap; j++)
            {
                sum = (sum + row[j]) % Modulo;
            }

            capped[i] = sum;
        }

        return capped;
    }

    // This repo's ModularArithmetic.Inverse turns each row sum into a direct
    // C(i,j) lookup against a precomputed factorial/inverse-factorial table -
    // the same reverse-fill trick RoomWaysPrecomputedFactorialAlgebra uses, one
    // Inverse call total instead of one per row.
    public static long SumByFactorialCombinatorics(int[] nums, int k)
    {
        var sorted = SortedCopy(nums);
        var capped = CappedRowSumsByFactorial(sorted.Length, k);

        return Combine(sorted, capped);
    }

    private static long[] CappedRowSumsByFactorial(int n, int k)
    {
        var factorial = new long[n];
        var inverseFactorial = new long[n];
        factorial[0] = 1;

        for (var i = 1; i < n; i++)
        {
            factorial[i] = factorial[i - 1] * i % Modulo;
        }

        inverseFactorial[n - 1] = ModularArithmetic.Inverse(factorial[n - 1]);

        for (var i = n - 2; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % Modulo;
        }

        long NCr(int total, int choose) =>
            choose < 0 || choose > total
                ? 0
                : factorial[total] * inverseFactorial[choose] % Modulo * inverseFactorial[total - choose] % Modulo;

        var capped = new long[n];

        for (var i = 0; i < n; i++)
        {
            var cap = Math.Min(i, k - 1);
            var sum = 0L;

            for (var j = 0; j <= cap; j++)
            {
                sum = (sum + NCr(i, j)) % Modulo;
            }

            capped[i] = sum;
        }

        return capped;
    }

    private static int[] SortedCopy(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    // capped[i] counts subsequences of size <=k where sorted[i] is the maximum
    // (chosen from the i elements before it); capped[n-1-i] counts subsequences
    // where it is the minimum (chosen from the elements after it, by symmetry of
    // the same table read from the other end).
    private static long Combine(int[] sorted, long[] capped)
    {
        var n = sorted.Length;
        var total = 0L;

        for (var i = 0; i < n; i++)
        {
            var count = (capped[i] + capped[n - 1 - i]) % Modulo;
            total = (total + (long)sorted[i] * count) % Modulo;
        }

        return total;
    }
}
