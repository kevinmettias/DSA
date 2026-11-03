using System.Numerics;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// Every letter has its own slot, so a letter's slot in the alphabet is its position in the array and
// a node never grows or moves a child. A walk that only reads the trie pays one array read per letter,
// which is why a trie built once and read many times picks this layout.
/// <summary>A trie layout with a slot for every letter, read directly by the letter's slot.</summary>
public readonly struct DenseTrieLayout : ITrieLayout
{
    /// <summary>True: every letter has a slot, empty while it has no child.</summary>
    public static bool HasSlotPerLetter => true;

    /// <summary>A slot for every letter of the alphabet.</summary>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns><paramref name="alphabetSize"/>.</returns>
    public static int InitialCapacity(int alphabetSize) => alphabetSize;

    /// <summary>The whole alphabet, which a dense node already has room for.</summary>
    /// <param name="childCount">How many children the node has.</param>
    /// <param name="alphabetSize">How many letters the trie's alphabet has.</param>
    /// <returns><paramref name="alphabetSize"/>.</returns>
    public static int GrownCapacity(int childCount, int alphabetSize) => alphabetSize;

    /// <summary>The letter's own slot.</summary>
    /// <param name="present">The node's child mask, which a dense node does not need to find a slot.</param>
    /// <param name="letter">The letter's slot in the alphabet.</param>
    /// <returns><paramref name="letter"/>.</returns>
    public static int PositionOf(ulong present, int letter) => letter;

    /// <summary>The slot of the letter whose bit is the given rank among the mask's set bits.</summary>
    /// <param name="present">The node's child mask.</param>
    /// <param name="rank">The child's rank in letter order, below the mask's popcount.</param>
    /// <returns>The slot of that child's letter.</returns>
    public static int PositionOfRank(ulong present, int rank)
    {
        var remaining = present;

        for (var skipped = 0; skipped < rank; skipped++)
        {
            remaining &= remaining - 1;
        }

        return BitOperations.TrailingZeroCount(remaining);
    }
}
