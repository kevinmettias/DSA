using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's binary search runs over a virtual monotone sequence rather than a
// materialized array: index i's "value" is whether at least rank ugly numbers are at
// or below i. This witness answers that one question for that one problem, so it
// lives beside the solution rather than in DataStructures/Sequence with
// IRandomAccessSequence's other implementations - the same placement NthMagicalNumber's
// MagicalCountSequence gets.
//
// Two values meet here and stay apart: the window searched, whose end is derived below
// rather than passed in - rank multiples of the smallest factor alone already reach
// rank, so it is bounded before the first probe and no caller can hand in a window the
// count does not actually hold over - and the counting rule itself, which is
// MultiplesOfThreeFactors' subject and not this type's.
internal readonly struct UglyCountSequence : IRandomAccessSequence<int>
{
    private readonly int _target;
    private readonly int _upperBound;
    private readonly MultiplesOfThreeFactors _multiples;

    public int Length => _upperBound + 1;

    public UglyCountSequence(int rank, int firstFactor, int secondFactor, int thirdFactor)
    {
        _target = rank;
        var smallestFactor = Math.Min(firstFactor, secondFactor);
        _upperBound = checked((int)((long)rank * Math.Min(smallestFactor, thirdFactor)));
        _multiples = new MultiplesOfThreeFactors(firstFactor, secondFactor, thirdFactor, _upperBound);
    }

    // Get(index) treats index itself as the candidate ugly number, the same
    // "value doubles as index" shape MagicalCountSequence uses.
    public int Get(int index)
    {
        var count = _multiples.CountUpTo(index);

        return count >= _target ? 1 : 0;
    }
}
