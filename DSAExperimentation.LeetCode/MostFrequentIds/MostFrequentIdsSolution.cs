using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MostFrequentIds;

// LeetCode 3092. Most Frequent IDs: nums[i]/freq[i] add or remove copies of an
// id from a running collection; after each step report the highest per-id
// count currently in the collection. Both strategies answer the same question
// with the same signature, so the test harness can assert they agree and the
// benchmark harness can time them against each other without either restating
// the algorithm.
internal static class MostFrequentIdsSolution
{
    // Textbook: track every id's live count in a BCL Dictionary and rescan
    // every value after each step to find the current maximum. O(n) per step,
    // O(n^2) overall - the arm the lazy-heap strategy has to beat.
    public static long[] MostFrequentCountsByBruteForce(int[] nums, int[] freq)
    {
        var counts = new Dictionary<int, long>();
        var answer = new long[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            counts.TryGetValue(nums[i], out var current);
            counts[nums[i]] = current + freq[i];

            var max = 0L;

            foreach (var count in counts.Values)
            {
                if (count > max)
                {
                    max = count;
                }
            }

            answer[i] = max;
        }

        return answer;
    }

    // Composed: this repo's own HashMap<int, long> tracks each id's live
    // count, and a Heap<(long Count, int Id), MaxHeapOrder<...>>
    // (DataStructures/Heap/Heap.cs) is pushed a fresh entry on every step
    // rather than ever being updated in place. The root is trusted only once
    // its stored count still matches the id's live count in the map; a root
    // left stale by a later update to the same id is popped and discarded
    // before anything is trusted - the same lazy-deletion shape
    // FindBuildingWhereAliceAndBobCanMeetSolution uses a heap for. Amortized
    // O(log n) per step.
    public static long[] MostFrequentCountsByLazyHeap(int[] nums, int[] freq)
    {
        var counts = new HashMap<int, long>();
        var heap = new Heap<(long Count, int Id), MaxHeapOrder<(long, int)>>();
        var answer = new long[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            counts.TryGetValue(nums[i], out var current);
            var updated = current + freq[i];
            counts.Set(nums[i], updated);
            heap.Push((updated, nums[i]));

            while (heap.TryPeek(out var top) && !IsLive(counts, top))
            {
                heap.TryPop(out _);
            }

            answer[i] = heap.TryPeek(out var live) ? live.Count : 0;
        }

        return answer;
    }

    private static bool IsLive(HashMap<int, long> counts, (long Count, int Id) entry) =>
        counts.TryGetValue(entry.Id, out var current) && current == entry.Count;
}
