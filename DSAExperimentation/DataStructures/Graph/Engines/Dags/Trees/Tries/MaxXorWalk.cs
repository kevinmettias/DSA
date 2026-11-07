using System.Runtime.CompilerServices;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The maximum-XOR walk of a trie over an int's bits, written once for every bit trie: from the top
// bit down, take the child of the opposite digit wherever it holds a value, which sets that bit of
// the XOR, and the child of the query's own digit otherwise. Which children hold a value is the
// node payload's answer (IBitTrieNode.IsLive): BitTrie's present children, CountedBitTrie's
// children with a positive count.
//
// Precondition: the root holds a value. Every node on the walk then holds one too, since a node
// holds a value exactly when one of its children does: where the opposite child holds none, the
// query's own child holds the values the node does, and the walk never steps onto an empty node.
// The walk looks for the query's complement, the partner every bit of whose XOR would be 1, and
// each digit is read from its own key, so both arrive as the top bit shifted down, which the
// store's two slots need no check for. The XOR is built from the top: shifted up one per level, its
// new low bit is 1 exactly where the walk went the opposite way.
//
// A node class is a reference type, so this code is shared across node classes wherever it is not
// inlined, and its calls on the node and the store would be lookups at run time. It is always
// inlined, into a method each configuration compiles on its own for its exact node class (§13.8).
/// <summary>
/// The maximum-XOR walk down a bit trie, over any node class that says which children hold a value.
/// </summary>
/// <typeparam name="TNode">The node class.</typeparam>
internal static class MaxXorWalk<TNode>
    where TNode : class, IBitTrieNode<TNode>
{
    // The largest unsigned XOR of the key with a value held under root, given the key's complement.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Pattern(TNode root, Int32BitsKey key, Int32BitsKey complement)
    {
        var current = root;
        var xor = 0u;

        for (var position = 0; position < Int32BitsKey.Bits; position++)
        {
            var opposite = current.Children.ChildFor(complement.LetterAt(position));
            xor <<= 1;

            if (!TNode.IsLive(opposite))
            {
                // presumption: allow -- the opposite child holds no value, and every node on the
                // walk holds one (see the precondition), so the query's own child holds it.
                current = current.Children.ChildFor(key.LetterAt(position))!;
                continue;
            }

            xor |= 1;
            current = opposite;
        }

        return xor;
    }
}
