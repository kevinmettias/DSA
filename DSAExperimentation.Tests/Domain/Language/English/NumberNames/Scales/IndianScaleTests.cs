using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales.Fixtures;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Scales;

// The literals are written in the Indian grouping, a first group of three digits, then twos.
public sealed partial class IndianScaleTests
{
    [Fact]
    public void Groups_AreThreeThenTwo()
    {
        Assert.Equal(3, IndianScale.FirstGroupDigits);
        Assert.Equal(2, IndianScale.LaterGroupDigits);
    }

    [Fact]
    public void Lakh_IsAHundredThousand() => Assert.Equal(new NumberWord(1_00_000, "lakh"), IndianScale.Lakh);

    [Fact]
    public void Crore_IsAHundredLakh() => Assert.Equal(new NumberWord(1_00_00_000, "crore"), IndianScale.Crore);

    [Fact]
    public void GroupWords_AreThousandLakhCrore() =>
        Assert.Equal(["thousand", "lakh", "crore"], IndianScale.GroupWords.Select(word => word.Word));

    [Fact]
    public void GroupWords_EachNameTheirGroup() =>
        Assert.All(ScaleFacts.GroupWordValues<IndianScale>(), pair => Assert.Equal(pair.Expected, pair.Actual));
}
