using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MinimizeTheMaximumOfTwoArrays;

// LeetCode 2513. Minimize the Maximum of Two Arrays: fill arr1 with uniqueCnt1
// distinct positive integers none of which is divisible by divisor1, and arr2 with
// uniqueCnt2 distinct positive integers none of which is divisible by divisor2, the
// two arrays disjoint, minimizing the largest number used.
//
// "Can both arrays be filled using only numbers in [1, m]?" is monotone - raising
// the cap only ever adds eligible numbers - so the answer is the leftmost true in an
// implicit [false...false, true...true] sequence over m in [1, 2 * (uniqueCnt1 +
// uniqueCnt2)]. The baseline bisects that range with a hand-rolled lo/hi loop; the
// composed strategy states the same predicate as an IRandomAccessSequence<bool>
// computed on demand and hands it to this repo's own BinarySearch.LowerBound, the
// same search-on-answer shape KokoEatingBananas and SplitArrayLargestSum use.
//
// Feasibility is inclusion-exclusion over [1, m]: eligible1 = the numbers not
// divisible by divisor1 (candidates for arr1), eligible2 = those not divisible by
// divisor2, and eligibleEither = those divisible by neither divisor, hence usable by
// whichever array still needs them. m works iff eligible1 covers uniqueCnt1,
// eligible2 covers uniqueCnt2, and eligibleEither covers both counts at once.
internal static class MinimizeTheMaximumOfTwoArraysSolution
{
    // Maximums are 1-based while sequence indices are 0-based.
    private const int SmallestMaximum = 1;

    private const int MidpointDivisor = 2;

    // 2 * (uniqueCnt1 + uniqueCnt2) is always feasible: the worst divisor, 2, still
    // leaves half the range eligible for both arrays together.
    private const int WorstCaseMaximumFactor = 2;

    // The textbook answer: a hand-written bisection over the maximum, BCL-only.
    public static int MinimizeSetByManualBisection(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2)
    {
        var low = (long)SmallestMaximum;
        var high = SmallestFeasibleUpperBound(uniqueCnt1, uniqueCnt2);

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);

            if (IsFeasible(divisor1, divisor2, uniqueCnt1, uniqueCnt2, mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return (int)low;
    }

    // This repo's own BinarySearch.LowerBound over the feasibility sequence: the
    // candidate maximums are never materialized, each probe just recomputes the
    // inclusion-exclusion counts, and the leftmost feasible index maps back to its
    // maximum.
    public static int MinimizeSetBySequenceLowerBound(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2)
    {
        var sequence = new FeasibleMaximumSequence(divisor1, divisor2, uniqueCnt1, uniqueCnt2);

        return SmallestMaximum + BinarySearch.LowerBound<bool, FeasibleMaximumSequence>(sequence, true);
    }

    // One past the largest maximum either search ever has to consider, so the
    // bisection and the sequence cover exactly the same candidate range.
    private static long SmallestFeasibleUpperBound(int uniqueCnt1, int uniqueCnt2)
        => ((long)WorstCaseMaximumFactor * (uniqueCnt1 + (long)uniqueCnt2)) + 1;

    // The feasibility predicate, and the only thing either strategy asks about a
    // candidate maximum: monotone in the maximum, which is what makes bisecting it
    // valid.
    private static bool IsFeasible(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2, long max)
    {
        var lcm = Lcm(divisor1, divisor2);
        var eligible1 = max - (max / divisor1);
        var eligible2 = max - (max / divisor2);
        var eligibleEither = max - (max / lcm);

        return eligible1 >= uniqueCnt1
            && eligible2 >= uniqueCnt2
            && eligibleEither >= uniqueCnt1 + (long)uniqueCnt2;
    }

    private static long Lcm(int a, int b) => (long)a / Gcd(a, b) * b;

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    // Get(index) is "maximum index + 1 fills both arrays" - false up to the answer
    // and true from there on, the monotonicity BinarySearch.LowerBound assumes but
    // never checks. A witness for this problem alone: the two-divisor eligibility
    // rule is LC 2513's own content, not a general monotone-predicate shape.
    private readonly struct FeasibleMaximumSequence(int divisor1, int divisor2, int uniqueCnt1, int uniqueCnt2)
        : IRandomAccessSequence<bool>
    {
        public int Length => (WorstCaseMaximumFactor * (uniqueCnt1 + uniqueCnt2)) + 1;

        public bool Get(int index)
            => IsFeasible(divisor1, divisor2, uniqueCnt1, uniqueCnt2, SmallestMaximum + index);
    }
}
