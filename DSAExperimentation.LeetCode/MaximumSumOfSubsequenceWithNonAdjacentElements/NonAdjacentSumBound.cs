namespace DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

// The sentinel LC 3165's segment answers use for a boundary combination a segment
// cannot satisfy (Both on a leaf: the same element can't be both "picked as
// leftmost" and "not picked as rightmost").
//
// Comfortably larger in magnitude than any real sum (|nums[i]| <= 1e5, length <=
// 5e4, so |sum| <= 5e9) yet small enough that a few merge levels of
// NegativeInfinity + NegativeInfinity can never approach long's range.
//
// Owned here rather than in NonAdjacentSumNode because the merge operation and the
// solution both name it, which is what makes it a shared bound rather than one
// node's own value.
internal static class NonAdjacentSumBound
{
    public const long NegativeInfinity = -4_000_000_000_000L;
}
