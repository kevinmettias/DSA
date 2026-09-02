using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.PowerOfFour;

// LeetCode 342's binary search runs over a virtual, already-sorted sequence of
// every power of four that fits in a 32-bit int (4^0..4^15) rather than a
// materialized array: index i's "value" is 4^i via a single left shift. This
// witness answers that one question for that one problem, so it lives beside
// the solution rather than in DataStructures/Sequence with
// IRandomAccessSequence's other implementations.
internal readonly struct PowersOfFourSequence : IRandomAccessSequence<int>
{
    // Number of powers of four representable in a non-negative int (4^0..4^15);
    // 4^16 already overflows.
    public int Length => 16;

    public int Get(int index) => 1 << (2 * index);
}
