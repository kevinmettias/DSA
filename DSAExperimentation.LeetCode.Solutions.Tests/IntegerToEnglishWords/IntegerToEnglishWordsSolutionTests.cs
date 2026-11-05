using DSAExperimentation.LeetCode.Solutions.IntegerToEnglishWords;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IntegerToEnglishWords;

// Harness only. Both strategies are IntegerToEnglishWordsSolution's. The catalog's
// cases (123, 12345, 1234567) use only some of the words, so this file adds a number
// for every other word the strategies spell - each of zero to nineteen, each ten, and
// each scale an int reaches - so a wrong word anywhere fails here, and LC 273's own
// hint, 1000010, where two groups in a row are silent.
public sealed partial class IntegerToEnglishWordsSolutionTests
{
    public static TheoryData<int, string> Numbers =>
        new()
        {
            { 1, "One" },
            { 2, "Two" },
            { 3, "Three" },
            { 4, "Four" },
            { 6, "Six" },
            { 7, "Seven" },
            { 8, "Eight" },
            { 12, "Twelve" },
            { 13, "Thirteen" },
            { 14, "Fourteen" },
            { 15, "Fifteen" },
            { 16, "Sixteen" },
            { 17, "Seventeen" },
            { 18, "Eighteen" },
            { 20, "Twenty" },
            { 40, "Forty" },
            { 58, "Fifty Eight" },
            { 60, "Sixty" },
            { 77, "Seventy Seven" },
            { 80, "Eighty" },
        };

    [Theory]
    [MemberData(nameof(Numbers))]
    public void NumberToWordsByStringPrepend_Number_SpellsItInLeetCodesNotation(int number, string expected) =>
        Assert.Equal(expected, IntegerToEnglishWordsSolution.NumberToWordsByStringPrepend(number));

    [Theory]
    [MemberData(nameof(Numbers))]
    public void NumberToWordsByWordStack_Number_SpellsItInLeetCodesNotation(int number, string expected) =>
        Assert.Equal(expected, IntegerToEnglishWordsSolution.NumberToWordsByWordStack(number));
}
