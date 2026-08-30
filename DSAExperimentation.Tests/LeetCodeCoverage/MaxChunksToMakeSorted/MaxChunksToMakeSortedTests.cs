namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxChunksToMakeSorted;

// LeetCode 769. Max Chunks To Make Sorted: a single left-to-right pass tracking the
// running max seen so far - whenever that running max equals the current index,
// every value 0..i has appeared somewhere in arr[0..i] (arr is a permutation of
// 0..n-1), so that prefix can be its own sorted chunk. Same "greedy single-pass
// scan, no stronger reusable primitive applies beyond ordinary sequence traversal"
// shape JumpGame's own manifest entry already uses.
public sealed partial class MaxChunksToMakeSortedTests
{
    [Fact]
    public void MaxChunks_StrictlyDescending_CanOnlyFormOneChunk()
    {
        var chunks = MaxChunksToSorted([4, 3, 2, 1, 0]);

        Assert.Equal(1, chunks);
    }

    [Fact]
    public void MaxChunks_NearlySorted_SplitsIntoFourChunks()
    {
        var chunks = MaxChunksToSorted([1, 0, 2, 3, 4]);

        Assert.Equal(4, chunks);
    }

    [Fact]
    public void MaxChunks_AlreadySorted_SplitsIntoOneChunkPerElement()
    {
        var chunks = MaxChunksToSorted([0, 1, 2, 3, 4]);

        Assert.Equal(5, chunks);
    }

    private static int MaxChunksToSorted(int[] arr)
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
