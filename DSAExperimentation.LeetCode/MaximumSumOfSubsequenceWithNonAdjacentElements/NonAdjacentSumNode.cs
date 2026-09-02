namespace DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

// One segment's answer to "the best non-adjacent-sum subsequence", broken out
// by whether the segment's own leftmost/rightmost element is forced into the
// pick - the four combinations a merge needs to keep a picked boundary element
// from ever counting a neighbor across the join twice. NegativeInfinity marks a
// combination a single element cannot satisfy (Both on a leaf: the same
// element can't be both "picked as leftmost" and "not picked as rightmost").
// This answers LC 3165 alone, which is why it lives beside the solution
// rather than in Domain.
internal readonly record struct NonAdjacentSumNode(long Neither, long LeftOnly, long RightOnly, long Both)
{
    // Comfortably larger in magnitude than any real sum (|nums[i]| <= 1e5,
    // length <= 5e4, so |sum| <= 5e9) yet small enough that a few merge levels
    // of NegativeInfinity + NegativeInfinity can never approach long's range.
    public const long NegativeInfinity = -4_000_000_000_000L;

    public long BestSum => Math.Max(Math.Max(Neither, LeftOnly), Math.Max(RightOnly, Both));

    public static NonAdjacentSumNode Leaf(long value) => new(0, NegativeInfinity, NegativeInfinity, value);
}
