using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.HIndex;

// LeetCode 274. H-Index: the largest h such that at least h of the
// researcher's papers have each been cited at least h times.
//
// The naive baseline recounts, for every candidate h from 0 to n, how many
// citations are >= h - O(n^2). Sorting once lets a single O(n) scan from
// the most-cited paper down answer the same question in O(n log n): at
// sorted index i counted from the end, "papers cited at least this much"
// is a running count, and h stops growing the moment a citation falls
// below it.
internal static class HIndexSolution
{
    // O(n^2): recount citations >= candidate for every candidate h. What
    // you would write without this repo - a plain BCL array, no sort.
    public static int HIndexByBruteForce(int[] citations)
    {
        var h = 0;

        for (var candidate = 0; candidate <= citations.Length; candidate++)
        {
            var count = 0;
            foreach (var citation in citations)
            {
                if (citation >= candidate)
                {
                    count++;
                }
            }

            if (count >= candidate)
            {
                h = candidate;
            }
        }

        return h;
    }

    // O(n log n): this repo's own MergeSort over ArrayIndexedSequence, then
    // a single O(n) scan from the most-cited paper down.
    public static int HIndexByMergeSortScan(int[] citations)
    {
        var sorted = citations.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var h = 0;
        for (var i = sorted.Length - 1; i >= 0; i--)
        {
            var papersAtLeastThisCited = sorted.Length - i;
            if (sorted[i] < papersAtLeastThisCited)
            {
                break;
            }

            h = papersAtLeastThisCited;
        }

        return h;
    }
}
