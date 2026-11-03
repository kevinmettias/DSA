namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// How a trie node keeps its children: where in the node's slot array a letter's child sits, how many
// slots the array has, and whether adding a child moves the ones after it. Every node also keeps a
// mask with one bit per letter it has a child for, under every layout, so counting children is a
// popcount whichever layout is chosen; the layout decides only where each child is stored.
//
// Dense gives every letter its own slot: lookup is one array read, at the cost of a full array per
// node. Packed stores only the children present, in letter order, found by counting the mask's bits
// below the letter: less memory, a popcount per lookup, and a shift on insert. Which wins depends on
// the workload (IMPROVEMENT-WORKLIST, 2026-11-02), so each trie names its layout as a type argument
// and compiles only that layout's arithmetic. The members answer in integers, not in nodes, so a
// layout is not generic over the node type and nothing about it is looked up at run time.
/// <summary>Where a trie node stores each child in its slot array, and how that array grows.</summary>
public interface ITrieLayout
{
    /// <summary>Whether every letter has a slot of its own, left null while the letter has no child.</summary>
    static abstract bool HasSlotPerLetter { get; }

    /// <summary>The slots a new node starts with.</summary>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns>The starting slot count.</returns>
    static abstract int InitialCapacity(int alphabetSize);

    /// <summary>The slot count a full node grows to when it takes one more child.</summary>
    /// <param name="childCount">How many children the node has, which is how many slots it has.</param>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns>The grown slot count, more than <paramref name="childCount"/>.</returns>
    static abstract int GrownCapacity(int childCount, int alphabetSize);

    /// <summary>The slot that holds, or would hold, a letter's child.</summary>
    /// <param name="present">The node's mask, bit <c>i</c> set when the letter in slot <c>i</c> of the alphabet has a child.</param>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns>The position in the node's slot array.</returns>
    static abstract int PositionOf(ulong present, int letter);

    /// <summary>The slot that holds a node's child of a given rank in letter order.</summary>
    /// <param name="present">The node's mask, bit <c>i</c> set when the letter in slot <c>i</c> of the alphabet has a child.</param>
    /// <param name="rank">The child's rank among the node's children, from 0 for the one with the lowest letter.</param>
    /// <returns>The position in the node's slot array.</returns>
    static abstract int PositionOfRank(ulong present, int rank);
}
