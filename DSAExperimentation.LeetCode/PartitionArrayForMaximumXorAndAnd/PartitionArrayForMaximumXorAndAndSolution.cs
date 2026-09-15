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
        AssignFrom(nums, (Index: 0, XorA: 0, AndB: null, XorC: 0));

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
            var splitValue = BestSplitValueForMask(mask, nums);
            best = Math.Max(best, splitValue);
        }

        return best;
    }

    // One mask's whole fold: AND(B) and XOR(T) accumulated over the elements the mask
    // sends to B and to T respectively, T's XOR-subset span collected as a basis, and
    // the resulting best XOR(A) + XOR(C) split added on top of AND(B) (nothing when B
    // is empty, since AND of the empty set is 0).
    private static long BestSplitValueForMask(int mask, int[] nums)
    {
        var andB = 0L;
        var hasB = false;
        var xorT = 0L;
        var basis = new XorBasis();

        for (var i = 0; i < nums.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                andB = hasB ? AndWithValue(andB, nums[i]) : ElementAt(nums, i);
                hasB = true;
            }
            else
            {
                xorT ^= nums[i];
                basis.Insert(nums[i]);
            }
        }

        var splitValue = xorT + (2 * basis.MaxMaskedXor(~xorT));

        return (hasB ? andB : 0) + splitValue;
    }

    private static int ElementAt(int[] values, int index) => values[index];

    // The fold carries B's accumulated AND as a `long?` rather than as an AND beside a
    // "has B an element yet" flag: AND of an empty set is 0, not the all-ones identity,
    // so "no B element yet" is exactly what a missing value means for it.
    private static long AssignFrom(int[] nums, (int Index, long XorA, long? AndB, long XorC) state)
    {
        if (state.Index == nums.Length)
        {
            return state.XorA + (state.AndB ?? 0) + state.XorC;
        }

        var value = nums[state.Index];
        var next = state.Index + 1;
        var andBWithValue = state.AndB is null ? value : AndWithValue(state.AndB.Value, value);

        var toA = AssignFrom(nums, (next, state.XorA ^ value, state.AndB, state.XorC));
        var toB = AssignFrom(nums, (next, state.XorA, andBWithValue, state.XorC));
        var toC = AssignFrom(nums, (next, state.XorA, state.AndB, state.XorC ^ value));

        var bestOfBOrC = Math.Max(toB, toC);
        return Math.Max(toA, bestOfBOrC);
    }

    // B's running AND narrowed by one more element of nums.
    private static long AndWithValue(long accumulatedAnd, int value) => accumulatedAnd & value;
}
