using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.Algorithms.Searching;

public sealed partial class BinarySearchTests
{
    [Fact]
    public void Find_TargetPresent_ReturnsIndexOfTarget()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.Find(sequence, 7);

        Assert.Equal(3, index);
    }

    [Fact]
    public void Find_TargetAbsentBetweenElements_ReturnsNull()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.Find(sequence, 4);

        Assert.Null(index);
    }

    [Fact]
    public void Find_TargetBelowRange_ReturnsNull()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.Find(sequence, 0);

        Assert.Null(index);
    }

    [Fact]
    public void Find_TargetAboveRange_ReturnsNull()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.Find(sequence, 10);

        Assert.Null(index);
    }

    [Fact]
    public void Find_EmptySequence_ReturnsNull()
    {
        var sequence = new ArraySequence<int>([]);

        var index = BinarySearch.Find(sequence, 5);

        Assert.Null(index);
    }

    [Fact]
    public void Find_SingleElementSequenceMatchingTarget_ReturnsZero()
    {
        var sequence = new ArraySequence<int>([5]);

        var index = BinarySearch.Find(sequence, 5);

        Assert.Equal(0, index);
    }

    [Fact]
    public void Find_SingleElementSequenceNotMatchingTarget_ReturnsNull()
    {
        var sequence = new ArraySequence<int>([5]);

        var index = BinarySearch.Find(sequence, 3);

        Assert.Null(index);
    }

    [Fact]
    public void Find_WithCustomComparer_UsesComparerInsteadOfDefaultOrder()
    {
        var sequence = new ArraySequence<int>([9, 7, 5, 3, 1]);
        var descendingComparer = Comparer<int>.Create((left, right) => right.CompareTo(left));

        var index = BinarySearch.Find(sequence, 5, descendingComparer);

        Assert.Equal(2, index);
    }

    [Fact]
    public void Find_DuplicateTargets_ReturnsIndexOfAMatchingOccurrence()
    {
        var sequence = new ArraySequence<int>([1, 3, 3, 3, 5]);

        var index = BinarySearch.Find(sequence, 3);

        Assert.NotNull(index);
        Assert.Equal(3, sequence.Get(index.Value));
    }

    [Fact]
    public void Find_OverDynamicArraySequence_ReturnsSameResultAsArraySequence()
    {
        int[] values = [1, 3, 5, 7, 9];
        var arraySequence = new ArraySequence<int>(values);
        var dynamicArray = new DynamicArray<int>();

        foreach (var value in values)
        {
            dynamicArray.Add(value);
        }

        var dynamicArraySequence = new DynamicArraySequence<int>(dynamicArray);

        var arrayResult = BinarySearch.Find(arraySequence, 7);
        var dynamicArrayResult = BinarySearch.Find(dynamicArraySequence, 7);

        Assert.Equal(3, arrayResult);
        Assert.Equal(arrayResult, dynamicArrayResult);
    }

    [Fact]
    public void LowerBound_DuplicateTargets_ReturnsFirstOccurrence()
    {
        var sequence = new ArraySequence<int>([1, 3, 3, 3, 5]);

        var index = BinarySearch.LowerBound(sequence, 3);

        Assert.Equal(1, index);
    }

    [Fact]
    public void LowerBound_TargetAbsentBetweenElements_ReturnsInsertionIndex()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.LowerBound(sequence, 4);

        Assert.Equal(2, index);
    }

    [Fact]
    public void LowerBound_TargetAboveRange_ReturnsSequenceLength()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.LowerBound(sequence, 10);

        Assert.Equal(sequence.Length, index);
    }

    [Fact]
    public void LowerBound_TargetBelowRange_ReturnsZero()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.LowerBound(sequence, 0);

        Assert.Equal(0, index);
    }

    [Fact]
    public void LowerBound_EmptySequence_ReturnsZero()
    {
        var sequence = new ArraySequence<int>([]);

        var index = BinarySearch.LowerBound(sequence, 5);

        Assert.Equal(0, index);
    }

    [Fact]
    public void LowerBound_WithCustomComparer_UsesComparerInsteadOfDefaultOrder()
    {
        var sequence = new ArraySequence<int>([9, 7, 5, 5, 3, 1]);
        var descendingComparer = Comparer<int>.Create((left, right) => right.CompareTo(left));

        var index = BinarySearch.LowerBound(sequence, 5, descendingComparer);

        Assert.Equal(2, index);
    }

    [Fact]
    public void UpperBound_DuplicateTargets_ReturnsIndexPastLastOccurrence()
    {
        var sequence = new ArraySequence<int>([1, 3, 3, 3, 5]);

        var index = BinarySearch.UpperBound(sequence, 3);

        Assert.Equal(4, index);
    }

    [Fact]
    public void UpperBound_TargetAbsentBetweenElements_ReturnsInsertionIndex()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.UpperBound(sequence, 4);

        Assert.Equal(2, index);
    }

    [Fact]
    public void UpperBound_TargetAboveRange_ReturnsSequenceLength()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.UpperBound(sequence, 10);

        Assert.Equal(sequence.Length, index);
    }

    [Fact]
    public void UpperBound_TargetBelowRange_ReturnsZero()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var index = BinarySearch.UpperBound(sequence, 0);

        Assert.Equal(0, index);
    }

    [Fact]
    public void UpperBound_EmptySequence_ReturnsZero()
    {
        var sequence = new ArraySequence<int>([]);

        var index = BinarySearch.UpperBound(sequence, 5);

        Assert.Equal(0, index);
    }

    [Fact]
    public void UpperBound_WithCustomComparer_UsesComparerInsteadOfDefaultOrder()
    {
        var sequence = new ArraySequence<int>([9, 7, 5, 5, 3, 1]);
        var descendingComparer = Comparer<int>.Create((left, right) => right.CompareTo(left));

        var index = BinarySearch.UpperBound(sequence, 5, descendingComparer);

        Assert.Equal(4, index);
    }

    [Fact]
    public void LowerBoundAndUpperBound_TargetAbsent_ProduceEmptyRange()
    {
        var sequence = new ArraySequence<int>([1, 3, 5, 7, 9]);

        var lower = BinarySearch.LowerBound(sequence, 4);
        var upper = BinarySearch.UpperBound(sequence, 4);

        Assert.Equal(lower, upper);
    }
}
