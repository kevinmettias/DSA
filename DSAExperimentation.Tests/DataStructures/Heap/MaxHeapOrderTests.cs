using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.DataStructures.Heap;

public sealed partial class MaxHeapOrderTests
{
    [Fact]
    public void HasPriority_LargerCandidate_ReturnsTrue()
    {
        var hasPriority = MaxHeapOrder<int>.HasPriority(2, 1);

        Assert.True(hasPriority);
    }

    [Fact]
    public void HasPriority_SmallerCandidate_ReturnsFalse()
    {
        var hasPriority = MaxHeapOrder<int>.HasPriority(1, 2);

        Assert.False(hasPriority);
    }

    [Fact]
    public void HasPriority_EqualCandidate_ReturnsFalse()
    {
        var hasPriority = MaxHeapOrder<int>.HasPriority(1, 1);

        Assert.False(hasPriority);
    }
}
