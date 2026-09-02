using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SumOperationTests
{
    private const int FirstAddend = 3;
    private const int SecondAddend = 4;
    private const int ExpectedSum = 7;
    private const int SoleValue = 5;

    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, SumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues()
    {
        var actual = SumOperation<int>.Combine(FirstAddend, SecondAddend);
        Assert.Equal(ExpectedSum, actual);
    }

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
    {
        var actual = SumOperation<int>.Combine(SumOperation<int>.Identity, SoleValue);
        Assert.Equal(SoleValue, actual);
    }
}
