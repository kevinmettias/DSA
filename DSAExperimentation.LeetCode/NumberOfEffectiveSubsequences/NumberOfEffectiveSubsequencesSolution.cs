using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfEffectiveSubsequences;

// LeetCode 3757. Number of Effective Subsequences: nums' strength is the
// bitwise OR of every element. A subsequence (any subset of indices, kept in
// their original order) is effective when removing it strictly lowers the OR
// of what remains. Count the effective subsequences, modulo 1e9+7.
//
// Removing a subset T can only ever leave the OR unchanged or smaller: every
// surviving element's bits already sit inside the full OR S, so the
// surviving OR is always a submask of S. So T is effective iff the surviving
// set's OR is NOT exactly S - counting "not exactly S" is the complement of
// counting "exactly S": answer = 2^n - (# subsets whose OR == S).
//
// "# subsets whose OR == S" is the classic sum-over-subsets problem, solved
// on the compact space of S's own set bits (every element is already a
// submask of S, so remapping S's m set bits to 0..m-1 loses nothing): a zeta
// transform turns per-value frequencies into "how many elements are a
// submask of t" counts, 2^(that count) is "# subsets whose OR is a submask
// of t", and a Mobius (inverse-zeta) pass over those recovers "# subsets
// whose OR is EXACTLY t" - read off at t = S's own compacted mask.
internal static class NumberOfEffectiveSubsequencesSolution
{
    // The textbook answer: walk every one of the 2^n subsets directly and
    // recompute the surviving OR from scratch. Deliberately written without
    // this repo's primitives - only tractable for small n, but it is the arm
    // the composed solution below has to agree with.
    public static int CountEffectiveByBruteForce(int[] nums)
    {
        var n = nums.Length;
        var strength = OrOfAll(nums);
        var effective = 0L;

        for (var removed = 0; removed < (1 << n); removed++)
        {
            if (IsEffectiveWhenRemoved(nums, removed, strength))
            {
                effective++;
            }
        }

        return (int)(effective % ModularArithmetic.Modulo);
    }

    // Whether removing exactly the index set `removed` lowers the array's OR:
    // the OR of the elements it keeps, recomputed from scratch, differs from
    // the full strength.
    private static bool IsEffectiveWhenRemoved(int[] nums, int removed, int strength)
    {
        var survivingOr = 0;

        for (var i = 0; i < nums.Length; i++)
        {
            if ((removed & (1 << i)) == 0)
            {
                survivingOr |= nums[i];
            }
        }

        return survivingOr != strength;
    }

    // Sum-over-subsets: count subsets whose OR equals the full strength via
    // zeta/Mobius over S's own bits, then complement against 2^n.
    public static int CountEffectiveByOrSubsetTransform(int[] nums)
    {
        var n = nums.Length;
        var strength = OrOfAll(nums);
        var bitPositions = CompactBitPositions(strength);
        var bitCount = bitPositions.Length;
        var universeSize = 1 << bitCount;

        var frequencyByMask = new long[universeSize];

        foreach (var value in nums)
        {
            frequencyByMask[Compact(value, bitPositions)]++;
        }

        var subsetsWithFullOr = CountSubsetsWithFullOr(frequencyByMask, bitCount);
        var totalSubsets = ModularArithmetic.Power(2, n);
        var effective = ((totalSubsets - subsetsWithFullOr) % ModularArithmetic.Modulo + ModularArithmetic.Modulo)
            % ModularArithmetic.Modulo;

        return (int)effective;
    }

    // The positions of strength's own set bits, low to high - the compact
    // 0..bitCount-1 space every element's bits get remapped into.
    private static int[] CompactBitPositions(int strength)
    {
        var positions = new List<int>();

        for (var bit = 0; strength >> bit != 0; bit++)
        {
            if (((strength >> bit) & 1) == 1)
            {
                positions.Add(bit);
            }
        }

        return [.. positions];
    }

    private static int Compact(int value, int[] bitPositions)
    {
        var compact = 0;

        for (var i = 0; i < bitPositions.Length; i++)
        {
            if (((value >> bitPositions[i]) & 1) == 1)
            {
                compact |= 1 << i;
            }
        }

        return compact;
    }

    // "# subsets whose OR is exactly the full strength", over the compacted
    // mask space: the zeta transform turns per-mask frequencies into "how many
    // elements are a submask of t" counts, 2^(that count) is "# subsets whose
    // OR is a submask of t", and the Mobius pass inverts those back to exact
    // values - read off at the full mask, the last one.
    private static long CountSubsetsWithFullOr(long[] frequencyByMask, int bitCount)
    {
        var subsetsByOrSubmask = new long[frequencyByMask.Length];

        ZetaTransformSubsetSums(frequencyByMask, bitCount);

        for (var mask = 0; mask < subsetsByOrSubmask.Length; mask++)
        {
            subsetsByOrSubmask[mask] = ModularArithmetic.Power(2, frequencyByMask[mask]);
        }

        MobiusInvertSubsetSums(subsetsByOrSubmask, bitCount);

        return subsetsByOrSubmask[subsetsByOrSubmask.Length - 1];
    }

    // In place: counts[t] becomes the number of elements whose compacted mask
    // is a submask of t, for every t - the standard subset-sum zeta transform.
    private static void ZetaTransformSubsetSums(long[] counts, int bitCount)
    {
        for (var bit = 0; bit < bitCount; bit++)
        {
            var bitMask = 1 << bit;

            for (var mask = 0; mask < counts.Length; mask++)
            {
                if ((mask & bitMask) != 0)
                {
                    counts[mask] += counts[mask ^ bitMask];
                }
            }
        }
    }

    // The inverse of ZetaTransformSubsetSums: turns "value at t = sum over
    // submasks s of t of f(s)" back into f, in place, mod 1e9+7.
    private static void MobiusInvertSubsetSums(long[] values, int bitCount)
    {
        for (var bit = 0; bit < bitCount; bit++)
        {
            var bitMask = 1 << bit;

            for (var mask = 0; mask < values.Length; mask++)
            {
                if ((mask & bitMask) != 0)
                {
                    values[mask] = ((values[mask] - values[mask ^ bitMask]) % ModularArithmetic.Modulo
                        + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
                }
            }
        }
    }

    private static int OrOfAll(int[] nums)
    {
        var strength = 0;

        foreach (var value in nums)
        {
            strength |= value;
        }

        return strength;
    }
}
