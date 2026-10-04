using DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenII;

namespace DSAExperimentation.LeetCode.DistributeCandiesAmongChildrenI;

// LeetCode 2928. Distribute Candies Among Children I: count the ways to hand out
// the given number of candies to 3 children so that no child receives more than
// limit candies. The candy count and limit are both at most 50 here, which is
// exactly what keeps the double loop below fast enough - LC 2929 asks the
// identical question at up to 1e6 and needs the closed form.
internal static class DistributeCandiesAmongChildrenISolution
{
    // The textbook double loop: fix the first two children's shares, the third is
    // forced by the total, and only counted when it also respects the limit. LC 2929
    // asks the identical question at a bound where the count outgrows an int, so its
    // long-returning loop is the one implementation of that loop; this arm is it
    // narrowed to this problem's answer type, which fits because the candy count and
    // limit stop at 50 here.
    public static int CountWaysByBruteForce(int candyCount, int limit) =>
        (int)DistributeCandiesAmongChildrenIISolution.CountWaysByBruteForce(candyCount, limit);

    // Stars-and-bars for a+b+c=n counts every nonnegative solution; inclusion-
    // exclusion then subtracts back the ones where one child alone already exceeds
    // limit, adds back the ones where two children do, and subtracts the ones
    // where all three do - O(1). LC 2929 owns the identity (ARCHITECTURE 17.3),
    // kept in long for its larger bound; this arm narrows it.
    public static int CountWaysByInclusionExclusion(int candyCount, int limit) =>
        (int)DistributeCandiesAmongChildrenIISolution.CountWaysByInclusionExclusion(candyCount, limit);
}
