using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.LastStoneWeight;

// LeetCode 1046. Last Stone Weight: repeatedly smash the two heaviest stones
// together - equal stones both shatter, unequal ones leave a stone of their
// difference - and report the last remaining stone's weight, or 0 if none is left.
//
// The smash rule only ever asks for the two current maxima, so the whole problem is
// "repeated extract-max with re-insertion". The baseline rescans the whole
// remaining list for both maxima every smash; the composed strategy hands that job
// to this repo's own Heap<int, MaxHeapOrder<int>>.
internal static class LastStoneWeightSolution
{
    // Baseline: an O(stones) rescan per smash, O(stones^2) overall. Deliberately
    // plain BCL - this is what you would write without this repo.
    public static int LastStoneWeightByLinearRescan(int[] stones)
    {
        var remaining = new List<int>(stones);

        while (remaining.Count > 1)
        {
            var firstIndex = IndexOfLargest(remaining, -1);
            var secondIndex = IndexOfLargest(remaining, firstIndex);
            var difference = remaining[firstIndex] - remaining[secondIndex];

            var higherIndex = Math.Max(firstIndex, secondIndex);
            remaining.RemoveAt(higherIndex);

            var lowerIndex = Math.Min(firstIndex, secondIndex);
            remaining.RemoveAt(lowerIndex);

            if (difference != 0)
            {
                remaining.Add(difference);
            }
        }

        return remaining.Count == 0 ? 0 : remaining[0];
    }

    private static int IndexOfLargest(List<int> values, int excludeIndex)
    {
        var best = -1;

        for (var i = 0; i < values.Count; i++)
        {
            if (i != excludeIndex && (best == -1 || values[i] > values[best]))
            {
                best = i;
            }
        }

        return best;
    }

    // Composed: a max-heap offers up the heaviest remaining stone in O(log stones),
    // so the whole smash sequence costs O(stones log stones).
    public static int LastStoneWeightByMaxHeap(int[] stones)
    {
        var heap = new Heap<int, MaxHeapOrder<int>>();

        foreach (var stone in stones)
        {
            heap.Push(stone);
        }

        while (heap.Count > 1)
        {
            heap.TryPop(out var heaviest);
            heap.TryPop(out var second);

            if (heaviest != second)
            {
                heap.Push(heaviest - second);
            }
        }

        heap.TryPeek(out var remaining);
        return heap.Count == 0 ? 0 : remaining;
    }
}
