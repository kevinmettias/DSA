using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed class StandardAlphabetsTests
{
    [Fact]
    public void LowercaseLatin_IsTheTwentySixLettersInOrder()
    {
        Assert.Equal("abcdefghijklmnopqrstuvwxyz", StandardAlphabets.LowercaseLatin.Characters);
        Assert.Equal(26, StandardAlphabets.LowercaseLatin.Characters.Length);
    }

    [Fact]
    public void Dna_IsTheFourBases() => Assert.Equal("ACGT", StandardAlphabets.Dna.Characters);

    [Fact]
    public void LowercaseLatin_And_Dna_ContainNoRepeats()
    {
        // A repeated character would make OneCharacterMutations emit duplicates.
        Assert.Equal(
            StandardAlphabets.LowercaseLatin.Characters.Length,
            StandardAlphabets.LowercaseLatin.Characters.Distinct().Count());

        Assert.Equal(
            StandardAlphabets.Dna.Characters.Length,
            StandardAlphabets.Dna.Characters.Distinct().Count());
    }
}
