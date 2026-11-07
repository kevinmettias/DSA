using DSAExperimentation.Domain.Computing.Binary;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// An int read as a trie key of its 32 bits, the most significant first, each a binary digit given
// as its slot among BinaryDigits, 0 or 1. Most significant first is a correctness requirement, not
// a choice: a higher bit outweighs every lower one, so a walk that settles the bits in this order,
// as a maximum-XOR walk does, settles the number's magnitude, and keys that share their high bits
// share a path. The bits are read as the unsigned pattern, so a negative int's sign bit is simply
// its first letter.
//
// The length is the width of an int, a constant, so a walk over this key runs a fixed count of
// steps. Position 0 is bit 31 and position 31 is bit 0. A letter is read by shifting the bit at its
// position up to the top and down to the bottom: two shifts by a count the loop already holds, so
// reading from the most significant end costs no more than reading from the least.
/// <summary>An int as a trie key of its 32 bits, the most significant first.</summary>
/// <param name="value">The int whose bits the key reads, as a two's-complement pattern.</param>
public readonly struct Int32BitsKey(int value) : ITrieKey<int>
{
    /// <summary>The number of letters every such key has: the bits in an int.</summary>
    public const int Bits = BitWidths.Int32;

    private const int LastPosition = Bits - 1;

    /// <inheritdoc/>
    int ITrieKey<int>.Length => Bits;

    /// <inheritdoc/>
    public int LetterAt(int position) => (int)((unchecked((uint)value) << position) >> LastPosition);

    /// <summary>Reads an int as a key.</summary>
    /// <param name="value">The int to read.</param>
    /// <returns>The key of the int's bits.</returns>
    public static implicit operator Int32BitsKey(int value) => new(value);
}
