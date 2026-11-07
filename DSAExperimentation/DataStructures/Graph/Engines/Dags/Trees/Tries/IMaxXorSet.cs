namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The layer a bit trie is to the Maximum XOR family (LeetCode 421, 1707, 2932, 2935): a set of
// ints, each keyed by its 32 bits from the most significant. Every implementation keeps these
// invariants, and MaxXorSetContractTests holds each to them:
// - Count counts distinct values, and inserting a value already held changes nothing.
// - HasKey is true exactly for an inserted value. HasPrefix(value, bits) is true exactly when some
//   held value has the same top bits as value, so 0 bits asks whether anything is held, and bits
//   outside 0 to 32 are refused.
// - TryMaxXor is false on an empty set and otherwise gives the largest XOR of the query with a held
//   value, compared as unsigned bit patterns.
// The one trie meets it through BitTrieSet, its view of BitTrie, and TwoFieldBitTrie is the
// hand-specialized strategy for exactly this layer (the owner's ruling, ARCHITECTURE §13.9). Each
// implementation is a struct and Empty is static, so code generic over the set, LC 2935's bucket
// strategy, is compiled for each one and calls it directly, never through the interface.
/// <summary>A set of ints keyed by their 32 bits, answering membership, high-bit prefix and maximum-XOR queries.</summary>
/// <typeparam name="TSelf">The set type itself.</typeparam>
public interface IMaxXorSet<TSelf>
    where TSelf : struct, IMaxXorSet<TSelf>
{
    /// <summary>Gets how many distinct values are held.</summary>
    int Count { get; }

    /// <summary>Makes a new, empty set.</summary>
    /// <returns>An empty set.</returns>
    static abstract TSelf Empty();

    /// <summary>Adds a value; adding one already held changes nothing.</summary>
    /// <param name="value">The value to add.</param>
    void Insert(int value);

    /// <summary>Reports whether a value is held.</summary>
    /// <param name="value">The value to look for.</param>
    /// <returns>True when the value was inserted.</returns>
    bool HasKey(int value);

    /// <summary>Reports whether any held value starts with the high bits of a value.</summary>
    /// <param name="value">The value whose high bits make the prefix.</param>
    /// <param name="bits">How many of its bits, from the most significant, the prefix takes: 0 to 32.</param>
    /// <returns>True when some held value shares those bits, so 0 bits asks whether any value is held.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bits"/> is negative or more than 32.</exception>
    bool HasPrefix(int value, int bits);

    /// <summary>Finds the largest XOR of a value with any value held, comparing results as unsigned bit patterns.</summary>
    /// <param name="value">The value to XOR against the held ones.</param>
    /// <param name="result">The largest XOR as an int, so negative when bit 31 is set, or 0 when the set is empty.</param>
    /// <returns><c>true</c> when the set holds a value; <c>false</c> when it is empty.</returns>
    bool TryMaxXor(int value, out int result);
}
