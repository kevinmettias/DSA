using System.Numerics;
using DSAExperimentation.DataStructures.Alphabets;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Walks a node's children in letter order with the letter each is stored under, reading the node's
// child mask one set bit at a time, so a walk that needs every child's letter visits only the
// children present rather than asking for all of the alphabet's letters in turn.
/// <summary>Enumerates an alphabet-trie node's children in letter order, each with its letter.</summary>
/// <typeparam name="TAlphabet">The letters a key is written in.</typeparam>
/// <typeparam name="TLayout">Where the node stores each child.</typeparam>
/// <typeparam name="TValue">The value a key maps to.</typeparam>
public struct AlphabetTrieChildEnumerator<TAlphabet, TLayout, TValue>
    where TAlphabet : struct, IAlphabet
    where TLayout : struct, ITrieLayout
{
    private readonly AlphabetTrieNode<TAlphabet, TLayout, TValue> _node;
    private ulong _unvisited;

    /// <summary>The current child, with its letter's slot in the alphabet.</summary>
    public (int Letter, AlphabetTrieNode<TAlphabet, TLayout, TValue> Child) Current { get; private set; }

    /// <summary>Starts before the node's first child.</summary>
    /// <param name="node">The node whose children to walk.</param>
    public AlphabetTrieChildEnumerator(AlphabetTrieNode<TAlphabet, TLayout, TValue> node)
    {
        _node = node;
        _unvisited = node.ChildLetters;
    }

    /// <summary>Moves to the child with the next letter.</summary>
    /// <returns>True when there was another child; false after the last.</returns>
    public bool MoveNext()
    {
        if (_unvisited == 0)
        {
            return false;
        }

        var letter = BitOperations.TrailingZeroCount(_unvisited);
        _unvisited &= _unvisited - 1;

        // presumption: allow -- the letter was read from the node's child mask, so it has a child.
        Current = (letter, _node.ChildAt(letter)!);

        return true;
    }
}
