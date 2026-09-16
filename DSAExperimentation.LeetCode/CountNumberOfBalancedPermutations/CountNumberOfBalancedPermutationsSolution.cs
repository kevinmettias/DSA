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
        var table = FactorialTable.Build(num.Length);
        var ways = FoldDigitSplits(counts, evenSlots, half, table);

        return ways * table.Factorial(evenSlots) % ModularArithmetic.Modulo
            * table.Factorial(oddSlots) % ModularArithmetic.Modulo;
    }

    private static long FoldDigitSplits(int[] counts, int evenSlots, int half, FactorialTable table)
    {
        var dp = new long[evenSlots + 1, half + 1];
        dp[0, 0] = 1;

        for (var digit = 0; digit < counts.Length; digit++)
        {
            dp = FoldDigit(dp, digit, counts[digit], table);
        }

        return dp[evenSlots, half];
    }

    // Every table the fold builds is (evenSlots + 1) x (half + 1), so both bounds are
    // the dimensions of the table it is handed - and one digit's value and its copy
    // count are a single fact about that digit rather than two.
    private static long[,] FoldDigit(long[,] dp, int digit, int count, FactorialTable table)
    {
        var evenSlots = dp.GetLength(0) - 1;
        var half = dp.GetLength(1) - 1;
        var next = new long[evenSlots + 1, half + 1];

        for (var used = 0; used <= evenSlots; used++)
        {
            for (var sum = 0; sum <= half; sum++)
            {
                if (dp[used, sum] != 0)
                {
                    var cell = (Used: used, Sum: sum, Ways: dp[used, sum]);
                    AccumulateDigitSplits(next, cell, (Value: digit, Count: count), table);
                }
            }
        }

        return next;
    }

    // The table being written is also the source of the fold's bounds, so the bounds
    // are read off it; the reached cell and the ways standing on it are one value, and
    // so are a digit and the number of copies being split.
    private static void AccumulateDigitSplits(
        long[,] next,
        (int Used, int Sum, long Ways) cell,
        (int Value, int Count) group,
        FactorialTable table)
    {
        var evenSlots = next.GetLength(0) - 1;
        var half = next.GetLength(1) - 1;

        for (var k = 0; k <= group.Count; k++)
        {
            var nextUsed = cell.Used + k;
            var nextSum = cell.Sum + k * group.Value;

            if (nextUsed > evenSlots || nextSum > half)
            {
                break;
            }

            var contribution = cell.Ways * table.InverseFactorial(k) % ModularArithmetic.Modulo *
                table.InverseFactorial(group.Count - k) % ModularArithmetic.Modulo;
            next[nextUsed, nextSum] = (next[nextUsed, nextSum] + contribution) % ModularArithmetic.Modulo;
        }
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
}
