namespace DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenII;

// LeetCode 2929. Distribute Candies Among Children II: the same "3 children, no
// more than limit candies each" count as LC 2928, but the candy count and limit are
// now both up to 1e6, so the answer no longer fits an int and the double loop no
// longer fits a benchmark budget - the closed form is what actually has to carry
// this version.
internal static class DistributeCandiesAmongChildrenIISolution
{
    // Same double loop LC 2928 uses. Still correct at LC 2929's bound, just the
    // arm the O(1) formula below has to beat once the candy count and limit reach
    // real size.
    public static long CountWaysByBruteForce(int candyCount, int limit)
    {
        var ways = 0L;

        for (var first = 0; first <= Math.Min(candyCount, limit); first++)
        {
            for (var second = 0; second <= Math.Min(candyCount - first, limit); second++)
            {
                var third = candyCount - first - second;

                if (third >= 0 && third <= limit)
                {
                    ways++;
                }
            }
        }

        return ways;
    }

    // Same stars-and-bars/inclusion-exclusion identity as LC 2928, kept in long
    // throughout: at the top of that bound the unbounded solution count alone is on
    // the order of 5*10^11, well past int range.
    public static long CountWaysByInclusionExclusion(int candyCount, int limit)
    {
        var excess = (long)limit + 1;

        return SolutionsIgnoringLimit(candyCount)
            - 3 * SolutionsIgnoringLimit(candyCount - excess)
            + 3 * SolutionsIgnoringLimit(candyCount - 2 * excess)
            - SolutionsIgnoringLimit(candyCount - 3 * excess);
    }

    // Nonnegative integer solutions to a+b+c=total, ignoring any upper bound: the
    // classic C(total+2, 2) stars-and-bars count.
    private static long SolutionsIgnoringLimit(long total) =>
        total < 0 ? 0 : StarsAndBarsCount(total);

    private static long StarsAndBarsCount(long total) => (total + 2) * (total + 1) / 2;
}
