namespace DSAExperimentation.DataStructures.CountedBitTrie;

// A binary trie over int's 32-bit two's-complement pattern (bit 31 down to bit 0) holding a
// MULTISET of ints that can shrink: every node counts the live values whose bit path passes
// through it, so TryRemove takes a value back out, and the two greedy walks - TryMaxXor's
// "prefer the opposite bit" and CountXorBelow's range count - read those counts to see only what
// is live. Built for the members of the Maximum XOR family that forget values: a sliding window
// (LeetCode 3845), a depth-first walk that undoes on the way back up (1938), and a range count
// (1803).
//
// A new type beside BitTrie (Graph/Engines/Dags/Trees/BitTrie.cs), not a Remove bolted onto it,
// because the two hold different things. BitTrie is insert-only: its structure IS its content -
// a node exists exactly when some inserted value's path passes through it - its Count counts
// Insert calls, and its TryMaxXor walk leans on that, since every node it reaches has a child.
// Here a removal leaves the nodes in place and lowers the counts, so a node's existence says only
// that some value once passed through it; liveness is the count, and every walk reads it.
// Putting a count on BitTrie would hand its insert-only users (LeetCode 421, 1707, 2932, 2935) a
// count per node and a count test per step that none of them reads. BitTrie, its
// BitTrieNode/BitTrieChildren/BitTrieTopology witness and every one of those users are untouched.
//
// No ITreeTopology witness, for both reasons ARCHITECTURE.md §13.9's test names. Nodes are slots
// in shared arrays, not objects with an identity of their own - the Heap/DisjointSet case, not
// BitTrieNode's. And the structural tree is not the content: after a removal it still holds every
// path ever inserted, so a generic Tree-tier engine walking it (TreeMetrics.Size, say) would
// count values that are gone.
//
// Index-addressed arrays rather than one object per node: _children keeps a node's two child
// slots side by side at node * 2 + bit, so a walk picks a child by arithmetic on the bit instead
// of branching between two fields, and _counts keeps its live count; the two double together when
// full. Slot 0 is a sentinel for "no node" - count 0, both child slots pointing back at itself -
// so a walk that steps off every inserted path keeps reading zeros instead of testing for an
// absent child at each level. Nodes are never reclaimed: storage grows with the distinct paths
// ever inserted (at most 32 nodes per value), not with Count.
//
// Measured side by side on the three solutions' operation mixes (BenchmarkDotNet medium job, 200
// to 20,000 values): the counts the solutions kept in a HashMap<BitTrieNode, int> cost 5-10x
// against the same class nodes carrying the count in a field, and these arrays then run 15-40%
// faster than those nodes from 2,000 values up, within 15% either way at 200.
//
// Every XOR here is read as an UNSIGNED 32-bit pattern, BitTrie.TryMaxXor's reading: TryMaxXor
// maximizes the unsigned pattern and CountXorBelow compares unsigned. For non-negative values
// both agree with signed arithmetic, and a caller's high + 1 limit stays right even at
// high = int.MaxValue, where it wraps to the unsigned 2^31.
//
// Insert, TryRemove, TryMaxXor and CountXorBelow each walk the 32 levels once (TryRemove twice):
// O(1), Insert's amortized over the doubling.
internal sealed class CountedBitTrie
{
    private const int BitWidth = 32;
    private const int ChildSlotsPerNode = 2;

    // The sentinel's slot, and the value an empty child slot holds - the root is slot 1, so no
    // real node is ever a child at slot 0.
    private const int Absent = 0;
    private const int Root = 1;

    // The sentinel, the root and one node per bit level: exactly what the first Insert lays down.
    private const int InitialNodeCapacity = BitWidth + 2;

    private int[] _children = new int[InitialNodeCapacity * ChildSlotsPerNode];
    private int[] _counts = new int[InitialNodeCapacity];
    private int _nodeCount = Root + 1;

    // How many values are live, duplicates included - every live value's path starts at the root.
    public int Count => _counts[Root];

    public void Insert(int value)
    {
        var bits = unchecked((uint)value);
        var node = Root;
        _counts[Root]++;

        for (var level = BitWidth - 1; level >= 0; level--)
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
    public bool TryRemove(int value)
    {
        if (_counts[LeafOf(value)] == 0)
        {
            return false;
        }

        var bits = unchecked((uint)value);
        var node = Root;
        _counts[Root]--;

        for (var level = BitWidth - 1; level >= 0; level--)
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

        for (var level = BitWidth - 1; level >= 0; level--)
        {
            var bit = BitAt(bits, level);
            node = _children[SlotOf(node, bit)];
        }

        return node;
    }

    // The largest value ^ y over every live y, as an unsigned pattern cast back to int; false
    // when nothing is live.
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

        for (var level = BitWidth - 1; level >= 0; level--)
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
    public int CountXorBelow(int value, int limit)
    {
        var valueBits = unchecked((uint)value);
        var limitBits = unchecked((uint)limit);
        var node = Root;
        var count = 0;

        for (var level = BitWidth - 1; level >= 0; level--)
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

    private static uint BitAt(uint bits, int level) => (bits >> level) & 1u;

    // A node's child for bit sits at node * 2 + bit, its two slots side by side.
    private static int SlotOf(int node, uint bit) => (node * ChildSlotsPerNode) + (int)bit;
}
