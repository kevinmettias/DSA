namespace DSAExperimentation.LeetCode.MaxChunksToMakeSorted;

// LeetCode 769. Max Chunks To Make Sorted: split a permutation of 0..n-1 into the
// maximum number of contiguous chunks that, sorted individually then concatenated
// in order, reproduce the fully sorted array.
//
// Both strategies rely on the same invariant: the prefix arr[0..i] can be cut off
// as its own chunk exactly when its running maximum equals i, because arr is a
// permutation of 0..n-1 and that equality means every value 0..i has already
// appeared somewhere in the prefix. They differ only in whether that prefix max is
// re-derived from scratch for every candidate boundary or carried forward as a
// running accumulator - the same "redo the scan vs. carry the accumulator"
// complexity split TwoSumSolution's two strategies use.
internal static class MaxChunksToMakeSortedSolution
{
    // The textbook baseline: recompute the prefix max from scratch for every
    // candidate boundary i - O(n^2), deliberately written without carrying state
    // across iterations.
    public static int MaxChunksByBruteForce(int[] arr)
    {
        var chunks = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            var prefixMax = 0;

            for (var j = 0; j <= i; j++)
            {
                prefixMax = Math.Max(prefixMax, arr[j]);
            }

            if (prefixMax == i)
            {
                chunks++;
            }
        }

        return chunks;
    }

    // Carry the running max forward across iterations instead of re-scanning - O(n).
    public static int MaxChunksByRunningMaxScan(int[] arr)
    {
        var chunks = 0;
        var runningMax = 0;

        for (var i = 0; i < arr.Length; i++)
        {
            runningMax = Math.Max(runningMax, arr[i]);

            if (runningMax == i)
            {
                chunks++;
            }
        }

        return chunks;
    }
}
