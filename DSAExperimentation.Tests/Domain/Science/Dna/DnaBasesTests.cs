using DSAExperimentation.Domain.Science.Dna;

namespace DSAExperimentation.Tests.Domain.Science.Dna;

// The bases are the fact itself, so the tests state them. As an alphabet they are A to T with gaps:
// B, which sits between A and C, and the characters either side of the run, '@' and 'U', are the
// ones a table that read its gaps as letters would accept.
public sealed partial class DnaBasesTests
{
    public static TheoryData<char, int> LettersToSlots =>
        new() { { 'A', 0 }, { 'C', 1 }, { 'G', 2 }, { 'T', 3 } };

    public static TheoryData<char> NonBases =>
        new() { 'B', '@', 'U', 'a', 'N' };

    public static TheoryData<int> NonSlots =>
        new() { -1, 4 };

    [Fact]
    public void All_AreTheFourBasesInOrder() =>
        Assert.Equal<DnaBase>([DnaBase.Adenine, DnaBase.Cytosine, DnaBase.Guanine, DnaBase.Thymine], DnaBases.All);

    [Fact]
    public void Letters_AreTheBasesLetters() => Assert.Equal<char>(['A', 'C', 'G', 'T'], DnaBases.Letters);

    [Fact]
    public void Names_AreTheBasesNames() =>
        Assert.Equal<string>(["adenine", "cytosine", "guanine", "thymine"], DnaBases.Names);

    [Fact]
    public void Size_IsFour() => Assert.Equal(4, DnaBases.Size);

    [Fact]
    public void LettersAsString_SpellsTheLettersInOrder() => Assert.Equal("ACGT", DnaBases.LettersAsString());

    [Theory]
    [MemberData(nameof(LettersToSlots))]
    public void IndexOf_BaseLetter_ReturnsItsSlot(char letter, int expected) =>
        Assert.Equal(expected, DnaBases.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonBases))]
    public void IndexOf_NotABase_Throws(char letter) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DnaBases.IndexOf(letter));

    [Theory]
    [MemberData(nameof(NonBases))]
    public void TryIndexOf_NotABase_IsFalse(char letter) => Assert.False(DnaBases.TryIndexOf(letter, out _));

    [Theory]
    [MemberData(nameof(NonSlots))]
    public void LetterAt_OutsideTheSlots_Throws(int index) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DnaBases.LetterAt(index));

    [Fact]
    public void LetterAt_IndexOf_RoundTripsEverySlot() =>
        Assert.All(Enumerable.Range(0, DnaBases.Size), index =>
            Assert.Equal(index, DnaBases.IndexOf(DnaBases.LetterAt(index))));
}
