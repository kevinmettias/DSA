using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Tests.DataStructures.Graph.Hamming;

public sealed partial class StandardAlphabetsTests
{
    [Fact]
    public void LowercaseLatin_IsTheTwentySixLettersInOrder()
    {
        Assert.Equal("abcdefghijklmnopqrstuvwxyz", StandardAlphabets.LowercaseLatin.Characters);
        Assert.Equal(26, StandardAlphabets.LowercaseLatin.Characters.Length);
    }

    // A repeated character would make OneCharacterMutations emit duplicates.
    [Fact]
    public void LowercaseLatin_ContainsNoRepeats() =>
        Assert.Equal(
            StandardAlphabets.LowercaseLatin.Characters.Length,
            StandardAlphabets.LowercaseLatin.Characters.Distinct().Count());
}
