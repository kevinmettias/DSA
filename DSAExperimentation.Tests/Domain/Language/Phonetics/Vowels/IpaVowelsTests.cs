using DSAExperimentation.Domain.Language.Phonetics.Vowels;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics.Vowels;

// The chart is the fact itself, so the tests spell it out: 28 symbols, row by row, each in a cell
// of its own, and some named values checked against the IPA's own names.
public sealed partial class IpaVowelsTests
{
    public static TheoryData<Vowel, Vowel> NamedVowels => new()
    {
        { IpaVowels.CloseFrontUnrounded, new("i", VowelHeight.Close, VowelBackness.Front, Rounding.Unrounded) },
        { IpaVowels.CloseFrontRounded, new("y", VowelHeight.Close, VowelBackness.Front, Rounding.Rounded) },
        {
            IpaVowels.NearCloseNearFrontUnrounded,
            new("ɪ", VowelHeight.NearClose, VowelBackness.NearFront, Rounding.Unrounded)
        },
        { IpaVowels.MidCentral, new("ə", VowelHeight.Mid, VowelBackness.Central, Rounding.Unspecified) },
        { IpaVowels.OpenMidBackUnrounded, new("ʌ", VowelHeight.OpenMid, VowelBackness.Back, Rounding.Unrounded) },
        { IpaVowels.NearOpenFrontUnrounded, new("æ", VowelHeight.NearOpen, VowelBackness.Front, Rounding.Unrounded) },
        { IpaVowels.OpenBackRounded, new("ɒ", VowelHeight.Open, VowelBackness.Back, Rounding.Rounded) },
    };

    [Theory]
    [MemberData(nameof(NamedVowels))]
    public void NamedVowel_IsItsSymbolAndCell(Vowel vowel, Vowel expected) => Assert.Equal(expected, vowel);

    [Fact]
    public void All_SpellsTheChartRowByRow() =>
        Assert.Equal("iyɨʉɯuɪʏʊeøɘɵɤoəɛœɜɞʌɔæɐaɶɑɒ", string.Concat(IpaVowels.All.Select(vowel => vowel.Symbol)));

    [Fact]
    public void All_HoldsTwentyEight() => Assert.Equal(28, IpaVowels.All.Length);

    [Fact]
    public void All_GivesEachCellOneSymbol() =>
        Assert.Equal(
            IpaVowels.All.Length,
            IpaVowels.All.Select(vowel => (vowel.Height, vowel.Backness, vowel.Rounding)).Distinct().Count());

    [Fact]
    public void All_RunsFromCloseToOpen() =>
        Assert.Equal(IpaVowels.All.OrderBy(vowel => vowel.Height), IpaVowels.All);

    // Only the two central vowels the chart gives no rounded partner leave the lips unsaid.
    [Fact]
    public void All_LeavesRoundingUnspecifiedOnlyForSchwaAndTheNearOpenCentral() =>
        Assert.Equal<string>(
            ["ə", "ɐ"],
            IpaVowels.All.Where(vowel => vowel.Rounding == Rounding.Unspecified).Select(vowel => vowel.Symbol));
}
