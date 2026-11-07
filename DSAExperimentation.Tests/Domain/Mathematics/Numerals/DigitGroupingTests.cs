using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.Domain.Mathematics.Numerals;

// Ten million in each grouping: the West's 10,000,000, India's 1,00,00,000 and the myriads'
// 1000,0000. The C# literals below are written in the grouping each row states.
public sealed partial class DigitGroupingTests
{
    public static TheoryData<long, string> Thousands => new()
    {
        { 0, "0" },
        { 999, "999" },
        { 1_000, "1,000" },
        { 10_000_000, "10,000,000" },
        { -1_234_567, "-1,234,567" },
        { long.MinValue, "-9,223,372,036,854,775,808" },
    };

    public static TheoryData<long, string> Indian => new()
    {
        { 999, "999" },
        { 1_000, "1,000" },
        { 1_00_000, "1,00,000" },
        { 1_00_00_000, "1,00,00,000" },
        { 2_14_74_83_647, "2,14,74,83,647" },
    };

    public static TheoryData<long, string> Myriads => new()
    {
        { 9999, "9999" },
        { 1_0000, "1,0000" },
        { 1000_0000, "1000,0000" },
        { 1_0000_0000, "1,0000,0000" },
    };

    [Theory]
    [MemberData(nameof(Thousands))]
    public void Write_ThousandsGrouping_GroupsByThree(long value, string numeral) =>
        Assert.Equal(numeral, DigitGrouping<ThousandsGrouping>.Write(value, ','));

    [Theory]
    [MemberData(nameof(Indian))]
    public void Write_IndianGrouping_GroupsByThreeThenTwo(long value, string numeral) =>
        Assert.Equal(numeral, DigitGrouping<IndianGrouping>.Write(value, ','));

    [Theory]
    [MemberData(nameof(Myriads))]
    public void Write_MyriadGrouping_GroupsByFour(long value, string numeral) =>
        Assert.Equal(numeral, DigitGrouping<MyriadGrouping>.Write(value, ','));

    [Fact]
    public void Write_Separator_IsTheCallers() =>
        Assert.Equal("1 000 000", DigitGrouping<ThousandsGrouping>.Write(1_000_000, ' '));
}
