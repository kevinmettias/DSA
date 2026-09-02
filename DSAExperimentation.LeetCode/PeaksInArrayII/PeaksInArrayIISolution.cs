using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.PeaksInArrayII;

// LeetCode 4017. Peaks in Array II: nums[i..j] (length >= 3) is a "peak
// subarray" if some index k with i < k < j is a local peak of nums - nums[k] >
// both neighbors, checked against the FULL array, the same predicate LC 3187
// (PeaksInArray) uses. queries[q] is either [1, l, r] - count peak subarrays
// fully contained in nums[l..r] - or [2, index, val] - nums[index] = val.
// Answer every type-1 query, in order.
//
// What's new versus LC 3187 is counting *subarrays* that contain at least one
// peak, not peak positions themselves. For a fixed [l, r], every length>=3
// subarray either straddles a peak or falls entirely inside one contiguous
// no-peak gap, so: answer = (total length>=3 subarrays in [l,r]) - (subarrays
// with no peak strictly inside). Both strategies compute that same
// subtraction; they differ only in how "no-peak subarrays" is counted. n, the
// query bound and the peak count can all reach 1e5, so a single query's count
// can exceed Int32 range (~5e9 pairs at n=1e5) - every answer here is a long.
internal static class PeaksInArrayIISolution
{
    // The textbook answer: keep a mutable copy of nums and, for every type-1
    // query, literally enumerate every candidate subarray and rescan its
    // interior for a peak. Deliberately BCL-only and O((r-l)^3) worst case per
    // query - the arm the segment-tree strategy below has to justify itself
    // against.
    public static List<long> CountPeakSubarraysByBruteForce(int[] nums, int[][] queries)
    {
        var working = (int[])nums.Clone();
        var answer = new List<long>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                answer.Add(CountPeakSubarraysInRange(working, query[1], query[2]));
            }
            else
            {
                working[query[1]] = query[2];
            }
        }

        return answer;
    }

    private static long CountPeakSubarraysInRange(int[] nums, int left, int right)
    {
        var count = 0L;

        for (var i = left; i <= right - 2; i++)
        {
            for (var j = i + 2; j <= right; j++)
            {
                if (HasPeakBetween(nums, i, j))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool HasPeakBetween(int[] nums, int exclusiveLeft, int exclusiveRight)
    {
        for (var k = exclusiveLeft + 1; k < exclusiveRight; k++)
        {
            if (IsPeak(nums, k))
            {
                return true;
            }
        }

        return false;
    }

    // Composed: a point-update SegmentTree<PeakGapNode, PeakGapMergeOperation>
    // (DataStructures/SegmentTree) keeps, per range, the leftmost/rightmost
    // peak in it and the total pair-count of every no-peak gap strictly
    // between consecutive peaks inside it (PeakGapMergeOperation.cs). A
    // type-1 query reads the tree over the query's open interior (l+1, r-1):
    // combined with the two boundary gaps (query edge to nearest interior
    // peak), that is every no-peak gap [l,r] can contain, so "total subarrays
    // minus those gaps' own subarray counts" is the answer in O(log n) per
    // query. A type-2 update can only change peak status at index-1, index
    // and index+1, so it is three or fewer O(log n) tree writes instead of an
    // O(n) rescan.
    public static List<long> CountPeakSubarraysBySegmentTree(int[] nums, int[][] queries)
    {
        var working = (int[])nums.Clone();
        var leaves = new PeakGapNode[working.Length];

        for (var i = 0; i < working.Length; i++)
        {
            leaves[i] = PeakGapNode.Leaf(IsPeak(working, i), i);
        }

        var tree = new SegmentTree<PeakGapNode, PeakGapMergeOperation>(leaves);
        var answer = new List<long>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                answer.Add(CountPeakSubarrays(tree, query[1], query[2]));
            }
            else
            {
                ApplyUpdate(working, tree, query[1], query[2]);
            }
        }

        return answer;
    }

    private static long CountPeakSubarrays(SegmentTree<PeakGapNode, PeakGapMergeOperation> tree, int left, int right)
    {
        var span = right - left;

        if (span < 2)
        {
            return 0;
        }

        var totalSubarrays = PeakGapNode.PairsInGap(span);
        var interior = tree.Query(left + 1, right - 1);

        var noPeakSubarrays = interior.HasPeak
            ? interior.InteriorGapPairs
                + PeakGapNode.PairsInGap(interior.MinPeak - left)
                + PeakGapNode.PairsInGap(right - interior.MaxPeak)
            : totalSubarrays;

        return totalSubarrays - noPeakSubarrays;
    }

    private static void ApplyUpdate(int[] working, SegmentTree<PeakGapNode, PeakGapMergeOperation> tree, int index, int value)
    {
        working[index] = value;

        foreach (var position in AffectedPositions(index, working.Length))
        {
            tree.Update(position, PeakGapNode.Leaf(IsPeak(working, position), position));
        }
    }

    private static IEnumerable<int> AffectedPositions(int index, int length)
    {
        for (var position = index - 1; position <= index + 1; position++)
        {
            if (position >= 1 && position <= length - 2)
            {
                yield return position;
            }
        }
    }

    private static bool IsPeak(int[] nums, int index) =>
        index > 0 && index < nums.Length - 1 && nums[index] > nums[index - 1] && nums[index] > nums[index + 1];
}
