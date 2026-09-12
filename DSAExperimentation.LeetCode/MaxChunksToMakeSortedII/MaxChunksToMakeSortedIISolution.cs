using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.MaxChunksToMakeSortedII;

// LeetCode 768. Max Chunks To Make Sorted II: the greatest number of contiguous
// chunks arr can be cut into so that sorting each chunk independently and
// concatenating them sorts the whole array. Unlike LC 769 (Max Chunks To Make
// Sorted), arr isn't a 0..n-1 permutation here - duplicates and arbitrary values
// are allowed, which is exactly why index-identity tricks don't work and a chunk
// boundary has to be confirmed by comparing values instead.
internal static class MaxChunksToMakeSortedIISolution
{
    // The textbook boundary check: re-scan the entire remaining suffix from scratch
    // for every candidate split point. O(n) per candidate, O(n^2) worst case.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int MaxChunksByBruteForceSuffixRescan(int[] arr)
    {
        var chunks = 0;
        var runningMax = int.MinValue;

        for (var i = 0; i < arr.Length; i++)
        {
            runningMax = Math.Max(runningMax, arr[i]);

            if (!RestIsAtLeast(arr, i + 1, runningMax))
            {
                continue;
            }

            chunks++;
            runningMax = int.MinValue;
        }

        return chunks;
    }

    private static bool RestIsAtLeast(int[] arr, int fromIndex, int threshold)
    {
        for (var j = fromIndex; j < arr.Length; j++)
        {
            if (arr[j] < threshold)
            {
                return false;
            }
        }

        return true;
    }

    // A monotonic non-decreasing Stack<int> of each closed chunk's max value
    // (DailyTemperaturesTests' Stack<int> precedent, applied here to chunk-merging
    // instead of a wait-day sweep). A value smaller than the top merges every chunk
    // whose max exceeds it into one, since all of them must now sort together with
    // it; the final stack size is the chunk count.
    public static int MaxChunksByMonotonicStackMerge(int[] arr)
    {
        var chunkMaxes = new RepoIntStack();

        foreach (var num in arr)
        {
            if (chunkMaxes.TryPeek(out var currentMax) && num < currentMax)
            {
                chunkMaxes.TryPop(out var mergedMax);

                while (chunkMaxes.TryPeek(out var previousMax) && previousMax > num)
                {
                    chunkMaxes.TryPop(out _);
                }

                chunkMaxes.Push(mergedMax);
            }
            else
            {
                chunkMaxes.Push(num);
            }
        }

        return chunkMaxes.Count;
    }
}
