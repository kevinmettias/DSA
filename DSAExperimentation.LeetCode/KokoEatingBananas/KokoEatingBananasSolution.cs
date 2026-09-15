using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.KokoEatingBananas;

// LeetCode 875. Koko Eating Bananas: the smallest eating speed k that clears every
// pile within h hours, where a pile of size p costs ceil(p / k) hours.
//
// "Can Koko finish at speed k?" is monotone - once a speed is feasible every faster
// speed stays feasible - so the answer is the leftmost true in an implicit
// [false...false, true...true] sequence over k in [1, max(piles)]. The baseline
// bisects that range with a hand-rolled lo/hi loop; the composed strategy states the
// same predicate as an IRandomAccessSequence<bool> computed on demand and hands it to
// this repo's own BinarySearch.LowerBound, the same search-on-answer shape
// SplitArrayLargestSum uses.
internal static class KokoEatingBananasSolution
{
    // Speeds are 1-based while sequence indices are 0-based.
    private const int SlowestSpeed = 1;

    // The textbook answer: a hand-written bisection over the speed range, BCL-only.
    public static int MinEatingSpeedByManualBisection(int[] piles, int h)
    {
        var low = SlowestSpeed;
        var high = piles.Max();

        while (low < high)
        {
            var mid = low + ((high - low) / AlgorithmConstants.HalvingFactor);

            if (ClearsEveryPile(piles, mid, h))
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
    private static bool ClearsEveryPile(int[] piles, int speed, int h) => HoursNeeded(piles, speed) <= h;

    private static long HoursNeeded(int[] piles, int speed)
    {
        var hours = 0L;

        foreach (var pile in piles)
        {
            hours += (pile + speed - 1) / speed;
        }

        return hours;
    }

    // This repo's own BinarySearch.LowerBound over the feasibility sequence: the
    // speeds are never materialized, each probe just recomputes the hour count, and
    // the leftmost feasible index maps back to its speed.
    public static int MinEatingSpeedBySequenceLowerBound(int[] piles, int h)
    {
        var sequence = new FeasibleSpeedSequence(piles, h);

        return SlowestSpeed + BinarySearch.LowerBound<bool, FeasibleSpeedSequence>(sequence, true);
    }

    // Get(index) is "speed index + 1 clears every pile within h hours" - false up to
    // the answer and true from there on, the monotonicity BinarySearch.LowerBound
    // assumes but never checks. A witness for this problem alone: the feasibility
    // rule is Koko's own content, not a general monotone-predicate shape.
    private readonly struct FeasibleSpeedSequence(int[] piles, int h) : IRandomAccessSequence<bool>
    {
        public int Length => piles.Max();

        public bool Get(int index) => ClearsEveryPile(piles, SlowestSpeed + index, h);
    }
}
