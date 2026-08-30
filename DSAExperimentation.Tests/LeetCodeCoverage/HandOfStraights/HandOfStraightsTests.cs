using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HandOfStraights;

// LeetCode 846. Hand of Straights: a HashMap<int,int> counts how many of each card
// value remain, this repo's own MergeSort (over an ArrayIndexedSequence<int>, the
// same witness Sorting/MergeSort's own tests use) orders a working copy of the hand,
// then a single ascending sweep greedily starts a fresh groupSize run at every card
// still holding a nonzero count - correct because, in ascending order, a card with
// cards remaining can only be the bottom of some group.
public sealed partial class HandOfStraightsTests
{
    [Fact]
    public void IsNStraightHand_ClassicExample_ReturnsTrue()
    {
        int[] hand = [1, 2, 3, 6, 2, 3, 4, 7, 8];

        var isStraight = IsNStraightHand(hand, groupSize: 3);

        Assert.True(isStraight);
    }

    [Fact]
    public void IsNStraightHand_HandDoesNotDivideEvenlyIntoGroups_ReturnsFalse()
    {
        int[] hand = [1, 2, 3, 4, 5];

        var isStraight = IsNStraightHand(hand, groupSize: 4);

        Assert.False(isStraight);
    }

    private static bool IsNStraightHand(int[] hand, int groupSize)
    {
        if (hand.Length % groupSize != 0)
        {
            return false;
        }

        var counts = new HashMap<int, int>();

        foreach (var card in hand)
        {
            counts.TryGetValue(card, out var count);
            counts.Set(card, count + 1);
        }

        var sortedHand = (int[])hand.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedHand));

        foreach (var card in sortedHand)
        {
            counts.TryGetValue(card, out var remaining);

            if (remaining == 0)
            {
                continue;
            }

            for (var next = card; next < card + groupSize; next++)
            {
                if (!counts.TryGetValue(next, out var nextCount) || nextCount == 0)
                {
                    return false;
                }

                counts.Set(next, nextCount - 1);
            }
        }

        return true;
    }
}
