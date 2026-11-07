using System.Runtime.CompilerServices;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The one trie's walk down a key, written once for every configuration: one edge per letter, each
// child found, or added, by the letter in the node's store. A configuration supplies the node
// class, which carries its payload, the store, and the letter type that joins a key to the store;
// what a key's end means, a value or a count or nothing, is the configuration's own. A trie whose
// edges carry runs of letters, the suffix tree, walks them with Find's edge-label overload.
//
// Each method is generic over the key it walks, so it is compiled for a configuration's own key, a
// text key's loop bounded by its string's length and read with no bounds check as a foreach over
// the string is, and for a KeyPrefix of it. A node class is a reference type, so this class's code
// is shared across node classes when it is not inlined, and in shared code a call on the node or
// the store through their contracts is a lookup at run time. Each method is therefore always
// inlined into its caller, a configuration whose node class is exact, where those calls are direct.
/// <summary>The trie's walk down a key, over any node class and child store.</summary>
/// <typeparam name="TNode">The node class, which carries the configuration's payload.</typeparam>
/// <typeparam name="TStore">How each node keeps its children.</typeparam>
/// <typeparam name="TLetter">One letter of a key, the label on one edge.</typeparam>
internal static class TrieWalk<TNode, TStore, TLetter>
    where TNode : class, ITrieNode<TNode, TStore>
    where TStore : struct, ILetteredTrieChildStore<TStore, TNode, TLetter>
{
    // The node a key's path ends on, adding every node the path lacks.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode GetOrAddPath<TKey>(TNode root, TKey key)
        where TKey : struct, ITrieKey<TLetter>
    {
        var current = root;

        for (var position = 0; position < key.Length; position++)
        {
            current = current.Children.GetOrAddChildFor(key.LetterAt(position));
        }

        return current;
    }

    // The same walk, applying a step to each node on the path, the root first: how a payload kept
    // along the path, a count, follows a key in or out. On a path already laid down it adds
    // nothing, so a step that undoes another takes a key back out along the path it went in by.
    // The walk without a step stays a method of its own, so that no configuration without a
    // payload on its path inlines a step it does not take.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode GetOrAddPath<TKey, TStep>(TNode root, TKey key)
        where TKey : struct, ITrieKey<TLetter>
        where TStep : struct, ITrieStep<TNode>
    {
        TStep.Apply(root);
        var current = root;

        for (var position = 0; position < key.Length; position++)
        {
            current = current.Children.GetOrAddChildFor(key.LetterAt(position));
            TStep.Apply(current);
        }

        return current;
    }

    // The node a key's path ends on, or null where the path stops short of it.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Find<TKey>(TNode root, TKey key)
        where TKey : struct, ITrieKey<TLetter>
    {
        var current = root;

        for (var position = 0; position < key.Length; position++)
        {
            current = current.Children.ChildFor(key.LetterAt(position));

            if (current is null)
            {
                return null;
            }
        }

        return current;
    }

    // The node on whose edge a key's path ends, in a trie whose edges each carry a run of letters,
    // or null where the path leaves the trie. The store finds a child by the first letter on its
    // edge, so the walk matches the rest of the edge here, as far as the key reaches. A key that
    // ends partway along an edge is a prefix of every key below it, so the node below is the one
    // returned. The key is a span of letters, read from the first: a substring index's pattern
    // arrives as one, so a query copies nothing. An ITrieKey cannot carry a span, since only a
    // ref struct holds one, and a walk generic over a ref struct key needs `allows ref struct`,
    // which the repository's lint cannot yet parse (ARCHITECTURE §13.8).
    //
    // A trie of one letter per edge matches nothing past the first, and that walk is Find above,
    // kept a method of its own so that no such configuration inlines an edge match it never takes,
    // as GetOrAddPath keeps its step to itself.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TNode? Find<TLabels>(TNode root, ReadOnlySpan<TLetter> key, TLabels labels)
        where TLabels : struct, ITrieEdgeLabels<TNode, TLetter>
    {
        var current = root;
        var position = 0;

        while (position < key.Length)
        {
            current = current.Children.ChildFor(key[position]);

            if (current is null || !IsEdgeMatched(current, key, position, labels))
            {
                return null;
            }

            position = Math.Min(position + labels.LengthOf(current), key.Length);
        }

        return current;
    }

    // Whether the edge into a node agrees with the key from a position, as far as the key reaches:
    // its first letter is the one the store found the node by, so the match starts at the second.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsEdgeMatched<TLabels>(
        TNode node, ReadOnlySpan<TLetter> key, int position, TLabels labels)
        where TLabels : struct, ITrieEdgeLabels<TNode, TLetter>
    {
        var end = Math.Min(position + labels.LengthOf(node), key.Length);

        for (var next = position + 1; next < end; next++)
        {
            if (!labels.HasLetterAt(node, next - position, key[next]))
            {
                return false;
            }
        }

        return true;
    }
}
