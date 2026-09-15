using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NthMagicalNumber;

// LC 878's binary search runs over a virtual monotone sequence rather than a
// materialized array: index i's "value" is whether at least n magical numbers are
// at or below i, counted by inclusion-exclusion over the multiples of a, of b, and
// of lcm(a, b). This witness answers that one question for that one problem, so it
// lives beside the solution rather than in DataStructures/Sequence with
// IRandomAccessSequence's other implementations - the same placement SqrtX's
// SquareExceedsSequence gets.
internal readonly struct MagicalCountSequence(int n, int a, int b, long lcm, int upperBound)
    : IRandomAccessSequence<int>
{
    public int Length => upperBound + 1;

    public int Get(int index)
    {
        var multiplesUpToIndex = (index / a) + (index / b) - (index / lcm);

        return multiplesUpToIndex >= n ? 1 : 0;
    }
}
