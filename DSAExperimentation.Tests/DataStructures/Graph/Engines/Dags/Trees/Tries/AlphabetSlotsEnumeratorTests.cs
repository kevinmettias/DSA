using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.Domain.Language.English;

namespace DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

// A child's Value is set to its letter's slot as it is added, so each (letter, child) pair the
// enumerator gives can be checked against itself, under both layouts.
public sealed partial class AlphabetSlotsEnumeratorTests
{
    private static readonly int[] OutOfOrder = [25, 2, 0, 13, 1];

    [Fact]
    public void MoveNext_EmptyNode_IsFalse()
    {
        var denseSlots = AlphabetSlots<EnglishLowercase, DenseTrieLayout, int>.Empty();
        var packedSlots = AlphabetSlots<EnglishLowercase, PackedTrieLayout, int>.Empty();
        var dense = new AlphabetSlotsEnumerator<EnglishLowercase, DenseTrieLayout, int>(in denseSlots);
        var packed = new AlphabetSlotsEnumerator<EnglishLowercase, PackedTrieLayout, int>(in packedSlots);

        Assert.False(dense.MoveNext());
        Assert.False(packed.MoveNext());
    }

    [Fact]
    public void MoveNext_ChildrenAddedOutOfOrder_GivesEachInLetterOrderWithItsLetter()
    {
        var expected = OutOfOrder.Order().Select(letter => (letter, letter)).ToArray();

        Assert.Equal(expected, Walk<DenseTrieLayout>());
        Assert.Equal(expected, Walk<PackedTrieLayout>());
    }

    // Each step's letter, and the letter its child was stored under.
    private static (int Letter, int StoredUnder)[] Walk<TLayout>()
        where TLayout : struct, ITrieLayout
    {
        var node = new TrieNode<AlphabetSlots<EnglishLowercase, TLayout, int>, int>();

        foreach (var letter in OutOfOrder)
        {
            node.Children.GetOrAddChild(letter).Value = letter;
        }

        var steps = new List<(int, int)>();
        var walk = new AlphabetSlotsEnumerator<EnglishLowercase, TLayout, int>(in node.Children);

        while (walk.MoveNext())
        {
            steps.Add((walk.Current.Letter, walk.Current.Child.Value));
        }

        return [.. steps];
    }
}
