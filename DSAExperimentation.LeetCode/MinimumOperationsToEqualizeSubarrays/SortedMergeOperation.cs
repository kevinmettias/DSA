using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumOperationsToEqualizeSubarrays;

// SegmentTree<int[], TOperation>'s combine witness for this problem's merge-
// sort tree: two adjacent ranges' already-sorted arrays merge into one larger
// sorted array via a plain two-cursor walk, the same merge step MergeSort.cs
// performs pairwise - written directly rather than composed from MergeSort
// because Combine has to return a brand-new Element per SegmentTree's own
// contract, not mutate a shared IIndexedSequence in place the way MergeSort's
// Merge does.
internal readonly struct SortedMergeOperation : ICombineOperation<int[]>
{
    public static int[] Identity => [];

    public static int[] Combine(int[] left, int[] right)
    {
        var merged = new int[left.Length + right.Length];
        var i = 0;
        var j = 0;

        for (var next = 0; next < merged.Length; next++)
        {
            var takeLeft = i < left.Length && (j >= right.Length || left[i] <= right[j]);
            merged[next] = takeLeft ? left[i++] : right[j++];
        }

        return merged;
    }
}
