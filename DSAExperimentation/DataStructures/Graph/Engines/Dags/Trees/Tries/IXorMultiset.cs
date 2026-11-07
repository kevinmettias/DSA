namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The layer a counted bit trie is to the members of the Maximum XOR family that forget values
// (LeetCode 1803, 1938, 2935's sliding window, 3845): a multiset of ints, each keyed by its 32 bits
// from the most significant, from which a copy can be taken back out. Every implementation keeps
// these invariants, and XorMultisetContractTests holds each to them:
// - Count counts copies, duplicates included: each Insert adds one and each successful TryRemove
//   takes one away. An Insert past int.MaxValue copies is refused, changing nothing, since a count
//   that wrapped would read as nothing held.
// - TryRemove is false, changing nothing, when no copy of the value is held, including a value
//   whose every copy was removed.
// - TryMaxXor is false when nothing is held and otherwise gives the largest XOR of the query with a
//   held value, compared as unsigned bit patterns: a removed value is never the partner.
// - CountXorBelow counts the held copies whose XOR with the value is below the limit, both read as
//   unsigned bit patterns.
// The one trie meets it through CountedBitTrieMultiset, its view of CountedBitTrie, and
// PooledCountedBitTrie is the hand-specialized strategy for exactly this layer (ARCHITECTURE
// §13.9). Each implementation is a struct and Empty is static, so code generic over the multiset is
// compiled for each one and calls it directly, never through the interface.
/// <summary>
/// A multiset of ints keyed by their 32 bits, answering maximum-XOR and XOR-below-a-limit
/// queries, from which values can be removed.
/// </summary>
/// <typeparam name="TSelf">The multiset type itself.</typeparam>
public interface IXorMultiset<TSelf>
    where TSelf : struct, IXorMultiset<TSelf>
{
    /// <summary>Gets how many values are held, duplicates included.</summary>
    int Count { get; }

    /// <summary>Makes a new, empty multiset.</summary>
    /// <returns>An empty multiset.</returns>
    static abstract TSelf Empty();

    /// <summary>Adds one copy of a value.</summary>
    /// <param name="value">The value to add, any int.</param>
    /// <exception cref="OverflowException">
    /// The multiset already holds <see cref="int.MaxValue"/> values.
    /// </exception>
    void Insert(int value);

    /// <summary>Removes one copy of a value if the multiset holds one.</summary>
    /// <param name="value">The value to remove a copy of.</param>
    /// <returns>True when a copy was removed; false, changing nothing, when none is held.</returns>
    bool TryRemove(int value);

    /// <summary>
    /// Finds the largest XOR of a value with any held value, comparing XORs as unsigned 32-bit
    /// patterns.
    /// </summary>
    /// <param name="value">The value to XOR with each held value.</param>
    /// <param name="result">
    /// The largest XOR as an int, negative when its top bit is set; 0 when nothing is held.
    /// </param>
    /// <returns>True when the multiset holds a value; false when it is empty.</returns>
    bool TryMaxXor(int value, out int result);

    /// <summary>
    /// Counts the held values whose XOR with a value is below a limit, both read as unsigned 32-bit
    /// patterns.
    /// </summary>
    /// <param name="value">The value to XOR with each held value.</param>
    /// <param name="limit">
    /// The exclusive bound, read unsigned, so an overflowed <c>int.MaxValue + 1</c> means 2^31.
    /// </param>
    /// <returns>
    /// The number of held values, duplicates included, whose XOR with the value is below the limit.
    /// </returns>
    int CountXorBelow(int value, int limit);
}
