using DSAExperimentation.DataStructures.Searching;

namespace DSAExperimentation.Tests.DataStructures.Searching;

public sealed partial class ArraySequenceTests
{
    [Fact]
    public void Length_ReflectsBackingLength()
    {
        var sequence = new ArraySequence<int>([1, 2, 3]);

        Assert.Equal(3, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementAtIndex()
    {
        var sequence = new ArraySequence<int>([1, 2, 3]);

        var value = sequence.Get(1);

        Assert.Equal(2, value);
    }
}
