using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayI;

// LeetCode 3507. Minimum Pair Removal to Sort Array I: repeatedly merge the
// adjacent pair with the smallest sum (leftmost pair on a tie) into their sum,
// until the array is non-decreasing; return how many merges that took.
internal static class MinimumPairRemovalToSortArrayISolution
{
    // The textbook answer: rescan the whole array for the minimum-sum adjacent
    // pair, and separately rescan it for sortedness, once per round -
    // deliberately without this repo's Heap, the arm the lazy-deletion heap
    // strategy below has to justify itself against.
    public static int MinOperationsByBruteForce(int[] nums)
    {
        var values = new List<int>(nums);
        var operations = 0;

        while (!IsSorted(values))
        {
            var mergeAt = IndexOfMinimumSumPair(values);
            values[mergeAt] += values[mergeAt + 1];
            values.RemoveAt(mergeAt + 1);
            operations++;
        }

        return operations;
    }

    private static bool IsSorted(List<int> values)
    {
        for (var i = 1; i < values.Count; i++)
        {
            if (values[i - 1] > values[i])
            {
                return false;
            }
        }

        return true;
    }

    private static int IndexOfMinimumSumPair(List<int> values)
    {
        var bestIndex = 0;
        var bestSum = values[0] + values[1];

        for (var i = 1; i < values.Count - 1; i++)
        {
            var sum = values[i] + values[i + 1];

            if (sum < bestSum)
            {
                bestSum = sum;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // This repo's own Heap<PairCandidate, MinHeapOrder<PairCandidate>> over
    // every currently-live adjacent pair's sum, plus a next/prev index over the
    // original positions (a merge only ever fuses two survivors, never
    // reorders them, so an original index stays a valid "how far left" tie-
    // break key for the array's whole lifetime) and a running count of
    // adjacent inversions so "is it sorted" is a field read instead of a full
    // rescan. Heap has no decrease-key/remove (see Heap.cs), so a merge pushes
    // two fresh candidates for its new boundary pairs and lets a stale pop of
    // an already-consumed pair be discarded lazily - the same shape
    // DesignTaskManagerSolution.TaskManagerByLazyDeletionHeap already uses.
    public static int MinOperationsByLazyPairHeap(int[] nums)
    {
        var n = nums.Length;

        if (n <= 1)
        {
            return 0;
        }

        var list = (Values: (int[])nums.Clone(), Next: new int[n], Prev: new int[n], Alive: new bool[n]);
        var heap = new Heap<PairCandidate, MinHeapOrder<PairCandidate>>();
        var violations = FillInitialLinks(list, heap);
        var operations = 0;

        while (violations > 0 && heap.TryPop(out var candidate))
        {
            var applied = ApplyCandidate(list, candidate, heap);

            violations += applied.Violations;
            operations += applied.Merges;
        }

        return operations;
    }

    // Every original position's place in the live list - its neighbour on either side, or
    // -1 where that side has none - and every adjacent pair seeded into the heap. Returns
    // how many of those pairs start out as an inversion, which is "is it sorted" as a count.
    private static int FillInitialLinks(
        (int[] Values, int[] Next, int[] Prev, bool[] Alive) list,
        Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        var n = list.Values.Length;
        var violations = 0;

        for (var i = 0; i < n; i++)
        {
            var hasSuccessor = i + 1 < n;
            var hasPredecessor = i - 1 >= 0;

            list.Next[i] = hasSuccessor ? SuccessorOf(i) : -1;
            list.Prev[i] = hasPredecessor ? PredecessorOf(i) : -1;
            list.Alive[i] = true;

            if (hasSuccessor)
            {
                violations += SeedPair(list.Values, heap, i);
            }
        }

        return violations;
    }

    // The index one along from `index`, and the one before it - what a live next/prev
    // link points at when that neighbour exists at all.
    private static int SuccessorOf(int index) => index + 1;

    private static int PredecessorOf(int index) => index - 1;

    // The adjacent pair starting at `index`, pushed as the heap candidate for that
    // boundary and returned as 1 when the two values are themselves an inversion.
    private static int SeedPair(
        int[] values, Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap, int index)
    {
        heap.Push(new PairCandidate(values[index] + values[index + 1], index));

        var inverted = values[index] > values[index + 1];

        return inverted ? 1 : 0;
    }

    // One candidate popped from the heap, resolved against the live list: a stale
    // candidate changes nothing, and a live one merges its pair and reports how the
    // adjacent-inversion count moved.
    private static (int Violations, int Merges) ApplyCandidate(
        (int[] Values, int[] Next, int[] Prev, bool[] Alive) list,
        PairCandidate candidate,
        Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        var left = candidate.LeftIndex;
        var right = list.Next[left];

        if (!IsLivePair(list, left, right, candidate.Sum))
        {
            return (0, 0);
        }

        return MergeAndCount(list, left, right, heap);
    }

    // A candidate is live while an earlier merge has not consumed its left endpoint and
    // the pair it stored is still the one sitting there: the same right neighbour, the
    // same sum. `right` is -1 past the end of the list, and the sum is read only beyond
    // that guard.
    private static bool IsLivePair(
        (int[] Values, int[] Next, int[] Prev, bool[] Alive) list, int left, int right, int sum)
    {
        var inRange = list.Alive[left] && right >= 0;

        return inRange && list.Values[left] + list.Values[right] == sum;
    }

    // Fuse `right` into `left`, re-thread the live links around the pair, and report how
    // the adjacent-inversion count moved: down by the pairs the merge erased, up by the
    // new pairs the merged node creates. The erased ones are read before the merge
    // changes either endpoint's value.
    private static (int Violations, int Merges) MergeAndCount(
        (int[] Values, int[] Next, int[] Prev, bool[] Alive) list,
        int left, int right,
        Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        var removed = BoundaryViolations(list.Values, (list.Prev[left], list.Next[right]), left, right);

        list.Values[left] += list.Values[right];
        list.Alive[right] = false;

        var after = list.Next[right];
        list.Next[left] = after;

        if (after >= 0)
        {
            list.Prev[after] = left;
        }

        var added = MergedNeighborViolations(list.Values, left, (list.Prev[left], after), heap);

        return (added - removed, 1);
    }

    // The inversion contribution of the (up to) three pairs a merge is about
    // to erase - (before, left), (left, right) itself, and (right, after) -
    // read before the merge changes either endpoint's value.
    // `neighbors` is the two live positions flanking the pair being merged - either of
    // which is -1 at that end of the list, and each read only under that guard.
    private static int BoundaryViolations(int[] values, (int Before, int After) neighbors, int left, int right)
    {
        var (before, after) = neighbors;
        var removed = 0;

        if (before >= 0 && values[before] > values[left])
        {
            removed++;
        }

        if (values[left] > values[right])
        {
            removed++;
        }

        if (after >= 0 && values[right] > values[after])
        {
            removed++;
        }

        return removed;
    }

    // The merged node's (up to two) new neighbor pairs - (before, merged) and
    // (merged, after) - each pushed as a fresh heap candidate and counted if
    // it is itself an inversion under the post-merge values.
    private static int MergedNeighborViolations(
        int[] values, int merged, (int Before, int After) neighbors, Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        var (before, after) = neighbors;

        return CountInversion(values, before, merged, heap) + CountInversion(values, merged, after, heap);
    }

    // One adjacent pair - (left, right) at the positions given - pushed as a fresh heap
    // candidate for the pair starting at `left`, and returned as 1 when the two values
    // are themselves an inversion. A side with no live neighbour is -1, and contributes
    // no candidate and no inversion.
    private static int CountInversion(
        int[] values, int left, int right, Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        if (left < 0 || right < 0)
        {
            return 0;
        }

        heap.Push(new PairCandidate(values[left] + values[right], left));

        var inverted = values[left] > values[right];

        return inverted ? 1 : 0;
    }
}
