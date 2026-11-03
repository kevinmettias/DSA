using System.Numerics;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// One node of an AlphabetTrie: a mask with a bit per letter that has a child, the slot array the
// layout keeps those children in, and an optional value. The mask is kept under every layout, so a
// child count is one popcount and a child of a given rank is found without scanning empty slots; the
// layout says only where a letter's child sits and how the array grows. A child is read by its letter's
// slot and added with GetOrAddChild, never by writing a slot, because under a packed layout a slot
// holds whichever child ranks there, not the letter's.
//
// The mask is 64 bits, so an alphabet of more than 64 letters has no trie here; AlphabetTrie refuses
// one when it is built.
/// <summary>An <see cref="AlphabetTrie{TAlphabet, TLayout, TValue}"/> node: its children by letter, and an optional value.</summary>
/// <typeparam name="TAlphabet">The letters a key is written in.</typeparam>
/// <typeparam name="TLayout">Where the node stores each child.</typeparam>
/// <typeparam name="TValue">The value a key maps to.</typeparam>
public sealed class AlphabetTrieNode<TAlphabet, TLayout, TValue>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    private const string NotAChildRankMessage = "Expected a rank below the node's child count.";

    private static readonly AlphabetTrieNode<TAlphabet, TLayout, TValue>?[] NoSlots = [];

    private ulong _present;
    private AlphabetTrieNode<TAlphabet, TLayout, TValue>?[] _slots = InitialSlots();

    /// <summary>How many children the node has.</summary>
    public int ChildCount => BitOperations.PopCount(_present);

    /// <summary>The letters the node has a child for: bit <c>i</c> is set when the letter in slot <c>i</c> of the alphabet does.</summary>
    public ulong ChildLetters => _present;

    /// <summary>The node's children in letter order; enumerating it gives each child with its letter.</summary>
    public AlphabetTrieChildren<TAlphabet, TLayout, TValue> Children => new(this);

    /// <summary>Whether a key ends at this node, so that <see cref="Value"/> holds its value.</summary>
    public bool HasValue { get; set; }

    // presumption: allow -- Value is only meaningful once HasValue is true (set by
    // AlphabetTrie.Set); every reader (HasKey/TryGetValue) already checks HasValue first, the same
    // contract Trie.TrieNode<TValue>.Value already uses.
    /// <summary>The value stored for the key ending here, meaningful only when <see cref="HasValue"/> is true.</summary>
    public TValue Value { get; set; } = default!;

    internal AlphabetTrieNode()
    {
    }

    /// <summary>The child a letter leads to.</summary>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns>The child, or null when no path continues with that letter.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a slot of the alphabet.</exception>
    public AlphabetTrieNode<TAlphabet, TLayout, TValue>? ChildAt(int letter)
    {
        var bit = BitOf(letter);

        if (!TLayout.HasSlotPerLetter && (_present & bit) == 0)
        {
            return null;
        }

        return _slots[TLayout.PositionOf(_present, letter)];
    }

    /// <summary>The child of a given rank, counting the node's children in letter order.</summary>
    /// <param name="rank">The rank, from 0 for the child with the lowest letter to <see cref="ChildCount"/> - 1.</param>
    /// <returns>The child.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="rank"/> is not below <see cref="ChildCount"/>.</exception>
    public AlphabetTrieNode<TAlphabet, TLayout, TValue> ChildOfRank(int rank)
    {
        if ((uint)rank >= (uint)ChildCount)
        {
            throw new ArgumentOutOfRangeException(nameof(rank), rank, NotAChildRankMessage);
        }

        // presumption: allow -- the rank is below the child count, and the layout stores every
        // child the mask counts, so the slot holds one.
        return _slots[TLayout.PositionOfRank(_present, rank)]!;
    }

    /// <summary>The child a letter leads to, added first when there is none.</summary>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns>The existing child, or a new empty one now stored under the letter.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="letter"/> is not a slot of the alphabet.</exception>
    public AlphabetTrieNode<TAlphabet, TLayout, TValue> GetOrAddChild(int letter)
    {
        var bit = BitOf(letter);
        var position = TLayout.PositionOf(_present, letter);

        if ((_present & bit) != 0)
        {
            // presumption: allow -- the letter's bit is set, so its child is stored at the position.
            return _slots[position]!;
        }

        var child = new AlphabetTrieNode<TAlphabet, TLayout, TValue>();
        Insert(position, child);
        _present |= bit;

        return child;
    }

    // A full array grows first; then, under a layout that keeps children in letter order, the
    // children from the position on move up one to open it.
    private void Insert(int position, AlphabetTrieNode<TAlphabet, TLayout, TValue> child)
    {
        var count = ChildCount;
        var slots = _slots;

        if (count == slots.Length)
        {
            slots = Grown(count);
        }

        if (!TLayout.HasSlotPerLetter)
        {
            Array.Copy(_slots, position, slots, position + 1, count - position);
        }

        slots[position] = child;
        _slots = slots;
    }

    private AlphabetTrieNode<TAlphabet, TLayout, TValue>?[] Grown(int count)
    {
        var grown = new AlphabetTrieNode<TAlphabet, TLayout, TValue>?[TLayout.GrownCapacity(count, TAlphabet.Size)];
        Array.Copy(_slots, grown, count);

        return grown;
    }

    private static AlphabetTrieNode<TAlphabet, TLayout, TValue>?[] InitialSlots()
    {
        var capacity = TLayout.InitialCapacity(TAlphabet.Size);

        if (capacity == 0)
        {
            return NoSlots;
        }

        return new AlphabetTrieNode<TAlphabet, TLayout, TValue>?[capacity];
    }

    private static ulong BitOf(int letter)
    {
        if ((uint)letter >= (uint)TAlphabet.Size)
        {
            AlphabetFailures.ThrowNotASlot(letter);
        }

        return 1UL << letter;
    }
}
