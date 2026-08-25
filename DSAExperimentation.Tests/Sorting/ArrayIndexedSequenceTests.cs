using DSAExperimentation.Sorting;

namespace DSAExperimentation.Tests.Sorting;

public sealed partial class ArrayIndexedSequenceTests
{
    [Fact]
    public void Length_ReflectsBackingLength()
    {
        var sequence = new ArrayIndexedSequence<int>([1, 2, 3]);

        Assert.Equal(3, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementAtIndex()
    {
        var sequence = new ArrayIndexedSequence<int>([1, 2, 3]);

        var value = sequence.Get(1);

        Assert.Equal(2, value);
    }

    [Fact]
    public void Set_ValidIndex_OverwritesElementAtIndex()
    {
        var sequence = new ArrayIndexedSequence<int>([1, 2, 3]);

        sequence.Set(1, 42);

        Assert.Equal(42, sequence.Get(1));
    }
}
