using BitWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BinarySlots<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.BitTrieNode>,
    int>;

using System.Runtime.CompilerServices;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie over an int's bits, as a set: Int32BitsKey reads a value's 32 bits from the most
// significant, BinarySlots keeps a node's two children inline, one per binary digit, and the node,
// BitTrieNode, carries no payload. Every key is 32 letters long, so a node at depth 32 is a key's
// end and no node above it is: whether a value is held is implied by its path, and nothing is
// written at the end of one. Insert, HasKey and HasPrefix are TrieWalk's walk, as the word trie's
// are; the set counts a value the first time its last node is added.
//
// It is the building block of the Maximum XOR of Two Numbers in an Array family (LeetCode 421):
// insert every candidate, then walk a query's bits from the top, at each level taking the child of
// the opposite bit when there is one, which sets that bit of the XOR. That walk, TryMaxXor, is the
// one query this configuration adds, since it needs both a binary alphabet and an int's place
// values. The opposite-bit rule is not a witness: it decides how the query walks a trie whose shape
// it never changes, so a minimum-XOR walk would be a second method here, not a second configuration
// (§13.2's test, the one that keeps IPathHeuristic a query-time axis).
//
// "Maximum" means the largest unsigned bit pattern, which is the largest value for LeetCode 421's
// non-negative inputs, whose bit 31 is always 0. Once negative values are inserted, the pattern
// with bit 31 set is the unsigned maximum but reads back as a negative int, so a caller with
// negative values accounts for that sign itself.
//
// Insert, HasKey and TryMaxXor walk 32 levels each, O(1) bounded by an int's width.
/// <summary>A set of ints in a binary trie over their 32 bits, answering maximum-XOR queries.</summary>
public sealed class BitTrie
{
    private const int LastPosition = Int32BitsKey.Bits - 1;

    private readonly BitTrieNode _root = new();

    /// <summary>Gets how many distinct values are held.</summary>
    public int Count { get; private set; }

    /// <summary>Gets the root, the node every value's path starts from.</summary>
    public BitTrieNode Root => _root;

    /// <summary>Adds a value; adding one already held changes nothing.</summary>
    /// <param name="value">The value to add.</param>
    public void Insert(int value)
    {
        if (TryAddPath(_root, value))
        {
            Count++;
        }
    }

    // The two walks, TryAddPath and MaxXorPattern, are compiled on their own and never inlined. Each
    // inlines TrieWalk and the store, which are generic over the node class, a reference type, and
    // are exact code only where they are inlined into a method whose node class is known. Inlined
    // into a large caller, LC 2932's whole bucket sweep, the walks used up the JIT's inlining budget
    // and the walk and the store's lookups were left as calls into shared generic code, looked up at
    // run time: LC 2932 ran 8-59% slower at 10 and 50 values. Compiled on their own, they always have
    // the walk inlined, at the cost of a direct call each. They take the root rather than the trie,
    // so a caller that inlines Insert and TryMaxXor never lets the trie object escape, and the JIT
    // may still keep it off the heap, as it did LC 421's.
    //
    // A value is new exactly when its last node is: every value's path is 32 nodes long, so a path
    // that already reaches the last level is a value already held.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TryAddPath(BitTrieNode root, Int32BitsKey key)
    {
        var parent = BitWalk.GetOrAddPath(root, new KeyPrefix<Int32BitsKey, int>(key, LastPosition));
        var last = key.LetterAt(LastPosition);

        if (parent.Children.ChildFor(last) is not null)
        {
            return false;
        }

        parent.Children.GetOrAddChildFor(last);
        return true;
    }

    /// <summary>Reports whether a value is held.</summary>
    /// <param name="key">The value to look for.</param>
    /// <returns>True when the value was inserted.</returns>
    public bool HasKey(Int32BitsKey key) => BitWalk.Find(_root, key) is not null;

    // The root exists from construction, so a prefix of no bits asks whether any value is held.
    /// <summary>Reports whether any held value starts with the high bits of a value.</summary>
    /// <param name="key">The value whose high bits make the prefix.</param>
    /// <param name="length">How many of its bits, from the most significant, the prefix takes: 0 to 32.</param>
    /// <returns>True when some held value shares those bits, so a length of 0 asks whether any value is held.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> is negative or more than 32.</exception>
    public bool HasPrefix(Int32BitsKey key, int length)
    {
        var prefix = new KeyPrefix<Int32BitsKey, int>(key, length);

        return length == 0 ? Count > 0 : BitWalk.Find(_root, prefix) is not null;
    }

    /// <summary>
    /// Finds the largest XOR of a value with any value held, comparing results as unsigned bit patterns.
    /// </summary>
    /// <param name="value">The value to XOR against the held ones.</param>
    /// <param name="result">
    /// The largest XOR as an int, so negative when bit 31 is set, or 0 when the trie is empty.
    /// </param>
    /// <returns><c>true</c> when the trie holds a value; <c>false</c> when it is empty.</returns>
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

    // Precondition: Count > 0. Every Insert walks all 32 levels before it returns and nothing is
    // ever removed, so every node this walk reaches above the last level has a child; where the
    // opposite bit's child is absent, the same bit's is present. The walk looks for the query's
    // complement, the partner every bit of whose XOR would be 1, and takes the query's own bit
    // where the complement's is missing. Each digit is read from its own key, so both arrive as the
    // top bit shifted down, which the store's two slots need no check for. The XOR is built from
    // the top: shifted up one per level, its new low bit is 1 exactly where the walk went the
    // opposite way.
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static uint MaxXorPattern(BitTrieNode root, Int32BitsKey key, Int32BitsKey complement)
    {
        var current = root;
        var xor = 0u;

        for (var position = 0; position < Int32BitsKey.Bits; position++)
        {
            var opposite = current.Children.ChildFor(complement.LetterAt(position));
            xor <<= 1;

            if (opposite is null)
            {
                // presumption: allow -- the opposite child is absent, and every node above the last
                // level has a child (see the precondition), so the same bit's child is present.
                current = current.Children.ChildFor(key.LetterAt(position))!;
                continue;
            }

            xor |= 1;
            current = opposite;
        }

        return xor;
    }
}
