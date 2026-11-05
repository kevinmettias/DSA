using DSAExperimentation.Domain.Mathematics.Algebra;

namespace DSAExperimentation.LeetCode.Solutions.NumberOfLongestIncreasingSubsequence;

// The SegmentTree<Element,IMonoid<Element>> witness
// NumberOfLongestIncreasingSubsequenceSolution keys by rank. Identity is (0,0):
// "no candidate yet" must combine away to whatever real value it's paired with,
// the same Maximum<Element>.Identity = MinValue shape. Combine prefers the
// longer length outright; on a tie it sums counts - the standard "argmax with
// tie-count" aggregate, associative/commutative the same way Maximum's
// plain max already is, since the result only ever depends on the overall max
// length among everything combined and the summed count of whichever elements
// attain it, never on grouping order. This answers LC 673 alone, which is why it
// lives beside the solution rather than in the library.
internal readonly struct LisAggregate : IMonoid<(int Length, int Count)>
{
    public static (int Length, int Count) Identity => (0, 0);

    public static (int Length, int Count) Combine((int Length, int Count) left, (int Length, int Count) right)
    {
        if (left.Length != right.Length)
        {
            return left.Length > right.Length ? left : right;
        }

        return (left.Length, left.Count + right.Count);
    }
}
