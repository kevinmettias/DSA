using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

public sealed partial class IndianGroupingTests
{
    [Fact]
    public void Groups_AreThreeThenTwo()
    {
        Assert.Equal(3, IndianGrouping.FirstGroupDigits);
        Assert.Equal(2, IndianGrouping.LaterGroupDigits);
    }
}
