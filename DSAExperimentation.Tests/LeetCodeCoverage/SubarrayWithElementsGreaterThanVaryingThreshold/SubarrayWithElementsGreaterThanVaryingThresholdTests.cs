using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarrayWithElementsGreaterThanVaryingThreshold;

// LeetCode 2334. Subarray With Elements Greater Than Varying Threshold: sort the
// indices by value descending (this repo's own MergeSort.Sort<Element,TSequence>
// over an ArrayIndexedSequence<int> - the same "sort an index array by a comparer
// closed over the value array" shape OddEvenJumpTests already uses), then process
// indices in that order and grow each one's window by DisjointSet.Union with any
// already-processed neighbor (GraphConnectivityWithThreshold's own DisjointSet
// precedent, just unioning adjacent indices instead of divisor multiples). Because
// values are processed non-increasing, every neighbor merged into an index's
// component already has a value >= the one being checked, so that component's
// length is always a valid window size the moment length * value exceeds threshold
// - returned immediately, since any qualifying window is an acceptable answer.
// -1 if no window ever qualifies. DisjointSet itself tracks the partition, not the
// size, so a plain int[] indexed by (unioned) root tracks each component's size
// alongside it - the same "DisjointSet plus caller-owned bookkeeping" shape a
// union-by-size policy would need if DisjointSet exposed one (ARCHITECTURE.md
// §10.1: linking policy is a complexity law, not something this type has to expose).
public sealed class SubarrayWithElementsGreaterThanVaryingThresholdTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 4, 3, 1 }, 6, 3)]
    [InlineData(new[] { 6, 5, 6, 5, 8 }, 7, 1)]
    [InlineData(new[] { 1, 1, 1, 1 }, 1000, -1)]
    public void ValidSubarraySize_LeetCodeExamples_ReturnsExpectedSize(int[] nums, int threshold, int expected)
        => Assert.Equal(expected, ValidSubarraySize(nums, threshold));

    private static int ValidSubarraySize(int[] nums, int threshold)
    {
        var n = nums.Length;
        var order = DescendingValueOrder(nums);

        var components = new DisjointSet(n);
        var size = new int[n];
        var visited = new bool[n];

        foreach (var index in order)
        {
            visited[index] = true;
            size[index] = 1;

            if (index > 0 && visited[index - 1])
            {
                MergeInto(components, size, index, index - 1);
            }

            if (index < n - 1 && visited[index + 1])
            {
                MergeInto(components, size, index, index + 1);
            }

            var root = components.Find(index);
            if ((long)size[root] * nums[index] > threshold)
            {
                return size[root];
            }
        }

        return -1;
    }

    private static int[] DescendingValueOrder(int[] nums)
    {
        var order = Enumerable.Range(0, nums.Length).ToArray();
        var byDescendingValueThenIndex = Comparer<int>.Create(
            (a, b) => nums[a] != nums[b] ? nums[b].CompareTo(nums[a]) : a.CompareTo(b));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(order), byDescendingValueThenIndex);

        return order;
    }

    private static void MergeInto(DisjointSet components, int[] size, int index, int neighbor)
    {
        var combined = size[components.Find(index)] + size[components.Find(neighbor)];
        components.Union(index, neighbor);
        size[components.Find(index)] = combined;
    }
}
