using System.Numerics;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Walks a node's children in letter order with the letter each is stored under, reading the child mask
// one set bit at a time, so a walk that needs every child's letter visits only the children present
// rather than asking for all of the alphabet's letters in turn.
//
// It holds a reference to the store, inside the node, rather than a copy of it. A copy is two more
// values - the slot array and the mask - that a recursive walk keeps alive across every call it makes
// from inside the loop, so the JIT spills them to the stack and reloads them each step: LC 676's
// search grew from 590 to 912 bytes and ran 10-15% slower with a copy. Through the reference the loop
// carries one pointer, as it did when the walk held the node itself.
/// <summary>Walks a slot store's children in letter order, each with its letter.</summary>
/// <typeparam name="TAlphabet">The letters a key is drawn from.</typeparam>
/// <typeparam name="TLayout">Where in the slot array each child is kept.</typeparam>
/// <typeparam name="TValue">The value the trie stores under each key.</typeparam>
public ref struct AlphabetSlotsEnumerator<TAlphabet, TLayout, TValue>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    private readonly ref readonly AlphabetSlots<TAlphabet, TLayout, TValue> _slots;
    private ulong _unvisited;

    /// <summary>Gets the child the walk stands on, with the letter it is stored under.</summary>
    public (int Letter, TrieNode<AlphabetSlots<TAlphabet, TLayout, TValue>, TValue> Child) Current { get; private set; }

    /// <summary>Starts a walk over a store's children.</summary>
    /// <param name="slots">The store whose children to walk.</param>
    public AlphabetSlotsEnumerator(ref readonly AlphabetSlots<TAlphabet, TLayout, TValue> slots)
    {
        _slots = ref slots;
        _unvisited = slots.Letters;
    }

    /// <summary>Moves to the next child in letter order.</summary>
    /// <returns>True while a child remains.</returns>
    public bool MoveNext()
    {
        if (_unvisited == 0)
        {
            return false;
        }

        var letter = BitOperations.TrailingZeroCount(_unvisited);
        _unvisited &= _unvisited - 1;

        // presumption: allow -- the letter was read from the store's child mask, so it has a child.
        Current = (letter, _slots.ChildAt(letter)!);
        return true;
    }
}
