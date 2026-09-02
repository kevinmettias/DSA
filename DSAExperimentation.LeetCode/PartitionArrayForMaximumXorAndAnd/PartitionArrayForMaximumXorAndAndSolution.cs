namespace DSAExperimentation.LeetCode.PartitionArrayForMaximumXorAndAnd;

// LeetCode 3630. Partition Array for Maximum XOR and AND: split nums into three
// (possibly empty) subsequences A, B, C - every element in exactly one - to
// maximize XOR(A) + AND(B) + XOR(C).
//
// AND(B) depends only on the *set* B, never on how the rest splits between A and
// C, so both strategies enumerate B and combine it with the best possible
// XOR(A)+XOR(C) split of whatever nums doesn't put in B.
internal static class PartitionArrayForMaximumXorAndAndSolution
{
    // The textbook answer: try every one of the 3^n ways to label each element A,
    // B or C, folding XOR(A), AND(B) and XOR(C) as it goes. AND(B) starts as the
    // all-ones identity and only counts once B is non-empty, matching "AND of an
    // empty set is 0". Correct for every n this problem allows (<= 19), just
    // exponential - the arm the subset-basis strategy below has to beat.
    public static long MaxPartitionValueByBruteForce(int[] nums) =>
        AssignFrom(nums, index: 0, xorA: 0, andB: 0, hasB: false, xorC: 0);

    private static long AssignFrom(int[] nums, int index, long xorA, long andB, bool hasB, long xorC)
    {
        if (index == nums.Length)
        {
            return xorA + (hasB ? andB : 0) + xorC;
        }

        var value = nums[index];
        var toA = AssignFrom(nums, index + 1, xorA ^ value, andB, hasB, xorC);
        var toB = AssignFrom(nums, index + 1, xorA, hasB ? andB & value : value, true, xorC);
        var toC = AssignFrom(nums, index + 1, xorA, andB, hasB, xorC ^ value);

        return Math.Max(toA, Math.Max(toB, toC));
    }

    // Enumerate only which elements go to B (2^n masks) - AND(B) folds directly
    // from the mask. For whatever's left (T = the complement), every reachable
    // (XOR(A), XOR(C)) pair is (x, XOR(T) ^ x) for some x in T's XOR-subset span
    // (XOR(A) ^ XOR(C) = XOR(T) always, and any subset-XOR of T is reachable by
    // routing that subset to A and the rest of T to C). Maximizing
    // x + (XOR(T) ^ x) bit by bit shows every bit XOR(T) already has contributes a
    // fixed 1, and every bit it doesn't contributes 2 if x sets it - so the split's
    // best value is XOR(T) + 2 * the largest (x & ~XOR(T)) the basis can reach,
    // exactly what XorBasis.MaxMaskedXor answers.
    public static long MaxPartitionValueBySubsetXorBasis(int[] nums)
    {
        var n = nums.Length;
        var best = 0L;

        for (var mask = 0; mask < (1 << n); mask++)
        {
            var andB = 0L;
            var hasB = false;
            var xorT = 0L;
            var basis = new XorBasis();

            for (var i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    andB = hasB ? andB & nums[i] : nums[i];
                    hasB = true;
                }
                else
                {
                    xorT ^= nums[i];
                    basis.Insert(nums[i]);
                }
            }

            var splitValue = xorT + (2 * basis.MaxMaskedXor(~xorT));
            best = Math.Max(best, (hasB ? andB : 0) + splitValue);
        }

        return best;
    }
}
