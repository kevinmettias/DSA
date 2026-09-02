namespace DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenI;

// LeetCode 2928. Distribute Candies Among Children I: count the ways to hand out n
// candies to 3 children so that no child receives more than limit candies. n and
// limit are both at most 50 here, which is exactly what keeps the double loop
// below fast enough - LC 2929 asks the identical question at up to 1e6 and needs
// the closed form.
internal static class DistributeCandiesAmongChildrenISolution
{
    // The textbook double loop: fix the first two children's shares, the third is
    // forced by n, and only counted when it also respects the limit.
    public static int CountWaysByBruteForce(int n, int limit)
    {
        var ways = 0;

        for (var first = 0; first <= Math.Min(n, limit); first++)
        {
            for (var second = 0; second <= Math.Min(n - first, limit); second++)
            {
                var third = n - first - second;

                if (third >= 0 && third <= limit)
                {
                    ways++;
                }
            }
        }

        return ways;
    }

    // Stars-and-bars for a+b+c=n counts every nonnegative solution; inclusion-
    // exclusion then subtracts back the ones where one child alone already exceeds
    // limit, adds back the ones where two children do, and subtracts the ones
    // where all three do - O(1), the same identity LC 2929 needs at its larger
    // bound.
    public static int CountWaysByInclusionExclusion(int n, int limit)
    {
        var excess = limit + 1;

        var total = SolutionsIgnoringLimit(n)
            - 3 * SolutionsIgnoringLimit(n - excess)
            + 3 * SolutionsIgnoringLimit(n - 2 * excess)
            - SolutionsIgnoringLimit(n - 3 * excess);

        return (int)total;
    }

    // Nonnegative integer solutions to a+b+c=total, ignoring any upper bound: the
    // classic C(total+2, 2) stars-and-bars count.
    private static long SolutionsIgnoringLimit(int total) =>
        total < 0 ? 0 : (long)(total + 2) * (total + 1) / 2;
}
