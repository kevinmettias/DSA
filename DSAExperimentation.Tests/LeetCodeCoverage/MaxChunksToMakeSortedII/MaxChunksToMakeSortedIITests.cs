using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxChunksToMakeSortedII;

// LeetCode 768. Max Chunks To Make Sorted II: a monotonic non-decreasing
// Stack<int> of each closed chunk's max value (DailyTemperaturesTests' Stack<int>
// precedent, applied here to chunk-merging instead of a wait-day sweep). A value
// smaller than the top merges every chunk whose max exceeds it into one, since all
// of them must now sort together with it; the final stack size is the chunk count.
// Unlike LC769 (Max Chunks To Make Sorted), arr isn't a 0..n-1 permutation here -
// duplicates and arbitrary values are allowed, which is exactly why index-identity
// tricks don't work and the merge has to compare values instead.
public sealed partial class MaxChunksToMakeSortedIITests
{
    [Fact]
    public void MaxChunks_StrictlyDecreasing_ReturnsOneChunk()
    {
        int[] arr = [5, 4, 3, 2, 1];

        var chunks = MaxChunksToSorted(arr);

        Assert.Equal(1, chunks);
    }

    [Fact]
    public void MaxChunks_ClassicExampleWithDuplicates_ReturnsFourChunks()
    {
        int[] arr = [2, 1, 3, 4, 4];

        var chunks = MaxChunksToSorted(arr);

        Assert.Equal(4, chunks);
    }

    [Fact]
    public void MaxChunks_AlreadySorted_ReturnsOneChunkPerElement()
    {
        int[] arr = [1, 0, 2, 3, 4];

        var chunks = MaxChunksToSorted(arr);

        Assert.Equal(4, chunks);
    }

    private static int MaxChunksToSorted(int[] arr)
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
