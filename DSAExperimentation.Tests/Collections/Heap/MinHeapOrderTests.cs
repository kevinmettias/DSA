using DSAExperimentation.Collections.Heap;

namespace DSAExperimentation.Tests.Collections.Heap;

public sealed partial class MinHeapOrderTests
{
    [Fact]
    public void HasPriority_SmallerCandidate_ReturnsTrue()
    {
        var hasPriority = MinHeapOrder<int>.HasPriority(1, 2);

        Assert.True(hasPriority);
    }

    [Fact]
    public void HasPriority_LargerCandidate_ReturnsFalse()
    {
        var hasPriority = MinHeapOrder<int>.HasPriority(2, 1);

        Assert.False(hasPriority);
    }

    [Fact]
    public void HasPriority_EqualCandidate_ReturnsFalse()
    {
        var hasPriority = MinHeapOrder<int>.HasPriority(1, 1);

        Assert.False(hasPriority);
    }
}
