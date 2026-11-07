using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

public sealed partial class MyriadGroupingTests
{
    [Fact]
    public void Groups_AreFourThenFour()
    {
        Assert.Equal(4, MyriadGrouping.FirstGroupDigits);
        Assert.Equal(4, MyriadGrouping.LaterGroupDigits);
    }
}
