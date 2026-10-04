using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.PeakIndexInAMountainArray;

// LeetCode 852. Peak Index in a Mountain Array: the array is unimodal (strictly
// increasing, then strictly decreasing), and the answer is the index of its
// summit.
//
// The baseline walks until the first downhill step, O(n). The composed solution
// states "is the step from i to i + 1 downhill" as a rule over the indices -
// monotone precisely because the array is unimodal - so this repo's own
// MonotonePredicateSearch.FirstTrue lands on the peak directly, O(log n). Same
// index-rule idiom FindMinimumInRotatedSortedArray uses for its own pivot
// search.
internal static class PeakIndexInAMountainArraySolution
{
    // The textbook answer: scan for the first index whose successor is smaller.
    // O(n), no repo primitive.
    public static int PeakIndexByLinearScan(int[] mountain)
    {
        for (var i = 0; i < mountain.Length - 1; i++)
        {
            if (mountain[i] > mountain[i + 1])
            {
                return i;
            }
        }

        return mountain.Length - 1;
    }

    // O(log n): MonotonePredicateSearch.FirstTrue over every index that has a
    // successor to step down to - all but the last.
    public static int PeakIndexByPredicateSearch(int[] mountain) =>
        MonotonePredicateSearch.FirstTrue(0, mountain.Length - 2, new StepIsDownhill(mountain));

    // IsSatisfiedBy(index) is true exactly when the step from mountain[index] to
    // mountain[index + 1] is downhill - the peak itself is the first such index,
    // and the unimodal precondition is what keeps this rule monotone, a law
    // MonotonePredicateSearch assumes but never checks.
    private readonly struct StepIsDownhill(int[] mountain) : IMonotonePredicate<int>
    {
        public bool IsSatisfiedBy(int index) => mountain[index] > mountain[index + 1];
    }
}
