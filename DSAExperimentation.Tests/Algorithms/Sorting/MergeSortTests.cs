using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.Algorithms.Sorting;

public sealed partial class MergeSortTests
{
    [Fact]
    public void Sort_UnsortedSequence_SortsAscending()
    {
        var sequence = new ArrayIndexedSequence<int>([5, 3, 8, 1, 9, 2, 7]);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence);

        Assert.Equal([1, 2, 3, 5, 7, 8, 9], ToArray(sequence));
    }

    [Fact]
    public void Sort_AlreadySortedSequence_RemainsSorted()
    {
        var sequence = new ArrayIndexedSequence<int>([1, 2, 3, 4, 5]);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence);

        Assert.Equal([1, 2, 3, 4, 5], ToArray(sequence));
    }

    [Fact]
    public void Sort_EmptySequence_DoesNotThrow()
    {
        var sequence = new ArrayIndexedSequence<int>([]);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence);

        Assert.Empty(ToArray(sequence));
    }

    [Fact]
    public void Sort_SingleElementSequence_RemainsUnchanged()
    {
        var sequence = new ArrayIndexedSequence<int>([5]);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence);

        Assert.Equal([5], ToArray(sequence));
    }

    [Fact]
    public void Sort_WithDuplicates_SortsAllOccurrences()
    {
        var sequence = new ArrayIndexedSequence<int>([3, 1, 3, 1, 3]);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence);

        Assert.Equal([1, 1, 3, 3, 3], ToArray(sequence));
    }

    [Fact]
    public void Sort_WithCustomComparer_SortsDescending()
    {
        var sequence = new ArrayIndexedSequence<int>([5, 3, 8, 1, 9]);
        var descendingComparer = Comparer<int>.Create((left, right) => right.CompareTo(left));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(sequence, descendingComparer);

        Assert.Equal([9, 8, 5, 3, 1], ToArray(sequence));
    }

    [Fact]
    public void Sort_OverDynamicArrayIndexedSequence_ProducesSameResultAsArrayIndexedSequence()
    {
        int[] values = [5, 3, 8, 1, 9, 2, 7];
        var arraySequence = new ArrayIndexedSequence<int>((int[])values.Clone());

        var dynamicArray = new DynamicArray<int>();
        foreach (var value in values)
        {
            dynamicArray.Add(value);
        }

        var dynamicArraySequence = new DynamicArrayIndexedSequence<int>(dynamicArray);

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(arraySequence);
        MergeSort.Sort<int, DynamicArrayIndexedSequence<int>>(dynamicArraySequence);

        Assert.Equal(ToArray(arraySequence), ToArray(dynamicArraySequence));
    }

    private static int[] ToArray<TSequence>(TSequence sequence)
        where TSequence : struct, IIndexedSequence<int>
    {
        var result = new int[sequence.Length];

        for (var i = 0; i < sequence.Length; i++)
        {
            result[i] = sequence.Get(i);
        }

        return result;
    }
}
