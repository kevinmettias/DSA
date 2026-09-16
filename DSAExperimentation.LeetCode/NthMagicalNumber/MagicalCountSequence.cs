using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NthMagicalNumber;

// LC 878's binary search runs over a virtual monotone sequence rather than a
// materialized array: an index's "value" is whether at least rank magical numbers are
// at or below that index, counted by inclusion-exclusion over the multiples of the
// first factor, of the second, and of their lcm. This witness answers that one
// question for that one problem, so it lives beside the solution rather than in
// DataStructures/Sequence with IRandomAccessSequence's other implementations - the
// same placement SqrtX's SquareExceedsSequence gets.
internal readonly struct MagicalCountSequence(int rank, int firstFactor, int secondFactor, long lcm, int upperBound)
    : IRandomAccessSequence<int>
{
    public int Length => upperBound + 1;

    public int Get(int index)
    {
        var multiplesUpToIndex = (index / firstFactor) + (index / secondFactor) - (index / lcm);

        return multiplesUpToIndex >= rank ? 1 : 0;
    }
}
