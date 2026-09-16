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
    // construction. Only tractable for the tiny sizes LeetCode's own examples use -
    // the arm the closed-form strategy has to beat.
    public static long CountGoodArraysByBruteForce(int arrayLength, int maxValue, int matchCount)
    {
        var arr = new int[arrayLength];
        return CountFrom(arr, (Index: 0, Matches: 0), maxValue, matchCount);
    }

    // Composed: one binomial coefficient (which n-1 gaps are equal) times one
    // modular power (every "different" gap's independent choice), both O(log n)
    // once Domain.Modular's FactorialTable is built - O(n) total against the
    // baseline's O(m^n).
    public static long CountGoodArraysByModularCombinatorics(int arrayLength, int maxValue, int matchCount)
    {
        var table = FactorialTable.Build(arrayLength);
        var waysToChooseEqualGaps = table.Choose(arrayLength - 1, matchCount);
        var waysToFillDifferentGaps = ModularArithmetic.Power(maxValue - 1, arrayLength - 1 - matchCount);

        return waysToChooseEqualGaps * maxValue % ModularArithmetic.Modulo
            * waysToFillDifferentGaps % ModularArithmetic.Modulo;
    }

    // The walk's own state: which position of the array it is deciding, and how many
    // equal adjacent gaps the filled prefix already has. The array's length is
    // `arrayLength`, so the walk reads the length off the array it is filling rather
    // than being told again.
    private static long CountFrom(int[] arr, (int Index, int Matches) state, int maxValue, int matchCount)
    {
        var (index, matches) = state;

        if (matches > matchCount)
        {
            return 0;
        }

        if (index == arr.Length)
        {
            return matches == matchCount ? 1 : 0;
        }

        var total = 0L;

        for (var value = 1; value <= maxValue; value++)
        {
            arr[index] = value;
            var nextMatches = MatchesAfterPlacing(arr, index, value, matches);
            total += CountFrom(arr, (Index: index + 1, Matches: nextMatches), maxValue, matchCount);
        }

        return total % ModularArithmetic.Modulo;
    }

    // How many equal adjacent gaps the filled prefix has once `value` sits at `index`:
    // one more than before when it repeats the cell to its left, and unchanged
    // otherwise - including at index 0, where there is no cell to its left.
    private static int MatchesAfterPlacing(int[] arr, int index, int value, int matches)
    {
        if (index > 0 && arr[index - 1] == value)
        {
            return matches + 1;
        }

        return matches;
    }
}
