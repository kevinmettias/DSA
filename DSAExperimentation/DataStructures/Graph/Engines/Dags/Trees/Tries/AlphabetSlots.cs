using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Children keyed by an alphabet's letters: a mask with a bit per letter that has a child, and the slot
// array the layout keeps those children in. The store a trie injects when its keys are drawn from a
// small alphabet, which buys two things a hash map cannot give. A child is found in O(1) worst case, by
// its letter's slot. And the children are ordered, by letter, so the trie has a tree-topology witness.
//
// The mask is kept under every layout, so a child count is one popcount and a child of a given rank is
// found without scanning empty slots; the layout says only where a letter's child sits and how the
// array grows. Dense gives every letter its own slot; Packed stores only the children present, in letter
// order, and finds a letter's by counting the mask's bits below it. A child is read by its letter and
// added with GetOrAddChild, never by writing a slot, because under a packed layout a slot holds whichever
// child ranks there. The mask is 64 bits, so an alphabet of more than 64 letters has no store here.
/// <summary>A trie node's children keyed by an alphabet's letters, at most 64, in slots the layout arranges.</summary>
/// <typeparam name="TAlphabet">The letters a key is drawn from, and the slot each takes.</typeparam>
/// <typeparam name="TLayout">Where in the slot array each child is kept.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public struct AlphabetSlots<TAlphabet, TLayout, TValue>
    : IOrderedTrieChildStore<AlphabetSlots<TAlphabet, TLayout, TValue>, TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>>,
        ILetteredTrieChildStore<AlphabetSlots<TAlphabet, TLayout, TValue>, TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>, char>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    private const int MaxAlphabetSize = sizeof(ulong) * 8;
    private const string AlphabetTooLargeMessage = "A trie's slots keep one mask bit per letter, so its alphabet has at most 64 letters.";

    private static readonly TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[] NoSlots = [];

    private ulong _present;
    private TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[] _slots;

    /// <inheritdoc/>
    public readonly int Count => BitOperations.PopCount(_present);

    /// <summary>Gets the node's child mask: bit <c>i</c> is set when the letter in slot <c>i</c> of the alphabet has a child.</summary>
    public readonly ulong Letters => _present;

    private AlphabetSlots(TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[] slots) => _slots = slots;

    /// <inheritdoc/>
    public static AlphabetSlots<TAlphabet, TLayout, TValue> Empty()
    {
        if (TAlphabet.Size > MaxAlphabetSize)
        {
            throw new NotSupportedException(AlphabetTooLargeMessage);
        }

        var capacity = TLayout.InitialCapacity(TAlphabet.Size);

        return new AlphabetSlots<TAlphabet, TLayout, TValue>(SlotsOf(capacity));
    }

    /// <inheritdoc/>
    public readonly TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>? ChildFor(char key) => ChildAt(TAlphabet.IndexOf(key));

    /// <inheritdoc/>
    public TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue> GetOrAddChildFor(char key) => GetOrAddChild(TAlphabet.IndexOf(key));

    /// <summary>Finds the child under a letter.</summary>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns>The child, or <c>null</c> when the letter has none.</returns>
    public readonly TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>? ChildAt(int letter)
    {
        var bit = BitOf(letter);

        if (!TLayout.HasSlotPerLetter && (_present & bit) == 0)
        {
            return null;
        }

        return _slots[TLayout.PositionOf(_present, letter)];
    }

    /// <inheritdoc/>
    public readonly TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue> ChildOfRank(int rank)
    {
        if ((uint)rank >= (uint)Count)
        {
            TrieFailures.ThrowNotAChildRank(rank);
        }

        // presumption: allow -- the rank is below the child count, and the layout stores every child
        // the mask counts, so the slot holds one.
        return _slots[TLayout.PositionOfRank(_present, rank)]!;
    }

    /// <summary>Finds the child under a letter, adding a new one when there is none.</summary>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns>The child under that letter.</returns>
    public TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue> GetOrAddChild(int letter)
    {
        var bit = BitOf(letter);
        var position = TLayout.PositionOf(_present, letter);

        if ((_present & bit) != 0)
        {
            // presumption: allow -- the letter's bit is set, so its child is stored at the position.
            return _slots[position]!;
        }

        var child = new TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>();
        Insert(position, child);
        _present |= bit;

        return child;
    }

    // A full array grows first; then, under a layout that keeps children in letter order, the children
    // from the position on move up one to open it.
    private void Insert(int position, TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue> child)
    {
        var count = Count;
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

    private readonly TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[] Grown(int count)
    {
        var capacity = TLayout.GrownCapacity(count, TAlphabet.Size);
        var grown = SlotsOf(capacity);
        Array.Copy(_slots, grown, count);

        return grown;
    }

    /// <summary>Walks the children in letter order, each with the letter it is stored under.</summary>
    /// <returns>An enumerator over the children present.</returns>
    [UnscopedRef]
    public readonly AlphabetSlotsEnumerator<TAlphabet, TLayout, TValue> GetEnumerator() => new(in this);

    // An array of the given capacity, or the one shared empty array for a layout that starts with none.
    private static TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[] SlotsOf(int capacity)
    {
        if (capacity == 0)
        {
            return NoSlots;
        }

        return new TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue>?[capacity];
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
