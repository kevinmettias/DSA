using DSAExperimentation.DataStructures.DoublyLinkedList;
using DSAExperimentation.DataStructures.Heap;
using DSAExperimentation.DataStructures.Set;
using PairNode = DSAExperimentation.DataStructures.DoublyLinkedList.DoublyLinkedListNode<(long Sum, int OriginalIndex)>;

namespace DSAExperimentation.LeetCode.MinimumPairRemovalToSortArrayII;

// LeetCode 3510. Minimum Pair Removal to Sort Array II: repeatedly replace the
// adjacent pair with the smallest sum (leftmost on a tie) with their sum, until
// the array is non-decreasing. Return how many replacements that took.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MinimumPairRemovalToSortArrayIISolution
{
    // Textbook: rescan every adjacent pair for the minimum sum, merge with
    // List<long>.RemoveAt, and recheck the whole array for non-decreasing order -
    // O(n) work per operation, O(n) operations worst case. The arm the lazy-heap
    // strategy below has to beat.
    public static int MinOperationsByBruteForceScan(int[] nums)
    {
        var values = new List<long>(nums.Length);

        foreach (var value in nums)
        {
            values.Add(value);
        }

        var operations = 0;

        while (!IsNonDecreasing(values))
        {
            var mergeIndex = IndexOfLeftmostMinAdjacentSum(values);
            values[mergeIndex] += values[mergeIndex + 1];
            values.RemoveAt(mergeIndex + 1);
            operations++;
        }

        return operations;
    }

    private static bool IsNonDecreasing(List<long> values)
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

    private static int IndexOfLeftmostMinAdjacentSum(List<long> values)
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

    // Composed: the array lives in this repo's DoublyLinkedList<(long, int)>, one
    // node per surviving element (Value.OriginalIndex is the element's fixed
    // position in the input, which never changes and stays leftmost-first even as
    // neighbors merge away - the tie-break LeetCode wants for free). Candidate
    // pair sums live in a Heap<RemovalCandidate, MinHeapOrder<...>>, pushed fresh
    // whenever a pair first becomes adjacent or either side's value changes,
    // never updated in place; a popped entry is trusted only once its two nodes
    // are both still in this repo's own Set<PairNode> of live nodes, still
    // directly adjacent, and its cached sum still matches the nodes' current
    // values - the same lazy-deletion shape MostFrequentIdsSolution and
    // FindBuildingWhereAliceAndBobCanMeetSolution use a heap for. A running count
    // of descending adjacent pairs (updated incrementally around every merge)
    // says when the array has become non-decreasing without ever rescanning it.
    // Amortized O(log n) per operation.
    public static int MinOperationsByLazyHeap(int[] nums)
    {
        var n = nums.Length;

        if (n < 2)
        {
            return 0;
        }

        var order = new PairNode[n];
        var alive = new Set<PairNode>();
        var list = BuildList(nums, order, alive);
        var heap = new Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>>();
        var descents = SeedCandidates(order, heap);

        return MergeUntilSorted(list, alive, heap, descents);
    }

    // The starting list: one node per element, linked in input order, each recorded at
    // its own index in `order` and registered as alive.
    private static DoublyLinkedList<(long Sum, int OriginalIndex)> BuildList(
        int[] nums, PairNode[] order, Set<PairNode> alive)
    {
        var list = new DoublyLinkedList<(long Sum, int OriginalIndex)>();

        for (var i = nums.Length - 1; i >= 0; i--)
        {
            var node = new PairNode { Value = (nums[i], i) };
            list.AddFront(node);
            order[i] = node;
            alive.TryAdd(node);
        }

        return list;
    }

    // A candidate for every initially adjacent pair, pushed into `heap`, plus how many
    // of those pairs descend.
    private static int SeedCandidates(
        PairNode[] order, Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>> heap)
    {
        var descents = 0;

        for (var i = 0; i < order.Length - 1; i++)
        {
            var left = order[i];
            var right = order[i + 1];
            heap.Push(new RemovalCandidate(left.Value.Sum + right.Value.Sum, i, left, right));

            if (left.Value.Sum > right.Value.Sum)
            {
                descents++;
            }
        }

        return descents;
    }

    // Merges the current smallest-sum adjacent pair until nothing descends any more,
    // reporting how many merges that took.
    private static int MergeUntilSorted(
        DoublyLinkedList<(long Sum, int OriginalIndex)> list,
        Set<PairNode> alive,
        Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>> heap,
        int descents)
    {
        var operations = 0;

        while (descents > 0)
        {
            var candidate = PopCurrentCandidate(heap, alive);
            descents += MergeCandidate(candidate, list, alive, heap);
            operations++;
        }

        return operations;
    }

    // A popped candidate is only ever trusted once both its nodes are still
    // alive, still directly adjacent, and its cached sum still matches their
    // current values - anything else is a stale entry left by an earlier merge
    // and is simply discarded. The correctness argument for never running dry
    // here: whenever a pair becomes adjacent (initial build, or as the result of
    // a merge) or a node's value changes (the survivor of a merge), a fresh entry
    // for that exact pair is pushed immediately, so every currently-adjacent pair
    // always has at least one valid entry sitting in the heap.
    private static RemovalCandidate PopCurrentCandidate(
        Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>> heap, Set<PairNode> alive)
    {
        while (heap.TryPop(out var candidate))
        {
            if (IsCurrent(candidate, alive))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("No adjacent pair remained while the array was still out of order.");
    }

    private static bool IsCurrent(RemovalCandidate candidate, Set<PairNode> alive) =>
        alive.Has(candidate.Left) && alive.Has(candidate.Right) &&
        ReferenceEquals(candidate.Left.Next, candidate.Right) &&
        candidate.Sum == candidate.Left.Value.Sum + candidate.Right.Value.Sum;

    // Merges Left and Right into Left (Right is removed), pushes fresh candidates
    // for Left's new neighbors, and returns the net change to the descending-pair
    // count so the caller can keep it current without rescanning the array.
    private static int MergeCandidate(
        RemovalCandidate candidate, DoublyLinkedList<(long Sum, int OriginalIndex)> list,
        Set<PairNode> alive, Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>> heap)
    {
        var left = candidate.Left;
        var right = candidate.Right;
        var previous = left.Previous!;
        var next = right.Next!;
        var hasPrevious = alive.Has(previous);
        var hasNext = alive.Has(next);

        var descentDelta = 0;
        descentDelta -= hasPrevious && previous.Value.Sum > left.Value.Sum ? 1 : 0;
        descentDelta -= left.Value.Sum > right.Value.Sum ? 1 : 0;
        descentDelta -= hasNext && right.Value.Sum > next.Value.Sum ? 1 : 0;

        left.Value = (left.Value.Sum + right.Value.Sum, left.Value.OriginalIndex);
        list.Remove(right);
        alive.TryRemove(right);

        descentDelta += hasPrevious && previous.Value.Sum > left.Value.Sum ? 1 : 0;
        descentDelta += hasNext && left.Value.Sum > next.Value.Sum ? 1 : 0;

        PushRefreshedCandidates((left, previous, next), alive, heap);

        return descentDelta;
    }

    // Fresh candidates for the pairs the merge just created, so every currently
    // adjacent pair always has at least one live entry sitting in the heap. Ends of the
    // surviving node that are not alive are the list's own bounds, and get no candidate.
    private static void PushRefreshedCandidates(
        (PairNode Left, PairNode Previous, PairNode Next) neighbors,
        Set<PairNode> alive, Heap<RemovalCandidate, MinHeapOrder<RemovalCandidate>> heap)
    {
        var previous = neighbors.Previous;
        var next = neighbors.Next;
        var left = neighbors.Left;

        if (alive.Has(previous))
        {
            heap.Push(new RemovalCandidate(
                previous.Value.Sum + left.Value.Sum, previous.Value.OriginalIndex, previous, left));
        }

        if (alive.Has(next))
        {
            heap.Push(new RemovalCandidate(left.Value.Sum + next.Value.Sum, left.Value.OriginalIndex, left, next));
        }
    }

    // A witness over two nodes of the shared DoublyLinkedList, ranked by sum then
    // by the leftmost node's fixed original index - the tie-break LeetCode wants.
    // Comparison deliberately never touches Left/Right themselves: neither
    // PairNode implements IComparable, and Sum plus OriginalIndex is already
    // enough to total-order every candidate this problem ever produces.
    private readonly record struct RemovalCandidate(long Sum, int LeftIndex, PairNode Left, PairNode Right)
        : IComparable<RemovalCandidate>
    {
        public int CompareTo(RemovalCandidate other)
        {
            var bySum = Sum.CompareTo(other.Sum);
            return bySum != 0 ? bySum : LeftIndex.CompareTo(other.LeftIndex);
        }
    }
}
