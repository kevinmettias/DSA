using DSAExperimentation.Domain.Dna;

namespace DSAExperimentation.Tests.Domain.Dna;

public sealed partial class DnaBasesTests
{
    [Fact]
    public void Letters_AreTheFourBases() => Assert.Equal("ACGT", DnaBases.Letters);

    // A repeated base would make a one-base mutation walk emit duplicates.
    [Fact]
    public void Letters_ContainNoRepeats() => Assert.Equal(DnaBases.Letters.Length, DnaBases.Letters.Distinct().Count());

    [Fact]
    public void Alphabet_HoldsTheLetters() => Assert.Equal(DnaBases.Letters, DnaBases.Alphabet.Characters);
}
