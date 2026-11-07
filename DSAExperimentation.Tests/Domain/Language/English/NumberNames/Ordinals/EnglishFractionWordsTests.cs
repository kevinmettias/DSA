using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Ordinals;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Ordinals;

public sealed partial class EnglishFractionWordsTests
{
    [Fact]
    public void Half_IsTwosPartsHalfAndHalves() =>
        Assert.Equal(new FractionWord(EnglishNumberWords.Two, "half", "halves"), EnglishFractionWords.Half);

    [Fact]
    public void Quarter_IsFoursPartsQuarterAndQuarters() =>
        Assert.Equal(new FractionWord(EnglishNumberWords.Four, "quarter", "quarters"), EnglishFractionWords.Quarter);

    [Fact]
    public void Irregular_IsHalfThenQuarter() =>
        Assert.Equal<FractionWord>([EnglishFractionWords.Half, EnglishFractionWords.Quarter], EnglishFractionWords.Irregular);

    [Fact]
    public void SingularAndPluralOf_Two_AreHalfAndHalves()
    {
        Assert.Equal("half", EnglishFractionWords.SingularOf(2, "second"));
        Assert.Equal("halves", EnglishFractionWords.PluralOf(2, "second"));
    }

    [Fact]
    public void SingularAndPluralOf_OtherDenominator_AreItsOrdinalAndItsOrdinalWithAnS()
    {
        Assert.Equal("fifth", EnglishFractionWords.SingularOf(5, "fifth"));
        Assert.Equal("twenty-seconds", EnglishFractionWords.PluralOf(22, "twenty-second"));
    }

    [Fact]
    public void Denominators_AreTheirNumberWords() =>
        Assert.Equal([2L, 4L], EnglishFractionWords.Irregular.Select(word => word.Denominator.Value));
}
