using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics.Vowels;

// The IPA marks length after the vowel: ː long, ˑ half-long, nothing for short.
public sealed partial class MonophthongTests
{
    public static TheoryData<Monophthong, string> Symbols => new()
    {
        { new(IpaVowels.CloseFrontUnrounded, VowelLength.Long), "iː" },
        { new(IpaVowels.CloseMidFrontUnrounded, VowelLength.HalfLong), "eˑ" },
        { new(IpaVowels.NearCloseNearFrontUnrounded, VowelLength.Short), "ɪ" },
    };

    [Theory]
    [MemberData(nameof(Symbols))]
    public void Symbol_IsTheQualityThenItsLengthMark(Monophthong vowel, string expected) =>
        Assert.Equal(expected, vowel.Symbol);
}
