namespace DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

// One segment's answer to "the best non-adjacent-sum subsequence", broken out
// by whether the segment's own leftmost/rightmost element is forced into the
// pick - the four combinations a merge needs to keep a picked boundary element
// from ever counting a neighbor across the join twice. The sentinel
// NonAdjacentSumBound marks a combination a single element cannot satisfy (Both
// on a leaf: the same element can't be both "picked as leftmost" and "not picked
// as rightmost"). This answers LC 3165 alone, which is why it lives beside the
// solution rather than in Domain.
internal readonly record struct NonAdjacentSumNode(long Neither, long LeftOnly, long RightOnly, long Both)
{
    public long BestSum
    {
        get
        {
            var bestWithLeft = Math.Max(Neither, LeftOnly);
            var bestWithRight = Math.Max(RightOnly, Both);
            return Math.Max(bestWithLeft, bestWithRight);
        }
    }

    public static NonAdjacentSumNode Leaf(long value) => new(
        0, NonAdjacentSumBound.NegativeInfinity, NonAdjacentSumBound.NegativeInfinity, value);
}
