using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.LeetCode.KokoEatingBananas;

// LeetCode 875. Koko Eating Bananas: the smallest eating speed k that clears every
// pile within hourBudget hours, where a pile of size p costs ceil(p / k) hours.
//
// "Can Koko finish at speed k?" is monotone - once a speed is feasible every faster
// speed stays feasible - so the answer is the leftmost true in an implicit
// [false...false, true...true] sequence over k in [1, max(piles)]. The baseline
// bisects that range with a hand-rolled lo/hi loop; the composed strategy states the
// same predicate as an IMonotonePredicate<int> and hands it, with the speed range, to
// this repo's own MonotonePredicateSearch.FirstTrue, the same search-on-answer shape
// SplitArrayLargestSum uses.
internal static class KokoEatingBananasSolution
{
    // Koko eats at least one banana an hour, so the speed range starts at 1.
    private const int SlowestSpeed = 1;

    // The textbook answer: a hand-written bisection over the speed range, BCL-only.
    public static int MinEatingSpeedByManualBisection(int[] piles, int hourBudget)
    {
        var low = SlowestSpeed;
        var high = piles.Max();

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (CanClearEveryPile(piles, mid, hourBudget))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    // Koko's feasibility predicate, and the only thing either strategy asks about a
    // candidate speed: monotone in speed, which is what makes bisecting it valid.
    private static bool CanClearEveryPile(int[] piles, int speed, int hourBudget) =>
        HoursNeeded(piles, speed) <= hourBudget;

    private static long HoursNeeded(int[] piles, int speed)
    {
        var hours = 0L;

        foreach (var pile in piles)
        {
            hours += (pile + speed - 1) / speed;
        }

        return hours;
    }

    // This repo's own MonotonePredicateSearch over the speed range: the speeds are
    // never materialized, each probe just recomputes the hour count, and the first
    // speed that clears every pile is the answer.
    public static int MinEatingSpeedByPredicateSearch(int[] piles, int hourBudget) =>
        MonotonePredicateSearch.FirstTrue(SlowestSpeed, piles.Max(), new ClearsEveryPile(piles, hourBudget));

    // Holds(speed) is "this speed clears every pile within hourBudget hours" - false
    // up to the answer and true from there on, the monotonicity
    // MonotonePredicateSearch assumes but never checks. A rule for this problem
    // alone: the feasibility rule is Koko's own content.
    private readonly struct ClearsEveryPile(int[] piles, int hourBudget) : IMonotonePredicate<int>
    {
        public bool Holds(int speed) => CanClearEveryPile(piles, speed, hourBudget);
    }
}
