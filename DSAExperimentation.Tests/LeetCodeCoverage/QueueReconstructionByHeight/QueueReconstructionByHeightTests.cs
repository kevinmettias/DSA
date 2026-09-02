using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueueReconstructionByHeight;

// LeetCode 406. Queue Reconstruction by Height: sort people tallest-first
// (ties broken by k ascending) via this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence - the same
// custom-comparer shape RussianDollEnvelopesTests.cs already exercises - then
// insert each person into a DynamicArray<Element> at index k. Every
// already-placed person is at least as tall, so k always lands exactly where
// the LeetCode rule requires, and every later, shorter insertion leaves it
// undisturbed.
public sealed partial class QueueReconstructionByHeightTests
{
    [Fact]
    public void ReconstructQueue_LeetCodeExample_ReturnsValidOrdering()
    {
        int[][] people = [[7, 0], [4, 4], [7, 1], [5, 0], [6, 1], [5, 2]];

        Assert.Equal(
            [[5, 0], [7, 0], [5, 2], [6, 1], [4, 4], [7, 1]],
            ReconstructQueue(people));
    }

    [Fact]
    public void ReconstructQueue_SecondExample_ReturnsValidOrdering()
    {
        int[][] people = [[6, 0], [5, 0], [4, 0], [3, 2], [2, 2], [1, 4]];

        Assert.Equal(
            [[4, 0], [5, 0], [2, 2], [3, 2], [1, 4], [6, 0]],
            ReconstructQueue(people));
    }

    private static int[][] ReconstructQueue(int[][] people)
    {
        var items = SortedItems(people);
        var queue = BuildQueue(items);
        return ToResultArray(queue);
    }

    private static (int Height, int K)[] SortedItems(int[][] people)
    {
        var items = people.Select(p => (Height: p[0], K: p[1])).ToArray();
        var byHeightDescendingThenKAscending = Comparer<(int Height, int K)>.Create(
            (a, b) => a.Height != b.Height ? b.Height.CompareTo(a.Height) : a.K.CompareTo(b.K));

        MergeSort.Sort<(int Height, int K), ArrayIndexedSequence<(int Height, int K)>>(
            new ArrayIndexedSequence<(int Height, int K)>(items), byHeightDescendingThenKAscending);

        return items;
    }

    private static DynamicArray<(int Height, int K)> BuildQueue((int Height, int K)[] items)
    {
        var queue = new DynamicArray<(int Height, int K)>();

        foreach (var person in items)
        {
            queue.Insert(person.K, person);
        }

        return queue;
    }

    private static int[][] ToResultArray(DynamicArray<(int Height, int K)> queue)
    {
        var result = new int[queue.Count][];

        for (var i = 0; i < queue.Count; i++)
        {
            var (height, k) = queue.Get(i);
            result[i] = [height, k];
        }

        return result;
    }
}
