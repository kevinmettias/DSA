using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountTheNumberOfArraysWithKMatchingAdjacentElements;

// LeetCode 3405. Count the Number of Arrays with K Matching Adjacent Elements:
// count length-n arrays with values in [1, m] that have exactly k indices i
// (1 <= i < n) where arr[i - 1] == arr[i], reported modulo 1e9+7.
//
// A good array is fully determined by: which k of the (n-1) adjacent gaps are
// "equal" gaps (C(n-1, k) choices), a value for the first element (m choices),
// and, independently for each of the remaining n-1-k "different" gaps, any of
// the (m-1) values unequal to whatever precedes it. So the count is
// C(n-1, k) * m * (m-1)^(n-1-k) - this repo's own Domain.Modular.ModularArithmetic
// supplies both the modulus and the Fermat's-little-theorem inverse a binomial
// coefficient needs, the same factorial/inverse-factorial table
// CountAnagramsByModularFactorial and RoomWaysPrecomputedFactorialAlgebra
// already build for other counting problems.
internal static class CountTheNumberOfArraysWithKMatchingAdjacentElementsSolution
{
    // Textbook baseline: build every one of the m^n arrays directly and count
    // adjacent matches on each, materializing nothing beyond the array under
    // construction. Only tractable for the tiny n/m LeetCode's own examples use -
    // the arm the closed-form strategy has to beat.
    public static long CountGoodArraysByBruteForce(int n, int m, int k)
    {
        var arr = new int[n];
        return CountFrom(arr, 0, 0, n, m, k);
    }

    private static long CountFrom(int[] arr, int index, int matches, int n, int m, int k)
    {
        if (matches > k)
        {
            return 0;
        }

        if (index == n)
        {
            return matches == k ? 1 : 0;
        }

        var total = 0L;

        for (var value = 1; value <= m; value++)
        {
            arr[index] = value;
            var nextMatches = index > 0 && arr[index - 1] == value ? matches + 1 : matches;
            total += CountFrom(arr, index + 1, nextMatches, n, m, k);
        }

        return total % ModularArithmetic.Modulo;
    }

    // Composed: one binomial coefficient (which n-1 gaps are equal) times one
    // modular power (every "different" gap's independent choice), both O(log n)
    // once the O(n) factorial/inverse-factorial table is built - O(n) total
    // against the baseline's O(m^n).
    public static long CountGoodArraysByModularCombinatorics(int n, int m, int k)
    {
        var (factorial, inverseFactorial) = BuildFactorialTable(n);
        var waysToChooseEqualGaps = BinomialCoefficient(n - 1, k, factorial, inverseFactorial);
        var waysToFillDifferentGaps = ModularArithmetic.Power(m - 1, n - 1 - k);

        return waysToChooseEqualGaps * m % ModularArithmetic.Modulo * waysToFillDifferentGaps % ModularArithmetic.Modulo;
    }

    private static long BinomialCoefficient(int n, int r, long[] factorial, long[] inverseFactorial)
    {
        if (r < 0 || r > n)
        {
            return 0;
        }

        return factorial[n] * inverseFactorial[r] % ModularArithmetic.Modulo * inverseFactorial[n - r] % ModularArithmetic.Modulo;
    }

    private static (long[] Factorial, long[] InverseFactorial) BuildFactorialTable(int maxSize)
    {
        var factorial = new long[maxSize + 1];
        var inverseFactorial = new long[maxSize + 1];
        factorial[0] = 1;

        for (var i = 1; i <= maxSize; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        inverseFactorial[maxSize] = ModularArithmetic.Inverse(factorial[maxSize]);

        for (var i = maxSize - 1; i >= 0; i--)
        {
            inverseFactorial[i] = inverseFactorial[i + 1] * (i + 1) % ModularArithmetic.Modulo;
        }

        return (factorial, inverseFactorial);
    }
}
