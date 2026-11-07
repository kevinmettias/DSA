using System.Runtime.CompilerServices;
using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Children keyed by a binary digit, the store a trie over bits injects. It is dense, a slot for
// each of the two digits read by the digit itself, and it keeps both slots inline, in the node,
// rather than in an array the node points to: a step down reads the child straight from the node,
// with no array between them and no slot count to check, as the hand-written bit trie's Zero and
// One fields did. Holding them as an inline array rather than two named fields lets a walk pick a
// child by arithmetic on the digit instead of branching between two fields.
//
// The node class is a type argument, so one store serves every bit trie whatever its node carries:
// BitTrieNode, which carries nothing, and a node with a count next. The node class names this store
// as its own, and a child is made through the node contract's New.
//
// Its children are ordered, 0 before 1, so its trie has a tree-topology witness (TrieNodeTopology,
// §13.9). A child is read by its digit and added with GetOrAddChildFor; a digit is a slot of
// BinaryDigits, 0 or 1, and any other is the caller's precondition broken, which the inline array
// refuses.
/// <summary>A trie node's children keyed by a binary digit, a slot for each kept inline in the node.</summary>
/// <typeparam name="TNode">The node class the children are.</typeparam>
public struct BinarySlots<TNode>
    : IOrderedTrieChildStore<BinarySlots<TNode>, TNode>, ILetteredTrieChildStore<BinarySlots<TNode>, TNode, int>
    where TNode : class, ITrieNode<TNode, BinarySlots<TNode>>
{
    private const int ZeroDigit = 0;
    private const int OneDigit = 1;

    private DigitSlots _slots;

    /// <inheritdoc/>
    public readonly int Count => SlotCount(ZeroDigit) + SlotCount(OneDigit);

    /// <inheritdoc/>
    public static BinarySlots<TNode> Empty() => default;

    /// <inheritdoc/>
    public readonly TNode? ChildFor(int key) => _slots[key];

    /// <inheritdoc/>
    public TNode GetOrAddChildFor(int key)
    {
        ref var slot = ref _slots[key];

        return slot ??= TNode.New();
    }

    // Rank 0 is the 0 child when there is one and the 1 child otherwise; rank 1, reached only with
    // both, is the 1 child. So a rank's digit is the rank, moved up one when the 0 slot is empty.
    /// <inheritdoc/>
    public readonly TNode ChildOfRank(int rank)
    {
        if ((uint)rank >= (uint)Count)
        {
            TrieFailures.ThrowNotAChildRank(rank);
        }

        var digit = rank + (OneDigit - SlotCount(ZeroDigit));

        // presumption: allow -- the rank is below the child count, and the digit it maps to is a
        // slot the count counted, so the slot holds a child.
        return _slots[digit]!;
    }

    private readonly int SlotCount(int digit) => _slots[digit] is null ? 0 : 1;

    // One slot per binary digit, laid out as an inline array so a digit indexes its slot directly.
    [InlineArray(BinaryDigits.Size)]
    private struct DigitSlots
    {
        private TNode? _first;
    }
}
