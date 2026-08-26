using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class FenwickArrayTests
{
    [Fact]
    public void Get_AfterSet_ReturnsStoredValue()
    {
        var array = new FenwickArray<int>(4);

        array.Set(1, 42);

        Assert.Equal(42, array.Get(1));
    }
}
