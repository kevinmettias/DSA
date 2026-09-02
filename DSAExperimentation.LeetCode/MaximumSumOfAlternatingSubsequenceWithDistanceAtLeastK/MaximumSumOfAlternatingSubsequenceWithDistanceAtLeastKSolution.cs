using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

using RepoSegmentTree = DSAExperimentation.DataStructures.SegmentTree.SegmentTree<long, DSAExperimentation.DataStructures.SegmentTree.MaxOperation<long>>;

namespace DSAExperimentation.LeetCode.MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastK;

// LeetCode 3915. Maximum Sum of Alternating Subsequence With Distance at Least K:
// pick indices i_1 < i_2 < ... with i_{j+1} - i_j >= k whose values strictly
// alternate (up-down-up-... or down-up-down-...), maximizing the sum of the
// selected values.
//
// dp splits into two roles per index i: peakEnd[i] is the best sum of a chain
// ending at i where the LAST step rose to i (so the next pick, if any, must be
// smaller); valleyEnd[i] is the mirror image (the next pick must be larger).
// peakEnd[i] = nums[i] + max(0, best valleyEnd[j] over eligible j with
// nums[j] < nums[i]); valleyEnd[i] is symmetric over peakEnd[j] with
// nums[j] > nums[i]. "Eligible" means j <= i - k - the distance constraint,
// enforced by only ever looking at predecessors already committed at the time i
// is processed.
internal static class MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution
{
    // Textbook O(n^2): for each i, scan every eligible earlier j directly. The
    // arm the segment-tree sweep below has to beat.
    public static long MaxAlternatingSumByBruteForce(int[] nums, int k)
    {
        var n = nums.Length;
        var peakEnd = new long[n];
        var valleyEnd = new long[n];
        var answer = long.MinValue;

        for (var i = 0; i < n; i++)
        {
            var bestFromValley = 0L;
            var bestFromPeak = 0L;

            for (var j = 0; j <= i - k; j++)
            {
                if (nums[j] < nums[i])
                {
                    bestFromValley = Math.Max(bestFromValley, valleyEnd[j]);
                }
                else if (nums[j] > nums[i])
                {
                    bestFromPeak = Math.Max(bestFromPeak, peakEnd[j]);
                }
            }

            peakEnd[i] = nums[i] + bestFromValley;
            valleyEnd[i] = nums[i] + bestFromPeak;
            answer = Math.Max(answer, Math.Max(peakEnd[i], valleyEnd[i]));
        }

        return answer;
    }

    // Composed: coordinate-compress nums via this repo's own BinarySearch.LowerBound
    // over the sorted distinct values (the same idiom
    // MaximumBalancedSubsequenceSumSolution uses for its own value-ranked sweep),
    // then sweep left to right through two SegmentTree<long, MaxOperation<long>>
    // instances ranked by value - one holding valleyEnd (queried by rank range
    // "< nums[i]" to extend into a peak), the other holding peakEnd (queried by
    // "> nums[i]" to extend into a valley). The distance constraint is enforced by
    // delaying insertion: index j is only written into either tree once the sweep
    // reaches i = j + k, so a query at i can never see a predecessor closer than k.
    public static long MaxAlternatingSumBySegmentTree(int[] nums, int k)
    {
        var n = nums.Length;
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var rankCount = sortedDistinct.Length;

        var peakByValleyRank = BuildRankedTree(rankCount);
        var valleyByPeakRank = BuildRankedTree(rankCount);

        var peakEnd = new long[n];
        var valleyEnd = new long[n];
        var answer = long.MinValue;
        var activated = 0;

        for (var i = 0; i < n; i++)
        {
            while (activated <= i - k)
            {
                var rank = BinarySearch.LowerBound(sequence, nums[activated]);
                Raise(peakByValleyRank, rank, valleyEnd[activated]);
                Raise(valleyByPeakRank, rank, peakEnd[activated]);
                activated++;
            }

            var currentRank = BinarySearch.LowerBound(sequence, nums[i]);
            var bestFromValley = currentRank > 0
                ? Math.Max(0L, peakByValleyRank.Query(0, currentRank - 1))
                : 0L;
            var bestFromPeak = currentRank < rankCount - 1
                ? Math.Max(0L, valleyByPeakRank.Query(currentRank + 1, rankCount - 1))
                : 0L;

            peakEnd[i] = nums[i] + bestFromValley;
            valleyEnd[i] = nums[i] + bestFromPeak;
            answer = Math.Max(answer, Math.Max(peakEnd[i], valleyEnd[i]));
        }

        return answer;
    }

    private static RepoSegmentTree BuildRankedTree(int rankCount)
    {
        var initial = new long[rankCount];
        Array.Fill(initial, long.MinValue);
        return new RepoSegmentTree(initial);
    }

    private static void Raise(RepoSegmentTree tree, int rank, long value)
    {
        if (value > tree.Query(rank, rank))
        {
            tree.Update(rank, value);
        }
    }
}
