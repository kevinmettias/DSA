using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

public sealed partial class PackedTrieLayoutTests
{
    private const int AlphabetSize = 26;

    // Letters 0, 2, 5 and 7 present.
    private const ulong Present = 0b1010_0101;

    public static TheoryData<int, int, int> GrowthSteps => new()
    {
        { 0, AlphabetSize, 1 },
        { 1, AlphabetSize, 2 },
        { 2, AlphabetSize, 4 },
        { 4, AlphabetSize, 8 },
        { 8, AlphabetSize, 16 },
        { 16, AlphabetSize, AlphabetSize },
        { 2, 3, 3 },
    };

    // A present letter's position is its rank; an absent one's is where it would be inserted.
    public static TheoryData<int, int> LettersAndPositions => new()
    {
        { 0, 0 },
        { 2, 1 },
        { 5, 2 },
        { 7, 3 },
        { 1, 1 },
        { 6, 3 },
        { 63, 4 },
    };

    [Fact]
    public void HasSlotPerLetter_IsFalse() => Assert.False(PackedTrieLayout.HasSlotPerLetter);

    [Fact]
    public void InitialCapacity_IsNoSlots() => Assert.Equal(0, PackedTrieLayout.InitialCapacity(AlphabetSize));

    [Theory]
    [MemberData(nameof(GrowthSteps))]
    public void GrownCapacity_DoublesFromOne_UpToTheAlphabet(int childCount, int alphabetSize, int expected) =>
        Assert.Equal(expected, PackedTrieLayout.GrownCapacity(childCount, alphabetSize));

    [Theory]
    [MemberData(nameof(LettersAndPositions))]
    public void PositionOf_CountsThePresentLettersBelow(int letter, int position) =>
        Assert.Equal(position, PackedTrieLayout.PositionOf(Present, letter));

    [Fact]
    public void PositionOfRank_IsTheRank() => Assert.Equal(2, PackedTrieLayout.PositionOfRank(Present, 2));
}
