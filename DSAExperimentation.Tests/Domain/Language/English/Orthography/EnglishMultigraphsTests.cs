using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.Orthography;

namespace DSAExperimentation.Tests.Domain.Language.English.Orthography;

// The digraphs and the trigraph are the fact itself, so the tests spell them out. Has is asked
// about each in mixed case, and about letter pairs that are no digraph of this set: a vowel pair, a
// single letter, a doubled one, and wh, which the set does not hold.
public sealed partial class EnglishMultigraphsTests
{
    public static TheoryData<Multigraph, string> NamedMultigraphs => new()
    {
        { EnglishMultigraphs.Ch, "ch" },
        { EnglishMultigraphs.Sh, "sh" },
        { EnglishMultigraphs.Th, "th" },
        { EnglishMultigraphs.Ph, "ph" },
        { EnglishMultigraphs.Ng, "ng" },
        { EnglishMultigraphs.Ck, "ck" },
        { EnglishMultigraphs.Tch, "tch" },
    };

    public static TheoryData<string> Spellings => new() { "ch", "SH", "Th", "pH", "ng", "CK", "tch", "TcH" };

    public static TheoryData<string> NoSpellings => new() { "c", "ea", "chh", "wh", "tc", "", "ch " };

    [Theory]
    [MemberData(nameof(NamedMultigraphs))]
    public void NamedMultigraph_IsItsLetters(Multigraph multigraph, string letters) =>
        Assert.Equal(new Multigraph(letters), multigraph);

    [Fact]
    public void Digraphs_AreTheSixConsonantDigraphs() =>
        Assert.Equal<string>(["ch", "sh", "th", "ph", "ng", "ck"], EnglishMultigraphs.Digraphs.Select(digraph => digraph.Letters));

    [Fact]
    public void Trigraphs_AreTheOneTrigraph() =>
        Assert.Equal<string>(["tch"], EnglishMultigraphs.Trigraphs.Select(trigraph => trigraph.Letters));

    [Fact]
    public void Multigraphs_AreTheDigraphsThenTheTrigraph() =>
        Assert.Equal<Multigraph>([.. EnglishMultigraphs.Digraphs, .. EnglishMultigraphs.Trigraphs], EnglishMultigraphs.Multigraphs);

    [Fact]
    public void Multigraphs_EachIsSpelledWithConsonantLetters() =>
        Assert.All(EnglishMultigraphs.Multigraphs, multigraph =>
            Assert.All(multigraph.Letters, letter => Assert.True(EnglishConsonants.Has(letter))));

    [Theory]
    [MemberData(nameof(Spellings))]
    public void Has_DigraphOrTrigraphInAnyCase_IsTrue(string letters) => Assert.True(EnglishMultigraphs.Has(letters));

    [Theory]
    [MemberData(nameof(NoSpellings))]
    public void Has_LettersNoneOfThemSpell_IsFalse(string letters) => Assert.False(EnglishMultigraphs.Has(letters));
}
