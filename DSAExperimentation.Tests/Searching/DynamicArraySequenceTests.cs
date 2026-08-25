using DSAExperimentation.Collections.DynamicArray;
using DSAExperimentation.Searching;

namespace DSAExperimentation.Tests.Searching;

public sealed partial class DynamicArraySequenceTests
{
    [Fact]
    public void Length_ReflectsBackingLength()
    {
        var items = new DynamicArray<int>();
        items.Add(1);
        items.Add(2);

        var sequence = new DynamicArraySequence<int>(items);

        Assert.Equal(2, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementAtIndex()
    {
        var items = new DynamicArray<int>();
        items.Add(1);
        items.Add(2);

        var sequence = new DynamicArraySequence<int>(items);
        var value = sequence.Get(1);

        Assert.Equal(2, value);
    }
}
