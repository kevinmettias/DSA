using DSAExperimentation.Domain.Computing.Binary;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A hand-specialized strategy for exactly one layer, the maximum-XOR set (IMaxXorSet), beside the
// one trie's configuration of it, BitTrie (the owner's ruling of 2026-11-06, ARCHITECTURE §13.9).
// It keeps that layer's invariants and nothing more: Count counts distinct values, HasKey and
// HasPrefix answer for inserted values and their high bits, and TryMaxXor gives the largest
// unsigned XOR with a held value. MaxXorSetContractTests and TrieContractTests hold it and the
// template to the same answers, and MaxXorSetInsertBenchmarks and MaxXorSetQueryBenchmarks compare
// them.
//
// What it specializes away is the template's layers: no key witness, no child store, no node
// contract and no shared walk. A node keeps its two children in two named fields, Zero and One, and
// each walk is written out for an int's 32 bits, most significant first, choosing a field by the
// bit. Its code is a few small methods with nothing generic in them, so a caller that inlines the
// whole of a bucket sweep, as LC 2932's does, inlines the set too, where the template's layered
// walk runs out of the JIT's inlining budget and is left as a call. What it gives up is the
// template's indexed child slots, which pick a child without branching on the bit and make large
// sets faster (LC 421, 1707 and 2935 keep the template).
//
// The set is a struct over its contents, a class holding the root and the count, so copies share
// one set and code generic over IMaxXorSet calls it directly. Make one with Empty; a default value
// holds no contents.
/// <summary>A maximum-XOR set as a bit trie whose nodes keep their two children in named fields, its walks written out for ints.</summary>
public readonly struct TwoFieldBitTrie : IMaxXorSet<TwoFieldBitTrie>
{
    private const int LastBit = BitWidths.Int32 - 1;
    private const string NotAPrefixLengthMessage = "Expected a prefix length from 0 to 32 bits.";

    private readonly Contents _contents;

    /// <inheritdoc/>
    public int Count => _contents.Count;

    private TwoFieldBitTrie(Contents contents) => _contents = contents;

    /// <inheritdoc/>
    public static TwoFieldBitTrie Empty() => new(new Contents());

    // Every value's path is 32 nodes long, so a value is new exactly when its last node is: the
    // walk lays down the first 31 levels and counts the value only if the leaf was missing.
    /// <inheritdoc/>
    public void Insert(int value)
    {
        var current = _contents.Root;
        var pattern = unchecked((uint)value);

        for (var i = LastBit; i > 0; i--)
        {
            var bit = (pattern >> i) & 1u;
            current = bit == 0 ? GetOrCreateZero(current) : GetOrCreateOne(current);
        }

        if (IsLeafAdded(current, pattern & 1u))
        {
            _contents.Count++;
        }
    }

    private static Node GetOrCreateZero(Node node) => node.Zero ??= new Node();

    private static Node GetOrCreateOne(Node node) => node.One ??= new Node();

    // Adds the leaf for the last bit under its parent, false when it was already there.
    private static bool IsLeafAdded(Node parent, uint bit)
    {
        if (bit == 0)
        {
            if (parent.Zero is not null)
            {
                return false;
            }

            parent.Zero = new Node();
            return true;
        }

        if (parent.One is not null)
        {
            return false;
        }

        parent.One = new Node();
        return true;
    }

    /// <inheritdoc/>
    public bool HasKey(int value) => FindNode(value, BitWidths.Int32) is not null;

    // The root exists from construction, so a prefix of no bits asks whether any value is held.
    /// <inheritdoc/>
    public bool HasPrefix(int value, int bits)
    {
        if ((uint)bits > BitWidths.Int32)
        {
            throw new ArgumentOutOfRangeException(nameof(bits), bits, NotAPrefixLengthMessage);
        }

        return bits == 0 ? Count > 0 : FindNode(value, bits) is not null;
    }

    /// <inheritdoc/>
    public bool TryMaxXor(int value, out int result)
    {
        if (Count == 0)
        {
            result = 0;
            return false;
        }

        result = unchecked((int)Walk(value));
        return true;
    }

    // Precondition: Count > 0. Every Insert walks all 32 levels and nothing is removed, so every
    // node reached above the last level has a child: with the opposite child absent, the same-bit
    // child is present by elimination between the two.
    private uint Walk(int value)
    {
        var current = _contents.Root;
        var pattern = unchecked((uint)value);
        var xor = 0u;

        for (var i = LastBit; i >= 0; i--)
        {
            var bit = (pattern >> i) & 1u;
            var (next, matchedOpposite) = DescendOneLevel(current, bit);

            if (matchedOpposite)
            {
                xor |= 1u << i;
            }

            current = next;
        }

        return xor;
    }

    private static (Node Next, bool MatchedOpposite) DescendOneLevel(Node current, uint bit)
    {
        var desiredIsOne = bit == 0;
        var desired = desiredIsOne ? current.One : current.Zero;

        if (desired is not null)
        {
            return (desired, true);
        }

        // presumption: allow -- the opposite child is absent, and every node reached here has a
        // child (see Walk's precondition), so the same-bit child is present.
        return ((desiredIsOne ? current.Zero : current.One)!, false);
    }

    // The node the value's top bits lead to, or null where the path stops short of it.
    private Node? FindNode(int value, int bits)
    {
        Node? current = _contents.Root;
        var pattern = unchecked((uint)value);

        for (var i = LastBit; i > LastBit - bits; i--)
        {
            var bit = (pattern >> i) & 1u;
            current = bit == 0 ? current.Zero : current.One;

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    // The set's state, shared by every copy of the struct: the root, which exists from
    // construction, and the count of distinct values.
    private sealed class Contents
    {
        public Node Root { get; } = new();

        public int Count { get; set; }
    }

    // A node: the child taken when the next bit is 0, and the one taken when it is 1, each null
    // until a value takes it.
    private sealed class Node
    {
        public Node? Zero { get; set; }

        public Node? One { get; set; }
    }
}
