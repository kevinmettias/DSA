using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.DataStructures.Sequence;

public sealed partial class OffsetSequenceTests
{
    [Fact]
    public void Length_ReflectsConstructedLength()
    {
        var sequence = new OffsetSequence<int>([1, 2, 3, 4, 5], start: 1, length: 3);

        Assert.Equal(3, sequence.Length);
    }

    [Fact]
    public void Get_ValidIndex_ReturnsElementOffsetFromStart()
    {
        var sequence = new OffsetSequence<int>([1, 2, 3, 4, 5], start: 1, length: 3);

        var value = sequence.Get(1);

        Assert.Equal(3, value); // items[start + index] = items[1 + 1] = items[2]
    }

    [Fact]
    public void Get_ZeroStart_BehavesLikeWholeArray()
    {
        var sequence = new OffsetSequence<int>([10, 20, 30], start: 0, length: 3);

        Assert.Equal(20, sequence.Get(1));
    }

    [Fact]
    public void Get_LastIndexOfWindow_ReturnsFinalElementOfWindow()
    {
        var sequence = new OffsetSequence<int>([1, 2, 3, 4, 5], start: 2, length: 3);

        Assert.Equal(5, sequence.Get(2));
    }
}
