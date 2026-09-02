using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SqrtX;

// LeetCode 69's binary search runs over a virtual monotone sequence rather than a
// materialized array: index i's "value" is whether i^2 exceeds x. This witness
// answers that one question for that one problem, so it lives beside the solution
// rather than in DataStructures/Sequence with IRandomAccessSequence's other
// implementations.
internal readonly struct SquareExceedsSequence(long x, int length) : IRandomAccessSequence<int>
{
    public int Length => length;

    public int Get(int value) => (long)value * value > x ? 1 : 0;
}
