using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales.Fixtures;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales;

public sealed partial class ShortScaleTests
{
    [Fact]
    public void Groups_AreThreeThenThree()
    {
        Assert.Equal(3, ShortScale.FirstGroupDigits);
        Assert.Equal(3, ShortScale.LaterGroupDigits);
    }

    [Fact]
    public void GroupWords_AreThousandToQuintillion() =>
        Assert.Equal(
            ["thousand", "million", "billion", "trillion", "quadrillion", "quintillion"],
            ShortScale.GroupWords.Select(word => word.Word));

    [Fact]
    public void GroupWords_EachNameTheirGroup() =>
        Assert.All(ScaleFacts.GroupWordValues<ShortScale>(), pair => Assert.Equal(pair.Expected, pair.Actual));
}
