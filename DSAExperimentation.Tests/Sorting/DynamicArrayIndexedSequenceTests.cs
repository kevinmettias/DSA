using DSAExperimentation.Collections.DynamicArray;
using DSAExperimentation.Sorting;

namespace DSAExperimentation.Tests.Sorting;

public sealed partial class DynamicArrayIndexedSequenceTests
{
    [Fact]
    public void Length_ReflectsBackingLength()
    {
        var items = new DynamicArray<int>();
        items.Add(1);
        items.Add(2);

        var sequence = new DynamicArrayIndexedSequence<int>(items);

        Assert.Equal(2, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementAtIndex()
    {
        var items = new DynamicArray<int>();
        items.Add(1);
        items.Add(2);

        var sequence = new DynamicArrayIndexedSequence<int>(items);
        var value = sequence.Get(1);

        Assert.Equal(2, value);
    }

    [Fact]
    public void Set_ValidIndex_OverwritesElementAtIndexInBackingDynamicArray()
    {
        var items = new DynamicArray<int>();
        items.Add(1);
        items.Add(2);

        var sequence = new DynamicArrayIndexedSequence<int>(items);
        sequence.Set(1, 42);

        Assert.Equal(42, items.Get(1));
    }
}
