using DSAExperimentation.Domain.Devices.Telephone;

namespace DSAExperimentation.Tests.Domain.Devices.Telephone;

public sealed partial class PhoneKeypadTests
{
    public static TheoryData<char, string> LetteredKeys => new()
    {
        { '2', "abc" },
        { '3', "def" },
        { '4', "ghi" },
        { '5', "jkl" },
        { '6', "mno" },
        { '7', "pqrs" },
        { '8', "tuv" },
        { '9', "wxyz" },
    };

    // 0 and 1 carry no letters, and neither does a character that is no digit key.
    public static TheoryData<char> KeysWithoutLetters => new() { '0', '1', '*', '#', 'a' };

    [Theory]
    [MemberData(nameof(LetteredKeys))]
    public void LowercaseLettersOf_LetteredKey_ReturnsItsLetters(char key, string letters) =>
        Assert.Equal(letters, PhoneKeypad.LowercaseLettersOf(key));

    [Theory]
    [MemberData(nameof(KeysWithoutLetters))]
    public void LowercaseLettersOf_KeyWithoutLetters_ReturnsEmpty(char key) =>
        Assert.Empty(PhoneKeypad.LowercaseLettersOf(key));

    [Fact]
    public void LowercaseLettersOf_KeysTwoThroughNine_SpellTheAlphabetOnceInOrder() =>
        Assert.Equal(
            "abcdefghijklmnopqrstuvwxyz",
            string.Concat("23456789".Select(PhoneKeypad.LowercaseLettersOf)));
}
