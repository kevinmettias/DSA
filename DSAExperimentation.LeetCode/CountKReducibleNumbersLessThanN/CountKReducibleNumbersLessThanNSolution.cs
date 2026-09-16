using System.Numerics;

using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountKReducibleNumbersLessThanN;

// LeetCode 3352. Count K-Reducible Numbers Less Than N: s is n written in binary
// (length L, no leading zeros); x is k-reducible when repeatedly replacing it
// with its own popcount reaches 1 within k applications. Both strategies answer
// the same question with the same signature (TwoSumSolution precedent).
internal static class CountKReducibleNumbersLessThanNSolution
{
    // Textbook baseline: parse n and simulate the popcount chain for every x in
    // [1, n), counting how many settle to 1 within k steps. Correct, but the
    // range [1, n) is itself exponential in s.Length (n can be a 800-bit
    // number), so this only ever runs on small s - the arm the combinatorial
    // strategy below has to beat.
    public static int CountKReducibleNumbersByBruteForce(string s, int k)
    {
        var n = Convert.ToInt64(s, 2);
        var count = 0L;

        for (var x = 1L; x < n; x++)
        {
            if (StepsToOne(x) <= k)
            {
                count++;
            }
        }

        return (int)(count % ModularArithmetic.Modulo);
    }

    private static int StepsToOne(long x)
    {
        var steps = 0;

        while (x != 1)
        {
            x = BitOperations.PopCount((ulong)x);
            steps++;
        }

        return steps;
    }

    // Every x < n with the same popcount c takes the same number of operations
    // to reach 1 (one operation to turn x into c, plus however many the small
    // integer c itself needs - computed once per c up to L instead of once per
    // x), so this groups x < n by popcount instead of enumerating it: `steps`
    // folds the popcount chain bottom-up for every value up to L (BitOperations'
    // own popcount, the same BCL primitive the baseline uses), and
    // `CountNumbersByPopcount` is the classic binary digit-DP count - for each
    // '1' bit s sets, fix it to 0 and freely choose every lower bit, weighting
    // each choice of how many of those free bits are set by the binomial
    // coefficient C(remaining bits, ones needed) (Domain.Modular's shared
    // FactorialTable, the same table CountBalancedPermutationsSolution and
    // RoomWaysPrecomputedFactorialAlgebra read their coefficients from). k >= 1
    // always (LC's own constraint), so x = 1 itself
    // (0 operations needed) never needs separating from the rest of its
    // popcount-1 bucket (1 operation needed): both are <= k regardless, so a
    // uniform "1 + steps[c] <= k" test decides the whole bucket correctly.
    public static int CountKReducibleNumbersByPopcountCombinatorics(string s, int k)
    {
        var length = s.Length;
        var table = FactorialTable.Build(length);
        var steps = BuildReductionSteps(length);
        var countByPopcount = CountNumbersByPopcount(s, table);
        var answer = 0L;

        for (var c = 1; c <= length; c++)
        {
            if (1 + steps[c] <= k)
            {
                answer = (answer + countByPopcount[c]) % ModularArithmetic.Modulo;
            }
        }

        return (int)answer;
    }

    // steps[v] is how many popcount applications reduce the small integer v to
    // 1. popcount(v) < v for every v >= 2, so a single bottom-up pass already
    // sees each value's dependency before it is needed.
    private static int[] BuildReductionSteps(int maxValue)
    {
        var steps = new int[maxValue + 1];

        for (var v = 2; v <= maxValue; v++)
        {
            steps[v] = 1 + steps[BitOperations.PopCount((uint)v)];
        }

        return steps;
    }

    private static long[] CountNumbersByPopcount(string s, FactorialTable table)
    {
        var length = s.Length;
        var counts = new long[length + 1];
        var onesInPrefix = 0;

        for (var i = 0; i < length; i++)
        {
            if (s[i] == '1')
            {
                AccumulateSuffixChoices(counts, onesInPrefix, length - 1 - i, table);
                onesInPrefix++;
            }
        }

        return counts;
    }

    // At a '1' bit forced to 0, every combination of the `remainingBits` lower
    // bits is a valid number less than n with that many extra set bits.
    private static void AccumulateSuffixChoices(long[] counts, int onesInPrefix, int remainingBits, FactorialTable table)
    {
        for (var onesInSuffix = 0; onesInSuffix <= remainingBits; onesInSuffix++)
        {
            var popcount = onesInPrefix + onesInSuffix;
            var ways = table.Choose(remainingBits, onesInSuffix);
            counts[popcount] = (counts[popcount] + ways) % ModularArithmetic.Modulo;
        }
    }
}
