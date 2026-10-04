using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.HouseRobberIV;

// LeetCode 2560. House Robber IV: the robber's "capability" is the largest amount
// taken from any single house, and the answer is the smallest capability that
// still allows robbing at least requiredHouseCount non-adjacent houses.
//
// Feasibility is monotone in the capability - once some cap admits requiredHouseCount
// non-adjacent houses, every larger cap admits at least as many, because raising
// the cap only ever adds candidates - so the answer is the leftmost "true" in an
// implicit [false...false, true...true] sequence over cap in [min(nums),
// max(nums)]. The greedy count itself is the same either way: walk left to right
// and take every affordable house whose predecessor was not taken.
//
// LinearScan tries each candidate cap in turn - what you would write without this
// repo. PredicateSearch states the same predicate as an IMonotonePredicate<int>
// so this repo's own MonotonePredicateSearch.FirstTrue can bisect the cap range,
// turning O(range * n) into O(n * log range) - the same search-on-the-answer
// shape LC 410 and LC 875 use.
internal static class HouseRobberIVSolution
{
    // The textbook answer: sweep every candidate capability upward from the
    // cheapest house and stop at the first that admits requiredHouseCount robberies.
    // Deliberately written without this repo's primitives - a plain loop over the
    // value range - it is the arm the composed solution below has to justify itself
    // against.
    public static int MinCapabilityByLinearScan(int[] nums, int requiredHouseCount)
    {
        var floor = nums.Min();
        var ceiling = nums.Max();

        for (var cap = floor; cap <= ceiling; cap++)
        {
            if (CanRobAtLeastHousesWithinCap(nums, requiredHouseCount, cap))
            {
                return cap;
            }
        }

        return ceiling;
    }

    // The greedy count both strategies share: taking every affordable house whose
    // predecessor was skipped is optimal, because deferring an affordable house
    // can never let more of them be taken later.
    private static bool CanRobAtLeastHousesWithinCap(int[] nums, int requiredHouseCount, int cap)
    {
        var count = 0;
        var previousRobbed = false;

        foreach (var value in nums)
        {
            if (value <= cap && !previousRobbed)
            {
                count++;
                previousRobbed = true;
            }
            else
            {
                previousRobbed = false;
            }
        }

        return count >= requiredHouseCount;
    }

    // Bisect the same monotone predicate instead of scanning it. The search runs
    // over the caps themselves, so the first cap the rule holds for is the answer.
    public static int MinCapabilityByPredicateSearch(int[] nums, int requiredHouseCount) =>
        MonotonePredicateSearch.FirstTrue(nums.Min(), nums.Max(), new RobsEnoughWithinCap(nums, requiredHouseCount));

    // Holds(cap) is "at least requiredHouseCount non-adjacent houses are affordable
    // under this cap". Meaningless outside this one problem's feasibility check -
    // stays beside the solution rather than in DataStructures/ or Algorithms/
    // (§17.3's CountWaysToBuildRoomsInAnAntColony precedent, and LC 410's own
    // SplitsWithinLimit).
    private readonly struct RobsEnoughWithinCap(int[] nums, int requiredHouseCount) : IMonotonePredicate<int>
    {
        public bool Holds(int cap) => CanRobAtLeastHousesWithinCap(nums, requiredHouseCount, cap);
    }
}
