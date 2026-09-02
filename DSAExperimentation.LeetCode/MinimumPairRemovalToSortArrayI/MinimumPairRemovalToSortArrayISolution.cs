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

        var values = (int[])nums.Clone();
        var next = new int[n];
        var prev = new int[n];
        var alive = new bool[n];
        var heap = new Heap<PairCandidate, MinHeapOrder<PairCandidate>>();
        var violations = 0;

        for (var i = 0; i < n; i++)
        {
            next[i] = i + 1 < n ? i + 1 : -1;
            prev[i] = i - 1 >= 0 ? i - 1 : -1;
            alive[i] = true;

            if (i < n - 1)
            {
                heap.Push(new PairCandidate(values[i] + values[i + 1], i));

                if (values[i] > values[i + 1])
                {
                    violations++;
                }
            }
        }

        var operations = 0;

        while (violations > 0 && heap.TryPop(out var candidate))
        {
            if (!alive[candidate.LeftIndex])
            {
                continue;
            }

            var right = next[candidate.LeftIndex];

            if (right < 0 || values[candidate.LeftIndex] + values[right] != candidate.Sum)
            {
                continue;
            }

            var left = candidate.LeftIndex;
            violations -= BoundaryViolations(values, prev[left], left, right, next[right]);

            values[left] += values[right];
            alive[right] = false;

            var after = next[right];
            next[left] = after;

            if (after >= 0)
            {
                prev[after] = left;
            }

            violations += MergedNeighborViolations(values, prev[left], left, after, heap);
            operations++;
        }

        return operations;
    }

    // The inversion contribution of the (up to) three pairs a merge is about
    // to erase - (before, left), (left, right) itself, and (right, after) -
    // read before the merge changes either endpoint's value.
    private static int BoundaryViolations(int[] values, int before, int left, int right, int after)
    {
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
        int[] values, int before, int merged, int after, Heap<PairCandidate, MinHeapOrder<PairCandidate>> heap)
    {
        var added = 0;

        if (before >= 0)
        {
            heap.Push(new PairCandidate(values[before] + values[merged], before));

            if (values[before] > values[merged])
            {
                added++;
            }
        }

        if (after >= 0)
        {
            heap.Push(new PairCandidate(values[merged] + values[after], merged));

            if (values[merged] > values[after])
            {
                added++;
            }
        }

        return added;
    }
}
