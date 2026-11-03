using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class DenseTrieLayoutTests
{
    private const int AlphabetSize = 26;

    // Letters 0, 2, 5 and 7 present.
    private const ulong Present = 0b1010_0101;

    public static TheoryData<int, int> RanksAndSlots => new()
    {
        { 0, 0 },
        { 1, 2 },
        { 2, 5 },
        { 3, 7 },
    };

    [Fact]
    public void HasSlotPerLetter_IsTrue() => Assert.True(DenseTrieLayout.HasSlotPerLetter);

    [Fact]
    public void InitialCapacity_IsTheWholeAlphabet() => Assert.Equal(AlphabetSize, DenseTrieLayout.InitialCapacity(AlphabetSize));

    [Fact]
    public void GrownCapacity_IsTheWholeAlphabet() => Assert.Equal(AlphabetSize, DenseTrieLayout.GrownCapacity(3, AlphabetSize));

    [Fact]
    public void PositionOf_IsTheLettersOwnSlot_WhateverIsPresent()
    {
        Assert.Equal(6, DenseTrieLayout.PositionOf(Present, 6));
        Assert.Equal(7, DenseTrieLayout.PositionOf(Present, 7));
    }

    [Theory]
    [MemberData(nameof(RanksAndSlots))]
    public void PositionOfRank_IsTheSlotOfThatPresentLetter(int rank, int slot) =>
        Assert.Equal(slot, DenseTrieLayout.PositionOfRank(Present, rank));
}
