using CountedWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.CountedBitTrieNode>,
    int>;

using System.Runtime.CompilerServices;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie over an int's bits, as a multiset whose values can be removed: BitTrie's key and
// store, Int32BitsKey over BinarySlots, with the count payload, CountedBitTrieNode, which counts
// the held values whose path passes through each node. It is built for the members of the Maximum
// XOR family that forget values: a range count (LeetCode 1803), a depth-first walk that undoes on
// the way back up (1938), and sliding windows (2935, 3845).
//
// What the count adds over BitTrie is what removal needs. A removal lowers the counts along a
// value's path but leaves its nodes, so a node's existence says only that some value once passed
// through it, and every walk reads liveness from the counts: the maximum-XOR walk, MaxXorWalk,
// shared with BitTrie, takes a child only while its count is positive, and CountXorBelow adds a
// whole subtree's count in one read. Insert and TryRemove are TrieWalk's walk with a step that
// adds one to each count on the path or takes one away.
//
// No tree-topology witness, though the store is ordered and the nodes are objects: §13.9's second
// test fails, since after a removal the tree still holds paths whose values are gone, so a
// Tree-tier engine walking it, TreeMetrics.Size say, would count values no longer held. The root
// is not handed out, so no engine is given a node to walk.
//
// The pooled layout the counted trie had been, nodes as slots in shared arrays, is
// PooledCountedBitTrie, the hand-specialized strategy for the same layer, IXorMultiset; a node here
// is 40 bytes against its 12, and its four users measured faster or leaner on it (§13.9).
//
// Every XOR here is read as an unsigned 32-bit pattern: TryMaxXor maximizes the unsigned pattern
// and CountXorBelow compares unsigned. For non-negative values both agree with signed arithmetic,
// and a caller's high + 1 limit stays right even at high = int.MaxValue, where it wraps to the
// unsigned 2^31. Each operation walks the 32 levels once, TryRemove twice: O(1) bounded by an
// int's width.
/// <summary>
/// A multiset of ints in a binary trie over their 32 bits, answering maximum-XOR and
/// XOR-below-a-limit queries, with removal.
/// </summary>
public sealed class CountedBitTrie
{
    // The value of the digit one: where the limit's digit is one, the walk counts the values whose
    // XOR digit is zero.
    private const int OneDigit = BinaryDigits.One - BinaryDigits.Zero;

    private readonly CountedBitTrieNode _root = new();

    // Every held value's path starts at the root, so the root's count is the multiset's size.
    /// <summary>Gets how many values are held, duplicates included.</summary>
    public int Count => _root.Count;

    /// <summary>Adds one copy of a value.</summary>
    /// <param name="value">The value to add, any int.</param>
    /// <exception cref="OverflowException">
    /// The trie already holds <see cref="int.MaxValue"/> values.
    /// </exception>
    public void Insert(int value) => AddCopy(_root, value);

    // The walks are compiled on their own and never inlined, as BitTrie's are (§13.8): each
    // inlines TrieWalk or MaxXorWalk, shared generic code over the node class, which is exact only
    // where it is inlined into a method whose node class is known. They take the root rather than
    // the trie, so a caller that inlines the public members never hands the trie to a call.
    //
    // Every node's count is at most the root's, so the root is the only count that can overflow,
    // and it is checked before any count changes.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AddCopy(CountedBitTrieNode root, Int32BitsKey key)
    {
        if (root.Count == int.MaxValue)
        {
            TrieFailures.ThrowCountFull();
        }

        CountedWalk.GetOrAddPath<Int32BitsKey, CopyAdded>(root, key);
    }

    /// <summary>Removes one copy of a value if the trie holds one.</summary>
    /// <param name="value">The value to remove a copy of.</param>
    /// <returns>True when a copy was removed; false, changing nothing, when none is held.</returns>
    public bool TryRemove(int value) => TryRemoveCopy(_root, value);

    // A value is held exactly when its last node's count is positive, and then every count above
    // it is positive too, so one walk that changes nothing decides it before the walk that takes
    // the copy out, along a path that is all laid down.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TryRemoveCopy(CountedBitTrieNode root, Int32BitsKey key)
    {
        if (CountedWalk.Find(root, key) is not { Count: > 0 })
        {
            return false;
        }

        CountedWalk.GetOrAddPath<Int32BitsKey, CopyRemoved>(root, key);
        return true;
    }

    /// <summary>
    /// Finds the largest XOR of a value with any held value, comparing XORs as unsigned 32-bit
    /// patterns.
    /// </summary>
    /// <param name="value">The value to XOR with each held value.</param>
    /// <param name="result">
    /// The largest XOR as an int, negative when its top bit is set; 0 when the trie is empty.
    /// </param>
    /// <returns>True when the trie holds a value; false when it is empty.</returns>
    public bool TryMaxXor(int value, out int result)
    {
        if (Count == 0)
        {
            result = 0;
            return false;
        }

        result = unchecked((int)MaxXorPattern(_root, value, ~value));
        return true;
    }

    // Precondition: Count > 0. The walk is MaxXorWalk's, shared with BitTrie, compiled here for
    // CountedBitTrieNode, whose children hold a value only while their count is positive.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static uint MaxXorPattern(
        CountedBitTrieNode root, Int32BitsKey key, Int32BitsKey complement) =>
        MaxXorWalk<CountedBitTrieNode>.Pattern(root, key, complement);

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
    public int CountXorBelow(int value, int limit) =>
        CountBelow(_root, value, limit, value ^ limit);

    // How many held y have value ^ y below limit. The walk follows the child whose XOR digit equals
    // limit's, so the XOR stays equal to limit's prefix: that child's digit is value's XOR limit's,
    // read from their XOR as a key of its own so that every digit is a top bit shifted down. Where
    // limit's digit is 1, the child of value's own digit puts a 0 in the XOR there, below limit
    // whatever its lower digits hold, so its whole count is added. A y whose XOR equals limit
    // reaches the end of the walk and is never added, and the walk stops where no node is laid
    // down.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int CountBelow(
        CountedBitTrieNode root, Int32BitsKey key, Int32BitsKey limit, Int32BitsKey keyXorLimit)
    {
        var current = root;
        var count = 0;

        for (var position = 0; position < Int32BitsKey.Bits; position++)
        {
            if (limit.LetterAt(position) == OneDigit)
            {
                count += CountOf(current.Children.ChildFor(key.LetterAt(position)));
            }

            var next = current.Children.ChildFor(keyXorLimit.LetterAt(position));

            if (next is null)
            {
                return count;
            }

            current = next;
        }

        return count;
    }

    private static int CountOf(CountedBitTrieNode? node) => node is null ? 0 : node.Count;

    // A test's way to reach the overflow refusal without 2^31 inserts: the root alone is given the
    // count, breaking the rule that a count is the sum of its children's, so nothing else is
    // meaningful on a trie seeded this way.
    internal void SeedRootCount(int count) => _root.Count = count;

    // A value going in: one more held value passes through each node on its path.
    private readonly struct CopyAdded : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count++;
    }

    // A value coming out: one fewer passes through each node on its path.
    private readonly struct CopyRemoved : ITrieStep<CountedBitTrieNode>
    {
        public static void Apply(CountedBitTrieNode node) => node.Count--;
    }
}
