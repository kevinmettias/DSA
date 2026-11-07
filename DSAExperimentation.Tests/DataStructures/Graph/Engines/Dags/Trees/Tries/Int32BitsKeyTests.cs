using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A key's letters, read one by one, must spell the value's bits as the framework's binary
// formatting does, most significant first, the sign bit of a negative value included.
public sealed partial class Int32BitsKeyTests
{
    public static TheoryData<int> Values => new() { 0, 5, -1, int.MinValue, int.MaxValue, 0x1234_5678 };

    [Fact]
    public void Bits_IsTheWidthOfAnInt() => Assert.Equal(32, Int32BitsKey.Bits);

    [Fact]
    public void Length_IsThirtyTwoForEveryValue() => Assert.Equal(Int32BitsKey.Bits, LengthOf(new Int32BitsKey(5)));

    [Theory]
    [MemberData(nameof(Values))]
    public void LetterAt_EveryPosition_SpellsTheBitsMostSignificantFirst(int value)
    {
        var key = new Int32BitsKey(value);
        var spelled = new string([.. Enumerable.Range(0, Int32BitsKey.Bits).Select(position => BinaryDigits.LetterAt(key.LetterAt(position)))]);

        Assert.Equal(Convert.ToString(value, 2).PadLeft(Int32BitsKey.Bits, BinaryDigits.Zero), spelled);
    }

    [Fact]
    public void LetterAt_FirstPosition_IsTheSignBit() => Assert.Equal((1, 0), (new Int32BitsKey(int.MinValue).LetterAt(0), new Int32BitsKey(int.MaxValue).LetterAt(0)));

    [Fact]
    public void ImplicitConversion_ReadsTheSameBits()
    {
        Int32BitsKey key = 1;

        Assert.Equal(1, key.LetterAt(Int32BitsKey.Bits - 1));
    }

    private static int LengthOf<TKey>(TKey key)
        where TKey : struct, ITrieKey<int>
        => key.Length;
}
