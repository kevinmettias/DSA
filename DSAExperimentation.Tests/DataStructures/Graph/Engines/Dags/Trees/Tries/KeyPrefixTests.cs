using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A prefix reads its key's own letters up to its length; a length outside the key is refused at
// once, so an int's 32 bits cannot be read past their end, where a shift would wrap round.
public sealed partial class KeyPrefixTests
{
    public static TheoryData<int> LengthsOutsideTheKey => new() { -1, 5 };

    [Fact]
    public void Length_IsTheLengthTaken() => Assert.Equal(2, new KeyPrefix<TextKey, char>("tree", 2).Length);

    [Fact]
    public void LetterAt_ReadsTheKeysLetters()
    {
        var prefix = new KeyPrefix<TextKey, char>("tree", 3);

        Assert.Equal("tre", new string([.. Enumerable.Range(0, prefix.Length).Select(prefix.LetterAt)]));
    }

    [Fact]
    public void Constructor_WholeKeyAndNone_AreBothPrefixes()
    {
        var whole = new KeyPrefix<Int32BitsKey, int>(5, Int32BitsKey.Bits);
        var none = new KeyPrefix<Int32BitsKey, int>(5, 0);

        Assert.Equal((Int32BitsKey.Bits, 0), (whole.Length, none.Length));
    }

    [Theory]
    [MemberData(nameof(LengthsOutsideTheKey))]
    public void Constructor_LengthOutsideTheKey_Throws(int length) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new KeyPrefix<TextKey, char>("tree", length));
}
