using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

public sealed partial class ThousandsGroupingTests
{
    [Fact]
    public void Groups_AreThreeThenThree()
    {
        Assert.Equal(3, ThousandsGrouping.FirstGroupDigits);
        Assert.Equal(3, ThousandsGrouping.LaterGroupDigits);
    }
}
