namespace DSAExperimentation.LeetCode.MinimumIncrementsForTargetMultiplesInAnArray;

// Precomputes the LCM of every subset of target - at most 2^4 = 16 of them, since
// LC pins target.Length <= 4. Subset 0 (the empty set) is left at its default 0 and
// never consulted: both strategies only ever look up non-empty subsets, the ones a
// single nums element could be raised to a multiple of.
//
// This is what the hoisted overload takes: SubsetLcm[subset] is the one piece of
// per-query setup worth charging to [GlobalSetup] rather than the measured method,
// the same role LockGraph.Build(deadends) plays for OpenTheLock.
internal readonly record struct TargetLcmTable(int TargetCount, long[] SubsetLcm)
{
    public static TargetLcmTable Build(int[] target)
    {
        var subsetLcm = new long[1 << target.Length];

        for (var subset = 1; subset < subsetLcm.Length; subset++)
        {
            subsetLcm[subset] = LcmOfSubset(target, subset);
        }

        return new TargetLcmTable(target.Length, subsetLcm);
    }

    private static long LcmOfSubset(int[] target, int subset)
    {
        var lcm = 1L;

        for (var bit = 0; bit < target.Length; bit++)
        {
            if ((subset & (1 << bit)) != 0)
            {
                lcm = Lcm(lcm, target[bit]);
            }
        }

        return lcm;
    }

    private static long Lcm(long firstValue, long secondValue)
        => firstValue / Gcd(firstValue, secondValue) * secondValue;

    private static long Gcd(long firstValue, long secondValue)
    {
        while (secondValue != 0)
        {
            (firstValue, secondValue) = (secondValue, firstValue % secondValue);
        }

        return firstValue;
    }
}
