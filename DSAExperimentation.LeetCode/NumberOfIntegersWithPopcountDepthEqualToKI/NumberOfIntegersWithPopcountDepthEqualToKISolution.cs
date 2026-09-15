using System.Numerics;

namespace DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKI;

// LeetCode 3621. Number of Integers With Popcount-Depth Equal to K I:
// repeatedly replacing x with its own popcount eventually reaches 1; the
// popcount-depth of x is how many replacements that takes. Count x in [1, n]
// whose popcount-depth is exactly k. Both strategies answer the same question
// with the same signature (TwoSumSolution precedent).
internal static class NumberOfIntegersWithPopcountDepthEqualToKISolution
{
    // Textbook baseline: simulate every x in [1, n] directly. Correct, but O(n)
    // - unusable at the real problem's n up to 10^15 - the arm the
    // combinatorial strategy below has to beat.
    public static long PopcountDepthByBruteForce(long n, int k)
    {
        var count = 0L;

        for (var x = 1L; x <= n; x++)
        {
            if (Depth(x) == k)
            {
                count++;
            }
        }

        return count;
    }

    private static int Depth(long x)
    {
        var depth = 0;

        while (x != 1)
        {
            x = BitOperations.PopCount((ulong)x);
            depth++;
        }

        return depth;
    }

    // popcount(x) is at most ~50 for x <= 10^15, so for x != 1, depth(x) is
    // 1 + depth(popcount(x)) where popcount(x) is itself always small - the
    // same "group by popcount, solve the small recursion once per popcount
    // value instead of once per x" move
    // CountKReducibleNumbersLessThanNSolution's own combinatorial arm makes
    // for its popcount-chain problem. x = 1 is the sole depth-0 value and is
    // handled separately: the recursive formula would read
    // 1 + depth(popcount(1)) = 1 + depth(1) = 1, which is wrong for x = 1
    // itself (its true depth is 0 by definition), so it is excluded from
    // whichever popcount-1 bucket the formula would otherwise sweep it into.
    public static long PopcountDepthByPopcountCombinatorics(long n, int k)
    {
        if (k == 0)
        {
            return 1; // Only x = 1; LC's own constraint keeps n >= 1.
        }

        var binary = Convert.ToString(n, 2);
        var length = binary.Length;
        var depthByPopcount = BuildDepthByPopcount(length);
        var countByPopcount = CountNumbersByPopcount(binary);
        var targetDepth = k - 1;
        var total = 0L;

        for (var c = 1; c <= length; c++)
        {
            total += CountAtPopcount(countByPopcount, depthByPopcount, c, targetDepth);
        }

        return total;
    }

    // depth[v] is how many popcount applications reduce the small integer v to
    // 1. popcount(v) < v for every v >= 2, so a single bottom-up pass already
    // sees each value's dependency before it is needed (the same shape
    // CountKReducibleNumbersLessThanNSolution's BuildReductionSteps uses).
    private static int[] BuildDepthByPopcount(int maxValue)
    {
        var depth = new int[maxValue + 1];

        for (var v = 2; v <= maxValue; v++)
        {
            depth[v] = 1 + depth[BitOperations.PopCount((uint)v)];
        }

        return depth;
    }

    // Classic binary digit-DP: for every '1' bit of n, fix it to 0 and freely
    // choose every lower bit, weighting each choice of how many of those free
    // bits are set by the binomial coefficient C(remaining bits, ones needed)
    // (Pascal's triangle - no modulus is involved here, unlike
    // CountKReducibleNumbersLessThanNSolution's factorial/inverse-factorial
    // table, since n <= 10^15 keeps every count well within a long). n itself
    // is then folded in as the one value the "fix a 1-bit to 0" sweep never
    // visits - its popcount is exactly the number of 1 bits the sweep counted.
    private static long[] CountNumbersByPopcount(string binary)
    {
        var length = binary.Length;
        var pascal = BuildPascalsTriangle(length);
        var counts = new long[length + 1];
        var onesInPrefix = 0;

        for (var i = 0; i < length; i++)
        {
            if (binary[i] == '1')
            {
                var remainingBits = length - 1 - i;
                AddFreeSuffixCounts(pascal[remainingBits], onesInPrefix, counts);
                onesInPrefix++;
            }
        }

        counts[onesInPrefix]++;

        return counts;
    }

    private static long[][] BuildPascalsTriangle(int size)
    {
        var pascal = new long[size + 1][];

        for (var row = 0; row <= size; row++)
        {
            pascal[row] = new long[row + 1];
            pascal[row][0] = 1;
            pascal[row][row] = 1;

            for (var col = 1; col < row; col++)
            {
                pascal[row][col] = pascal[row - 1][col - 1] + pascal[row - 1][col];
            }
        }

        return pascal;
    }

    // One fixed 1-bit turned to 0: every way the remaining bits below it can be
    // chosen, C(remainingBits, onesInSuffix), lands at baseIndex + onesInSuffix.
    private static void AddFreeSuffixCounts(long[] row, int baseIndex, long[] counts)
    {
        for (var onesInSuffix = 0; onesInSuffix < row.Length; onesInSuffix++)
        {
            counts[baseIndex + onesInSuffix] += row[onesInSuffix];
        }
    }

    // How many x <= n have exactly this popcount, given that this popcount's own
    // depth is the target one - or 0 when it is not. The digit-DP count is
    // reduced by one for x = 1 itself, the sole depth-0 value, whenever the
    // popcount-1 bucket would otherwise sweep it in.
    private static long CountAtPopcount(
        long[] countByPopcount, int[] depthByPopcount, int popcount, int targetDepth)
    {
        if (depthByPopcount[popcount] != targetDepth)
        {
            return 0;
        }

        var count = countByPopcount[popcount];

        if (popcount == 1 && targetDepth == 0)
        {
            count -= 1; // Exclude x = 1 itself - see the caller's comment.
        }

        return count;
    }
}
