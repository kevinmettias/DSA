using System.Collections.Immutable;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.Tests.DataStructures.Alphabets;

// The even digits as a set over the ten digits, test-only witnesses so tier 1's set is checked
// without a Domain preset.
public sealed partial class LetterSetTests
{
    public static TheoryData<char> Members => new() { '0', '4', '8' };

    public static TheoryData<char> NonMembers => new() { '1', '9', 'a', '/' };

    [Theory]
    [MemberData(nameof(Members))]
    public void Has_LetterOfTheSet_IsTrue(char letter) =>
        Assert.True(LetterSet<EvenDigits, Digits>.Has(letter));

    // An odd digit is a letter of the alphabet outside the set; 'a' and '/' are not letters at all.
    [Theory]
    [MemberData(nameof(NonMembers))]
    public void Has_AnyOtherCharacter_IsFalse(char letter) =>
        Assert.False(LetterSet<EvenDigits, Digits>.Has(letter));

    [Fact]
    public void Count_IsTheNumberOfLetters() => Assert.Equal(5, LetterSet<EvenDigits, Digits>.Count);

    // 65 letters do not fit a 64-slot mask, so the set refuses to build when its mask is first read.
    [Fact]
    public void Has_AlphabetLargerThanTheMask_Throws()
    {
        var thrown = Assert.Throws<TypeInitializationException>(() => LetterSet<FirstOfMany, Many>.Has('\u0100'));

        Assert.IsType<InvalidOperationException>(thrown.InnerException);
    }

    private readonly struct Digits : IAlphabet
    {
        public static int Size => 10;

        private static LetterRun Run => new('0', Size);

        public static int IndexOf(char letter) => Run.IndexOf(letter);

        public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

        public static char LetterAt(int index) => Run.LetterAt(index);
    }

    private readonly struct EvenDigits : ILetterSet<Digits>
    {
        public static ImmutableArray<char> Letters => ['0', '2', '4', '6', '8'];
    }

    private readonly struct Many : IAlphabet
    {
        public static int Size => 65;

        private static LetterRun Run => new('\u0100', Size);

        public static int IndexOf(char letter) => Run.IndexOf(letter);

        public static bool TryIndexOf(char letter, out int index) => Run.TryIndexOf(letter, out index);

        public static char LetterAt(int index) => Run.LetterAt(index);
    }

    private readonly struct FirstOfMany : ILetterSet<Many>
    {
        public static ImmutableArray<char> Letters => ['\u0100'];
    }
}
