using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.Domain.Language.English;

public sealed partial class LetterNumbersTests
{
    public static TheoryData<char, int> Letters => new()
    {
        { 'a', 1 },
        { 'b', 2 },
        { 'j', 10 },
        { 'y', 25 },
        { 'z', 26 },
    };

    public static TheoryData<char> NonLetters => new() { 'A', '`', '{', '1' };

    [Fact]
    public void Last_IsTheNumberOfZ() => Assert.Equal(26, LetterNumbers.Last);

    [Theory]
    [MemberData(nameof(Letters))]
    public void NumberOfLowercase_Letter_CountsFromOneAtA(char letter, int number) =>
        Assert.Equal(number, LetterNumbers.NumberOfLowercase(letter));

    [Theory]
    [MemberData(nameof(NonLetters))]
    public void NumberOfLowercase_NotALowercaseLetter_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => LetterNumbers.NumberOfLowercase(letter));
}
