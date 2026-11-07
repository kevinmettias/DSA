using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Composition;
using DSAExperimentation.Domain.Language.English.NumberNames.Scales;
using Indian = DSAExperimentation.Domain.Language.English.NumberNames.Composition.EnglishNumberNames<
    DSAExperimentation.Domain.Language.English.NumberNames.Scales.IndianScale>;
using Long = DSAExperimentation.Domain.Language.English.NumberNames.Composition.EnglishNumberNames<
    DSAExperimentation.Domain.Language.English.NumberNames.Scales.LongScale>;
using Short = DSAExperimentation.Domain.Language.English.NumberNames.Composition.EnglishNumberNames<
    DSAExperimentation.Domain.Language.English.NumberNames.Scales.ShortScale>;

namespace DSAExperimentation.Tests.Domain.Language.English.NumberNames.Composition;

// One composer, three scales: each row is a number as English says it in that scale. The Indian
// rows' literals are written in the Indian grouping, so 1_00_00_000 is one crore.
public sealed partial class EnglishNumberNamesTests
{
    public static TheoryData<long, string> ShortScaleCardinals => new()
    {
        { 0, "zero" },
        { 7, "seven" },
        { 19, "nineteen" },
        { 20, "twenty" },
        { 21, "twenty-one" },
        { 99, "ninety-nine" },
        { 100, "one hundred" },
        { 101, "one hundred one" },
        { 1_000_010, "one million ten" },
        { 2_147_483_647, "two billion one hundred forty-seven million four hundred eighty-three thousand six hundred forty-seven" },
        { -45, "minus forty-five" },
        { long.MinValue, "minus nine quintillion two hundred twenty-three quadrillion three hundred seventy-two trillion thirty-six billion eight hundred fifty-four million seven hundred seventy-five thousand eight hundred eight" },
    };

    public static TheoryData<long, string> IndianCardinals => new()
    {
        { 1_00_000, "one lakh" },
        { 1_00_00_000, "one crore" },
        { 2_14_74_83_647, "two hundred fourteen crore seventy-four lakh eighty-three thousand six hundred forty-seven" },
        { 1_00_000_00_00_000, "one lakh crore" },
        { 12_05_00_009, "twelve crore five lakh nine" },
    };

    public static TheoryData<long, string> LongScaleCardinals => new()
    {
        { 999_999, "nine hundred ninety-nine thousand nine hundred ninety-nine" },
        { 1_000_000_000, "one thousand million" },
        { 1_200_000_000, "one thousand two hundred million" },
        { 1_000_000_000_000, "one billion" },
        { 5_000_000_000_000_000_007, "five trillion seven" },
    };

    public static TheoryData<long, string> Ordinals => new()
    {
        { 0, "zeroth" },
        { 1, "first" },
        { 2, "second" },
        { 3, "third" },
        { 4, "fourth" },
        { 5, "fifth" },
        { 8, "eighth" },
        { 9, "ninth" },
        { 12, "twelfth" },
        { 18, "eighteenth" },
        { 20, "twentieth" },
        { 21, "twenty-first" },
        { 90, "ninetieth" },
        { 100, "one hundredth" },
        { 102, "one hundred second" },
        { 1_000_000, "one millionth" },
    };

    public static TheoryData<long, long, string> Fractions => new()
    {
        { 1, 2, "one half" },
        { 3, 2, "three halves" },
        { 3, 4, "three quarters" },
        { 1, 3, "one third" },
        { 2, 3, "two thirds" },
        { 5, 8, "five eighths" },
        { -2, 5, "minus two fifths" },
        { 1, 22, "one twenty-second" },
        { 3, 100, "three hundredths" },
    };

    public static TheoryData<long, long, long, string> MixedNumbers => new()
    {
        { 1, 1, 2, "one and a half" },
        { 2, 1, 3, "two and a third" },
        { 2, 1, 8, "two and an eighth" },
        { 2, 2, 3, "two and two thirds" },
        { 3, 3, 4, "three and three quarters" },
        { 5, 1, 11, "five and an eleventh" },
    };

    public static TheoryData<decimal, string> Decimals => new()
    {
        { 3.14m, "three point one four" },
        { -0.5m, "minus zero point five" },
        { 1.50m, "one point five zero" },
        { 12m, "twelve" },
        { 100.07m, "one hundred point zero seven" },
    };

    [Theory]
    [MemberData(nameof(ShortScaleCardinals))]
    public void Cardinal_ShortScale_IsEnglishsWords(long value, string words) => Assert.Equal(words, Short.Cardinal(value));

    [Theory]
    [MemberData(nameof(IndianCardinals))]
    public void Cardinal_IndianScale_ReadsLakhsAndCrores(long value, string words) => Assert.Equal(words, Indian.Cardinal(value));

    [Theory]
    [MemberData(nameof(LongScaleCardinals))]
    public void Cardinal_LongScale_ReadsAThousandMillion(long value, string words) => Assert.Equal(words, Long.Cardinal(value));

    [Theory]
    [MemberData(nameof(Ordinals))]
    public void Ordinal_Number_TakesItsLastWordsOrdinal(long value, string words) => Assert.Equal(words, Short.Ordinal(value));

    [Fact]
    public void Ordinal_Negative_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Short.Ordinal(-1));

    [Theory]
    [MemberData(nameof(Fractions))]
    public void Fraction_NumeratorAndDenominator_ReadsTheParts(long numerator, long denominator, string words) =>
        Assert.Equal(words, Short.Fraction(numerator, denominator));

    [Fact]
    public void Fraction_DenominatorBelowTwo_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Short.Fraction(1, 1));

    [Theory]
    [MemberData(nameof(MixedNumbers))]
    public void MixedNumber_WholeAndFraction_JoinsThemWithAnd(long whole, long numerator, long denominator, string words) =>
        Assert.Equal(words, Short.MixedNumber(whole, numerator, denominator));

    [Fact]
    public void MixedNumber_ImproperFraction_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Short.MixedNumber(1, 3, 2));

    [Theory]
    [MemberData(nameof(Decimals))]
    public void Decimal_Number_ReadsItsDigitsAfterThePointOneByOne(decimal value, string words) =>
        Assert.Equal(words, Short.Decimal(value));

    [Fact]
    public void Percentage_IsTheDecimalAndPercent() => Assert.Equal("twelve point five percent", Short.Percentage(12.5m));

    [Fact]
    public void Compose_WritesTheWordsInReadingOrder()
    {
        var sink = new NumberWordList();
        EnglishNumberNames<ShortScale>.Compose(1_021, ref sink);
        Assert.Equal(
            [EnglishNumberWords.One, EnglishNumberWords.Thousand, EnglishNumberWords.Twenty, EnglishNumberWords.One],
            sink.Words);
    }

    [Fact]
    public void Compose_Zero_IsTheWordZero()
    {
        var sink = new NumberWordList();
        EnglishNumberNames<IndianScale>.Compose(0, ref sink);
        Assert.Equal([EnglishNumberWords.Zero], sink.Words);
    }

    [Fact]
    public void Compose_LargestUnsignedLong_ReadsEighteenQuintillion() =>
        Assert.StartsWith("eighteen quintillion", Short.Decimal(ulong.MaxValue));
}
