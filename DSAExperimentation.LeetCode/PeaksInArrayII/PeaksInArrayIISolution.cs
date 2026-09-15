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
                var rangeCount = CountPeakSubarraysInRange(working, query[1], query[2]);
                answer.Add(rangeCount);
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
        var tree = BuildPeakTree(working);

        return RunQueries(working, tree, queries);
    }

    // The tree both phases answer against: one leaf per index, each carrying whether that
    // index is a peak, over which the merge operation accumulates the gap structure.
    private static SegmentTree<PeakGapNode, PeakGapMergeOperation> BuildPeakTree(int[] working)
    {
        var leaves = new PeakGapNode[working.Length];

        for (var i = 0; i < working.Length; i++)
        {
            var peak = IsPeak(working, i);
            leaves[i] = PeakGapNode.Leaf(peak, i);
        }

        return new SegmentTree<PeakGapNode, PeakGapMergeOperation>(leaves);
    }

    // Replays the queries in order against one already-built tree: a type-1 query's count
    // is collected, a type-2 update is applied. The answers come back in query order.
    private static List<long> RunQueries(
        int[] working, SegmentTree<PeakGapNode, PeakGapMergeOperation> tree, int[][] queries)
    {
        var answer = new List<long>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                var rangeCount = CountPeakSubarrays(tree, query[1], query[2]);
                answer.Add(rangeCount);
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
            ? PeakFreeSubarrays(interior, left, right)
            : totalSubarrays;

        return totalSubarrays - noPeakSubarrays;
    }

    private static long PeakFreeSubarrays(PeakGapNode interior, int left, int right) =>
        interior.InteriorGapPairs
            + PeakGapNode.PairsInGap(interior.MinPeak - left)
            + PeakGapNode.PairsInGap(right - interior.MaxPeak);

    private static void ApplyUpdate(int[] working, SegmentTree<PeakGapNode, PeakGapMergeOperation> tree, int index, int value)
    {
        working[index] = value;

        foreach (var position in AffectedPositions(index, working.Length))
        {
            var peak = IsPeak(working, position);
            var leaf = PeakGapNode.Leaf(peak, position);
            tree.Update(position, leaf);
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
