using System.Numerics;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Only the children present are stored, in letter order, so a letter's child sits at the number of
// present letters below it: a popcount of the mask under the letter's bit. A node starts with no
// slots and doubles as it fills, up to the whole alphabet, and a child added between two others moves
// the later ones up one. A trie that is mostly chains of single children, as long random words make,
// stores one slot per node instead of a slot per letter, which is why a trie built and walked once
// picks this layout.
/// <summary>A trie layout storing only the children present, in letter order, found by popcount.</summary>
public readonly struct PackedTrieLayout : ITrieLayout
{
    private const int FirstCapacity = 1;
    private const int GrowthFactor = 2;

    /// <summary>False: only the letters present have slots.</summary>
    public static bool HasSlotPerLetter => false;

    /// <summary>No slots until the first child.</summary>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns>0.</returns>
    public static int InitialCapacity(int alphabetSize) => 0;

    /// <summary>Double the slots, starting from one, but never more than the alphabet.</summary>
    /// <param name="childCount">How many children the node has, which is how many slots it has.</param>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns>1 for an empty node; otherwise twice <paramref name="childCount"/>, capped at <paramref name="alphabetSize"/>.</returns>
    public static int GrownCapacity(int childCount, int alphabetSize)
    {
        var doubled = Math.Max(FirstCapacity, childCount * GrowthFactor);

        return Math.Min(doubled, alphabetSize);
    }

    /// <summary>How many present letters come before the given one.</summary>
    /// <param name="present">The node's mask, bit <c>i</c> set when the letter in slot <c>i</c> of the alphabet has a child.</param>
    /// <param name="letter">The letter's slot in the alphabet, below 64.</param>
    /// <returns>The count of set bits below bit <paramref name="letter"/>.</returns>
    public static int PositionOf(ulong present, int letter)
    {
        var below = (1UL << letter) - 1;

        return BitOperations.PopCount(present & below);
    }

    /// <summary>The rank itself: children are stored in letter order.</summary>
    /// <param name="present">The node's child mask, which a packed node does not need to find a rank.</param>
    /// <param name="rank">The child's rank in letter order.</param>
    /// <returns><paramref name="rank"/>.</returns>
    public static int PositionOfRank(ulong present, int rank) => rank;
}
