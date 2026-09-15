using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PeakIndexInAMountainArray;

// LeetCode 852. Peak Index in a Mountain Array: the array is unimodal (strictly
// increasing, then strictly decreasing), and the answer is the index of its
// summit.
//
// The baseline walks until the first downhill step, O(n). The composed solution
// encodes "is the step from i to i + 1 downhill" as a 0/1 sequence - monotonic
// precisely because the array is unimodal - so this repo's own
// BinarySearch.LowerBound lands on the peak directly, O(log n). Same
// proxy-sequence idiom FindMinimumInRotatedSortedArray uses for its own pivot
// search.
internal static class PeakIndexInAMountainArraySolution
{
    // The value that marks a downhill step in the proxy sequence; LowerBound
    // wants the first index that reaches it.
    private const int Downhill = 1;

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

    // O(log n): BinarySearch.LowerBound over the downhill-step proxy sequence.
    public static int PeakIndexByBinarySearchLowerBound(int[] mountain) =>
        BinarySearch.LowerBound<int, DescendingStepSequence>(
            new DescendingStepSequence(mountain), Downhill);

    // Get(index) is 1 exactly when the step from mountain[index] to
    // mountain[index + 1] is downhill - the peak itself is the first such index,
    // and the unimodal precondition is what keeps this sequence monotonic, the
    // same unenforced-sortedness shape BinarySearch.LowerBound already leans on.
    private readonly struct DescendingStepSequence(int[] mountain) : IRandomAccessSequence<int>
    {
        public int Length => mountain.Length - 1;

        public int Get(int index)
        {
            var isDownhillStep = mountain[index] > mountain[index + 1];

            return isDownhillStep ? 1 : 0;
        }
    }
}
