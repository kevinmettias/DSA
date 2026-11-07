using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

// The symbols and pairs are the fact, written out here; every number from 1 to 3,999 makes a round
// trip, and a numeral not in standard form is refused.
public sealed partial class RomanNumeralsTests
{
    public static TheoryData<RomanSymbol, char, int> NamedSymbols => new()
    {
        { RomanNumerals.One, 'I', 1 },
        { RomanNumerals.Five, 'V', 5 },
        { RomanNumerals.Ten, 'X', 10 },
        { RomanNumerals.Fifty, 'L', 50 },
        { RomanNumerals.Hundred, 'C', 100 },
        { RomanNumerals.FiveHundred, 'D', 500 },
        { RomanNumerals.Thousand, 'M', 1_000 },
    };

    public static TheoryData<RomanPair, string, int> NamedPairs => new()
    {
        { RomanNumerals.Four, "IV", 4 },
        { RomanNumerals.Nine, "IX", 9 },
        { RomanNumerals.Forty, "XL", 40 },
        { RomanNumerals.Ninety, "XC", 90 },
        { RomanNumerals.FourHundred, "CD", 400 },
        { RomanNumerals.NineHundred, "CM", 900 },
    };

    public static TheoryData<int, string> Numerals => new()
    {
        { 3, "III" },
        { 4, "IV" },
        { 9, "IX" },
        { 14, "XIV" },
        { 58, "LVIII" },
        { 444, "CDXLIV" },
        { 1_994, "MCMXCIV" },
        { 2_026, "MMXXVI" },
        { 3_999, "MMMCMXCIX" },
    };

    public static TheoryData<string> NonStandard => new() { "", "IIII", "IC", "VX", "IIV", "MMMM", "XM", "A", "iv", "IXI" };

    [Theory]
    [MemberData(nameof(NamedSymbols))]
    public void Symbol_IsItsLetterAndValue(RomanSymbol symbol, char letter, int value) =>
        Assert.Equal(new RomanSymbol(letter, value), symbol);

    [Theory]
    [MemberData(nameof(NamedPairs))]
    public void Pair_IsWrittenAsItsTextAndStandsForItsValue(RomanPair pair, string text, int value)
    {
        Assert.Equal(text, pair.Text);
        Assert.Equal(value, pair.Value);
    }

    // Each pair is a power of ten, I, X or C, written before the next two symbols up.
    [Fact]
    public void SubtractivePairs_AreEachPowerOfTenBeforeTheNextTwoSymbols()
    {
        var symbols = RomanNumerals.Symbols;
        var expected = Enumerable.Range(0, symbols.Length - 1)
            .Where(at => at % 2 == 0)
            .SelectMany(at => new[] { new RomanPair(symbols[at], symbols[at + 1]), new RomanPair(symbols[at], symbols[at + 2]) });
        Assert.Equal(expected, RomanNumerals.SubtractivePairs);
    }

    [Fact]
    public void Symbols_AreInIncreasingOrder() =>
        Assert.Equal("IVXLCDM", string.Concat(RomanNumerals.Symbols.Select(symbol => symbol.Letter)));

    [Fact]
    public void Largest_IsThreeThousandNineHundredNinetyNine() => Assert.Equal(3_999, RomanNumerals.Largest);

    [Theory]
    [MemberData(nameof(Numerals))]
    public void Write_Number_IsItsStandardNumeral(int value, string numeral) =>
        Assert.Equal(numeral, RomanNumerals.Write(value));

    [Theory]
    [MemberData(nameof(Numerals))]
    public void TryRead_StandardNumeral_IsItsNumber(int value, string numeral)
    {
        Assert.True(RomanNumerals.TryRead(numeral, out var read));
        Assert.Equal(value, read);
    }

    [Fact]
    public void WriteThenRead_EveryNumberToTheLargest_ComesBack() =>
        Assert.All(
            Enumerable.Range(1, RomanNumerals.Largest),
            value => Assert.True(RomanNumerals.TryRead(RomanNumerals.Write(value), out var read) && read == value));

    [Fact]
    public void Write_EveryNumber_RepeatsNoSymbolMoreThanThreeTimes() =>
        Assert.All(
            Enumerable.Range(1, RomanNumerals.Largest).Select(RomanNumerals.Write),
            numeral => Assert.DoesNotContain(RomanNumerals.Symbols, symbol => numeral.Contains(new string(symbol.Letter, 4))));

    [Theory]
    [MemberData(nameof(NonStandard))]
    public void TryRead_NotInStandardForm_IsFalse(string numeral) =>
        Assert.False(RomanNumerals.TryRead(numeral, out _));

    [Fact]
    public void Write_ZeroOrPastTheLargest_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RomanNumerals.Write(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => RomanNumerals.Write(RomanNumerals.Largest + 1));
    }
}
