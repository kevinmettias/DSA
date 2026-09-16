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
    public static long MaxAlternatingSumByBruteForce(int[] nums, int minimumDistance)
    {
        var state = (Nums: nums, PeakEnd: new long[nums.Length], ValleyEnd: new long[nums.Length]);
        var answer = long.MinValue;

        for (var i = 0; i < nums.Length; i++)
        {
            var extensions = BestExtensionsBefore(state, i, i - minimumDistance);
            answer = CommitChainsAt(state, i, extensions, answer);
        }

        return answer;
    }

    // The scan at the heart of the brute-force arm: over every eligible
    // predecessor j <= lastEligible, the best valleyEnd[j] whose value is below
    // nums[index] (completing a rise into index) and the best peakEnd[j] whose
    // value is above it (completing a fall). Both start at 0 - the sum of a chain
    // that starts fresh at index.
    private static (long BestFromValley, long BestFromPeak) BestExtensionsBefore(
        (int[] Nums, long[] PeakEnd, long[] ValleyEnd) state,
        int index,
        int lastEligible)
    {
        var bestFromValley = 0L;
        var bestFromPeak = 0L;

        for (var j = 0; j <= lastEligible; j++)
        {
            if (state.Nums[j] < state.Nums[index])
            {
                bestFromValley = Math.Max(bestFromValley, state.ValleyEnd[j]);
            }
            else if (state.Nums[j] > state.Nums[index])
            {
                bestFromPeak = Math.Max(bestFromPeak, state.PeakEnd[j]);
            }
        }

        return (bestFromValley, bestFromPeak);
    }

    // Composed: coordinate-compress nums via this repo's own BinarySearch.LowerBound
    // over the sorted distinct values (the same idiom
    // MaximumBalancedSubsequenceSumSolution uses for its own value-ranked sweep),
    // then sweep left to right through two SegmentTree<long, MaxOperation<long>>
    // instances ranked by value - one holding valleyEnd (queried by rank range
    // "< nums[i]" to extend into a peak), the other holding peakEnd (queried by
    // "> nums[i]" to extend into a valley). The distance constraint is enforced by
    // delaying insertion: index j is only written into either tree once the sweep
    // reaches i = j + k, so a query at i can never see a predecessor closer than
    // minimumDistance.
    public static long MaxAlternatingSumBySegmentTree(int[] nums, int minimumDistance)
    {
        var sortedDistinct = nums.Distinct().OrderBy(value => value).ToArray();
        var sequence = new ArraySequence<int>(sortedDistinct);
        var trees = (PeakByValley: BuildRankedTree(sortedDistinct.Length), ValleyByPeak: BuildRankedTree(sortedDistinct.Length));
        var state = (Nums: nums, PeakEnd: new long[nums.Length], ValleyEnd: new long[nums.Length]);

        return SweepByValueRank(minimumDistance, sequence, trees, state);
    }

    private static RepoSegmentTree BuildRankedTree(int rankCount)
    {
        var initial = new long[rankCount];
        Array.Fill(initial, long.MinValue);
        return new RepoSegmentTree(initial);
    }

    // The ranked sweep itself: walk i left to right, first publishing every
    // predecessor the distance constraint has just freed, then extending the two
    // chains at i out of the trees.
    private static long SweepByValueRank(
        int minimumDistance,
        ArraySequence<int> sequence,
        (RepoSegmentTree PeakByValley, RepoSegmentTree ValleyByPeak) trees,
        (int[] Nums, long[] PeakEnd, long[] ValleyEnd) state)
    {
        var answer = long.MinValue;
        var activated = 0;

        for (var i = 0; i < state.Nums.Length; i++)
        {
            while (activated <= i - minimumDistance)
            {
                RaisePredecessor(state, activated, sequence, trees);
                activated++;
            }

            var extensions = BestExtensionsFromRank(state.Nums[i], sequence, trees);
            answer = CommitChainsAt(state, i, extensions, answer);
        }

        return answer;
    }

    // Publish predecessor `index` at its own value rank into both trees: its
    // valleyEnd can extend a later peak, its peakEnd a later valley.
    private static void RaisePredecessor(
        (int[] Nums, long[] PeakEnd, long[] ValleyEnd) state,
        int index,
        ArraySequence<int> sequence,
        (RepoSegmentTree PeakByValley, RepoSegmentTree ValleyByPeak) trees)
    {
        var rank = BinarySearch.LowerBound(sequence, state.Nums[index]);
        Raise(trees.PeakByValley, rank, state.ValleyEnd[index]);
        Raise(trees.ValleyByPeak, rank, state.PeakEnd[index]);
    }

    private static void Raise(RepoSegmentTree tree, int rank, long value)
    {
        if (value > tree.Query(rank, rank))
        {
            tree.Update(rank, value);
        }
    }

    // The best extension of a chain at `value` out of the ranks strictly below it
    // (a valley predecessor, completing a rise) and strictly above it (a peak
    // predecessor, completing a fall). A side with no eligible predecessor
    // contributes 0 - the sum of a chain that starts fresh at `value`.
    private static (long BestFromValley, long BestFromPeak) BestExtensionsFromRank(
        int value,
        ArraySequence<int> sequence,
        (RepoSegmentTree PeakByValley, RepoSegmentTree ValleyByPeak) trees)
    {
        var rank = BinarySearch.LowerBound(sequence, value);
        var lastRank = sequence.Length - 1;
        var lowerExtension = rank > 0 ? trees.PeakByValley.Query(0, rank - 1) : 0L;
        var bestFromValley = Math.Max(0L, lowerExtension);
        var higherExtension = rank < lastRank ? trees.ValleyByPeak.Query(rank + 1, lastRank) : 0L;
        var bestFromPeak = Math.Max(0L, higherExtension);

        return (bestFromValley, bestFromPeak);
    }

    // Both arms close an index the same way: the two chain ends at `index` are
    // nums[index] extended by the best eligible predecessor on each side, and the
    // running answer keeps the better of the two.
    private static long CommitChainsAt(
        (int[] Nums, long[] PeakEnd, long[] ValleyEnd) state,
        int index,
        (long BestFromValley, long BestFromPeak) extensions,
        long answer)
    {
        var peakEnd = state.Nums[index] + extensions.BestFromValley;
        var valleyEnd = state.Nums[index] + extensions.BestFromPeak;
        state.PeakEnd[index] = peakEnd;
        state.ValleyEnd[index] = valleyEnd;
        var bestEndHere = Math.Max(peakEnd, valleyEnd);

        return Math.Max(answer, bestEndHere);
    }
}
