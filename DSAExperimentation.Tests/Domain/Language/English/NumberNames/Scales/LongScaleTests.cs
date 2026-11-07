using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales.Fixtures;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales;

public sealed partial class LongScaleTests
{
    [Fact]
    public void Groups_AreSixThenSix()
    {
        Assert.Equal(6, LongScale.FirstGroupDigits);
        Assert.Equal(6, LongScale.LaterGroupDigits);
    }

    [Fact]
    public void Billion_IsAMillionMillion() => Assert.Equal(new NumberWord(1_000_000_000_000, "billion"), LongScale.Billion);

    [Fact]
    public void Trillion_IsAMillionBillion() =>
        Assert.Equal(new NumberWord(1_000_000_000_000_000_000, "trillion"), LongScale.Trillion);

    [Fact]
    public void GroupWords_AreMillionBillionTrillion() =>
        Assert.Equal(["million", "billion", "trillion"], LongScale.GroupWords.Select(word => word.Word));

    [Fact]
    public void GroupWords_EachNameTheirGroup() =>
        Assert.All(ScaleFacts.GroupWordValues<LongScale>(), pair => Assert.Equal(pair.Expected, pair.Actual));
}
