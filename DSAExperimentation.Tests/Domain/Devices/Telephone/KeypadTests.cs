using DSAExperimentation.Domain.Devices.Telephone;

namespace DSAExperimentation.Tests.Domain.Devices.Telephone;

// The E.161 layout read both ways, and the older North American layout without Q and Z.
public sealed partial class KeypadTests
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

    public static TheoryData<char, char, int> LettersToPresses => new()
    {
        { 'a', '2', 1 },
        { 'e', '3', 2 },
        { 'E', '3', 2 },
        { 's', '7', 4 },
        { 'z', '9', 4 },
    };

    [Theory]
    [MemberData(nameof(LetteredKeys))]
    public void KeyFor_LetteredKey_SpellsItsLetters(char digit, string letters) =>
        Assert.Equal(letters, Keypad<E161Keypad>.KeyFor(digit).Spelling);

    [Fact]
    public void KeyFor_ZeroAndOne_CarryNoLetters()
    {
        Assert.Empty(Keypad<E161Keypad>.KeyFor('0').Letters);
        Assert.Empty(Keypad<E161Keypad>.KeyFor('1').Letters);
    }

    [Fact]
    public void KeyFor_NotADigit_Throws() => Assert.Throws<ArgumentOutOfRangeException>(() => Keypad<E161Keypad>.KeyFor('*'));

    // A key is found by its digit's slot, so every layout must list its keys in digit order.
    [Fact]
    public void KeyFor_EveryDigit_IsTheKeyOfThatDigit()
    {
        Assert.All("0123456789", digit => Assert.Equal(digit, Keypad<E161Keypad>.KeyFor(digit).Digit));
        Assert.All("0123456789", digit => Assert.Equal(digit, Keypad<ClassicNorthAmericanKeypad>.KeyFor(digit).Digit));
    }

    [Fact]
    public void KeyFor_KeysTwoThroughNine_SpellTheAlphabetOnceInOrder() =>
        Assert.Equal("abcdefghijklmnopqrstuvwxyz", string.Concat("23456789".Select(digit => Keypad<E161Keypad>.KeyFor(digit).Spelling)));

    [Theory]
    [MemberData(nameof(LettersToPresses))]
    public void PressFor_Letter_IsItsKeyAndPressCount(char letter, char digit, int presses)
    {
        var press = Keypad<E161Keypad>.PressFor(letter);

        Assert.Equal(digit, press.Key.Digit);
        Assert.Equal(presses, press.Presses);
    }

    [Fact]
    public void PressFor_LetterTheLayoutLeavesOut_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Keypad<ClassicNorthAmericanKeypad>.PressFor('q'));

    [Fact]
    public void PressFor_ClassicLayout_PutsSOnSevenThirdPress() =>
        Assert.Equal(3, Keypad<ClassicNorthAmericanKeypad>.PressFor('s').Presses);
}
