using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's binary search runs over a virtual monotone sequence rather than a
// materialized array: index i's "value" is whether at least n ugly numbers are at
// or below i. This witness answers that one question for that one problem, so it
// lives beside the solution rather than in DataStructures/Sequence with
// IRandomAccessSequence's other implementations - the same placement NthMagicalNumber's
// MagicalCountSequence gets.
//
// Two values meet here and stay apart: the window searched, whose end is derived below
// rather than passed in - n multiples of the smallest factor alone already reach n, so
// it is bounded before the first probe and no caller can hand in a window the count
// does not actually hold over - and the counting rule itself, which is
// MultiplesOfThreeFactors' subject and not this type's.
internal readonly struct UglyCountSequence : IRandomAccessSequence<int>
{
    private readonly int target;
    private readonly int upperBound;
    private readonly MultiplesOfThreeFactors multiples;

    public int Length => upperBound + 1;

    public UglyCountSequence(int n, int a, int b, int c)
    {
        target = n;
        var smallestFactor = Math.Min(a, b);
        upperBound = checked((int)((long)n * Math.Min(smallestFactor, c)));
        multiples = new MultiplesOfThreeFactors(a, b, c, upperBound);
    }

    // Get(index) treats index itself as the candidate ugly number x, the same
    // "value doubles as index" shape MagicalCountSequence uses.
    public int Get(int index)
    {
        var count = multiples.CountUpTo(index);

        return count >= target ? 1 : 0;
    }
}
