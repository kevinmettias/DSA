using DSAExperimentation.Domain.Computing.Binary;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A hand-specialized strategy for exactly one layer, the XOR multiset (IXorMultiset), beside the
// one trie's configuration of it, CountedBitTrie (the owner's rule of 2026-11-02, as applied to the
// bit trie on 2026-11-06, ARCHITECTURE §13.9). It keeps that layer's invariants and nothing more:
// Count counts copies, TryRemove takes one back out, TryMaxXor reads only live values, and
// CountXorBelow counts them below a limit. XorMultisetContractTests holds it and the template to
// the same answers, and XorMultisetBenchmarks compares them.
//
// What it specializes away is the node as an object. Nodes are slots in shared arrays, a pool that
// doubles when full: _children keeps a node's two child slots side by side at node * 2 + bit, so a
// walk picks a child by arithmetic on the bit, and _counts keeps its live count. A node costs 12
// bytes where the template's CountedBitTrieNode costs 40, an object header, two references and the
// count, padded, and the arrays hold no references for the collector to trace. Slot 0 is a sentinel
// for "no node", count 0 with both child slots pointing back at itself, so a walk that steps off
// every inserted path keeps reading zeros instead of testing for an absent child at each level.
// Nodes are never reclaimed: storage grows with the distinct paths ever inserted (at most 32 nodes
// per value), not with Count. What it gives up is the one trie's node contract, so its walks are
// its own, written out for an int's 32 bits.
//
// Every XOR here is read as an UNSIGNED 32-bit pattern, BitTrie.TryMaxXor's reading: TryMaxXor
// maximizes the unsigned pattern and CountXorBelow compares unsigned. For non-negative values
// both agree with signed arithmetic, and a caller's high + 1 limit stays right even at
// high = int.MaxValue, where it wraps to the unsigned 2^31.
//
// Insert, TryRemove, TryMaxXor and CountXorBelow each walk the 32 levels once (TryRemove twice):
// O(1), Insert's amortized over the doubling.
/// <summary>
/// A XOR multiset as a bit trie whose nodes are slots in shared arrays, its walks written out
/// for ints.
/// </summary>
public sealed class PooledCountedBitTrie
{
    private const int ChildSlotsPerNode = 2;

    // The sentinel's slot, and the value an empty child slot holds - the root is slot 1, so no
    // real node is ever a child at slot 0.
    private const int Absent = 0;
    private const int Root = 1;

    // The sentinel, the root and one node per bit level: exactly what the first Insert lays down.
    private const int InitialNodeCapacity = BitWidths.Int32 + 2;

    private int[] _children = new int[InitialNodeCapacity * ChildSlotsPerNode];
    private int[] _counts = new int[InitialNodeCapacity];
    private int _nodeCount = Root + 1;

    // How many values are live, duplicates included - every live value's path starts at the root.
    /// <summary>Gets how many values are held, duplicates included.</summary>
    public int Count => _counts[Root];

    // Every node's count is at most the root's, so the root is the only count that can overflow,
    // and its increment, the first write, is checked arithmetic, which throws before it stores.
    // Checked arithmetic adds no IL, only a jump on overflow in the compiled code, so callers that
    // inline Insert keep their inlining budget: a comparison and a throw call written out ahead of
    // the increment made LC 3845's benchmarked root, which inlines its whole loop, run out of
    // budget at different calls than before.
    /// <summary>Adds one copy of a value.</summary>
    /// <param name="value">The value to add, any int.</param>
    /// <exception cref="OverflowException">
    /// The trie already holds <see cref="int.MaxValue"/> values.
    /// </exception>
    public void Insert(int value)
    {
        var bits = unchecked((uint)value);
        var node = Root;

        checked
        {
            _counts[Root]++;
        }

        for (var level = BitWidths.Int32 - 1; level >= 0; level--)
        {
            var bit = BitAt(bits, level);
            node = ChildOrNew(node, bit);
            _counts[node]++;
        }
    }

    private int ChildOrNew(int node, uint bit)
    {
        var slot = SlotOf(node, bit);

        if (_children[slot] != Absent)
        {
            return _children[slot];
        }

        // NewNode may replace _children with a larger copy, so the store reads the field only
        // after it returns - "_children[slot] = NewNode()" would evaluate the array first and
        // write the link into the copy being discarded.
        var created = NewNode();
        _children[slot] = created;

        return created;
    }

    private int NewNode()
    {
        if (_nodeCount == _counts.Length)
        {
            Grow();
        }

        return _nodeCount++;
    }

    // Array.Resize zero-fills the new tail, which is exactly an unused node: count 0, both child
    // slots Absent.
    private void Grow()
    {
        var capacity = _counts.Length * ArrayGrowth.GrowthFactor;

        Array.Resize(ref _counts, capacity);
        Array.Resize(ref _children, capacity * ChildSlotsPerNode);
    }

