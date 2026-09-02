using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountNumberOfBalancedPermutations;

// LeetCode 3343. Count Number of Balanced Permutations: a permutation of num's
// digits is balanced when the digits at even indices sum to the same total as
// the digits at odd indices. Both strategies answer the same question with the
// same signature (TwoSumSolution precedent) so the harnesses can hold them to
// one contract.
internal static class CountNumberOfBalancedPermutationsSolution
{
    private const int DigitCount = 10;

    // Textbook baseline: every distinct permutation of num's characters, deduped
    // via HashSet<string> (CountAnagrams precedent - repeated digits would
    // otherwise recount the same string), each checked directly against the
    // even/odd sum rule. Correct, but factorial-time in num.Length - the arm the
    // digit-count DP below has to beat.
    public static long CountBalancedPermutationsByBruteForce(string num)
    {
        var distinct = new HashSet<string>();
        Permute(num.ToCharArray(), 0, distinct);

        var balanced = 0L;

        foreach (var permutation in distinct)
        {
            if (IsBalanced(permutation))
            {
                balanced++;
            }
        }

        return balanced % ModularArithmetic.Modulo;
    }

    private static void Permute(char[] digits, int start, HashSet<string> distinct)
    {
        if (start == digits.Length)
        {
            distinct.Add(new string(digits));
            return;
        }

        for (var i = start; i < digits.Length; i++)
        {
            (digits[start], digits[i]) = (digits[i], digits[start]);
            Permute(digits, start + 1, distinct);
            (digits[start], digits[i]) = (digits[i], digits[start]);
        }
    }

    private static bool IsBalanced(string permutation)
    {
        var evenSum = 0;
        var oddSum = 0;

        for (var i = 0; i < permutation.Length; i++)
        {
            var digit = permutation[i] - '0';

            if (i % 2 == 0)
            {
                evenSum += digit;
            }
            else
            {
                oddSum += digit;
            }
        }

        return evenSum == oddSum;
    }

    // A balanced permutation is exactly a way to split num's multiset of digits
    // into an even-index group of size ceil(n/2) and an odd-index group of size
    // floor(n/2) with equal sums, each group then interleaved among itself in any
    // of that group's own factorial-many orders. dp[evenUsed][sumUsed] folds the
    // ten digit values one at a time, weighting each split of a digit's own
    // copies between the two groups by 1/(k! * (count-k)!) - the same
    // inverse-factorial-weighted fold RoomWaysPrecomputedFactorialAlgebra and
    // CountAnagrams' modular-factorial arm both use (Domain.Modular's Fermat's-
    // little-theorem inverse) - so the running total only needs multiplying by
    // evenCount! * oddCount! once at the end instead of renormalizing after
    // every digit.
    public static long CountBalancedPermutationsByDigitCountDp(string num)
    {
        var counts = new int[DigitCount];
        var total = 0;

        foreach (var c in num)
        {
            var digit = c - '0';
            counts[digit]++;
            total += digit;
        }

        if (total % 2 != 0)
        {
            return 0;
        }

        var half = total / 2;
        var evenSlots = (num.Length + 1) / 2;
        var oddSlots = num.Length / 2;
        var (factorial, inverseFactorial) = BuildFactorialTable(num.Length);
        var ways = FoldDigitSplits(counts, evenSlots, half, inverseFactorial);

        return ways * factorial[evenSlots] % ModularArithmetic.Modulo * factorial[oddSlots] % ModularArithmetic.Modulo;
    }

    private static long FoldDigitSplits(int[] counts, int evenSlots, int half, long[] inverseFactorial)
    {
        var dp = new long[evenSlots + 1, half + 1];
        dp[0, 0] = 1;

        for (var digit = 0; digit < counts.Length; digit++)
        {
            dp = FoldDigit(dp, digit, counts[digit], evenSlots, half, inverseFactorial);
        }

        return dp[evenSlots, half];
    }

    private static long[,] FoldDigit(long[,] dp, int digit, int count, int evenSlots, int half, long[] inverseFactorial)
    {
        var next = new long[evenSlots + 1, half + 1];

        for (var used = 0; used <= evenSlots; used++)
        {
            for (var sum = 0; sum <= half; sum++)
            {
                if (dp[used, sum] != 0)
                {
                    AccumulateDigitSplits(next, dp[used, sum], digit, count, used, sum, evenSlots, half, inverseFactorial);
                }
            }
        }

        return next;
    }

    private static void AccumulateDigitSplits(
        long[,] next, long ways, int digit, int count, int used, int sum, int evenSlots, int half, long[] inverseFactorial)
    {
        for (var k = 0; k <= count; k++)
        {
            var nextUsed = used + k;
            var nextSum = sum + k * digit;

            if (nextUsed > evenSlots || nextSum > half)
            {
                break;
            }

            var contribution = ways * inverseFactorial[k] % ModularArithmetic.Modulo *
                inverseFactorial[count - k] % ModularArithmetic.Modulo;
            next[nextUsed, nextSum] = (next[nextUsed, nextSum] + contribution) % ModularArithmetic.Modulo;
        }
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