    // False, changing nothing, when no live copy of value is held. A value is live exactly when
    // its leaf's count is positive, and a positive leaf means every node above it is positive too
    // (each count is the sum of its children's), so one read-only walk decides it before the
    // decrementing one.
    /// <summary>Removes one copy of a value if the trie holds one.</summary>
    /// <param name="value">The value to remove a copy of.</param>
    /// <returns>True when a copy was removed; false, changing nothing, when none is held.</returns>
    public bool TryRemove(int value)
    {
        if (_counts[LeafOf(value)] == 0)
        {
            return false;
        }

        var bits = unchecked((uint)value);
        var node = Root;
        _counts[Root]--;

        for (var level = BitWidths.Int32 - 1; level >= 0; level--)
        {
            var bit = BitAt(bits, level);
            node = _children[SlotOf(node, bit)];
            _counts[node]--;
        }

        return true;
    }

    // The node value's path ends at, or Absent once it leaves the laid-down nodes - the sentinel's
    // own child slots hold Absent, so the walk stays there without a test per level.
    private int LeafOf(int value)
    {
        var bits = unchecked((uint)value);
        var node = Root;

        for (var level = BitWidths.Int32 - 1; level >= 0; level--)
        {
            var bit = BitAt(bits, level);
            node = _children[SlotOf(node, bit)];
        }

        return node;
    }

    // The largest value ^ y over every live y, as an unsigned pattern cast back to int; false
    // when nothing is live.
    /// <summary>
    /// Finds the largest XOR of a value with any held value, comparing XORs as unsigned 32-bit patterns.
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

        result = unchecked((int)MaxXorPattern(unchecked((uint)value)));
        return true;
    }

    // Precondition: Count > 0. A node's count is the sum of its children's, so wherever the
    // opposite child carries no live value the same-bit child carries all of this node's - the
    // walk never lands on a dead node, and the result bit is set exactly where it went opposite.
    private uint MaxXorPattern(uint bits)
    {
        var node = Root;
        var xor = 0u;

        for (var level = BitWidths.Int32 - 1; level >= 0; level--)
        {
            var bit = BitAt(bits, level);
            var oppositeBit = bit ^ 1u;
            var oppositeIsLive = _counts[_children[SlotOf(node, oppositeBit)]] > 0;
            var chosenBit = oppositeIsLive ? oppositeBit : bit;

            node = _children[SlotOf(node, chosenBit)];
            xor |= (chosenBit ^ bit) << level;
        }

        return xor;
    }

    // How many live y have value ^ y strictly below limit, both read unsigned. The walk follows the
    // child whose XOR bit equals limit's bit, so the XOR stays equal to limit's prefix; at a level
    // where limit's bit is 1, the same-bit child's values put a 0 there - below limit whatever
    // their lower bits hold - so its whole subtree count is added. A y whose XOR equals limit
    // reaches the end of the walk and is never added. The walk runs all 32 levels even after it
    // leaves the laid-down nodes, through the sentinel: stopping there instead measured slower on
    // LeetCode 1803's workload, which seldom leaves them early.
    /// <summary>
    /// Counts the held values whose XOR with a value is below a limit, both read as unsigned 32-bit patterns.
    /// </summary>
    /// <param name="value">The value to XOR with each held value.</param>
    /// <param name="limit">
    /// The exclusive bound, read unsigned, so an overflowed <c>int.MaxValue + 1</c> means 2^31.
    /// </param>
    /// <returns>
    /// The number of held values, duplicates included, whose XOR with the value is below the limit.
    /// </returns>
    public int CountXorBelow(int value, int limit)
    {
        var valueBits = unchecked((uint)value);
        var limitBits = unchecked((uint)limit);
        var node = Root;
        var count = 0;

        for (var level = BitWidths.Int32 - 1; level >= 0; level--)
        {
            var bit = BitAt(valueBits, level);
            var limitBit = BitAt(limitBits, level);

            if (limitBit == 1u)
            {
                count += _counts[_children[SlotOf(node, bit)]];
            }

            node = _children[SlotOf(node, bit ^ limitBit)];
        }

        return count;
    }

    // A test's way to reach the overflow refusal without 2^31 inserts: the root alone is given the
    // count, breaking the rule that a count is the sum of its children's, so nothing else is
    // meaningful on a trie seeded this way.
    internal void SeedRootCount(int count) => _counts[Root] = count;

    private static uint BitAt(uint bits, int level) => (bits >> level) & 1u;

    // A node's child for bit sits at node * 2 + bit, its two slots side by side.
    private static int SlotOf(int node, uint bit) => (node * ChildSlotsPerNode) + (int)bit;
}
