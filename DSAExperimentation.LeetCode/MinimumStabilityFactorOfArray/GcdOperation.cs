using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.MinimumStabilityFactorOfArray;

// LC 3605's own ICombineOperation witness: gcd is associative, and gcd(0, x) = x
// makes 0 the identity element a "no overlap" query branch needs to be invisible
// to the result - the same role MinOperation's Element.MaxValue plays for min.
// Meaningless outside this problem's range-gcd queries, so it stays in this
// problem's own folder rather than DataStructures/SegmentTree/ alongside
// Min/Max/SumOperation (ARCHITECTURE.md 17.3: a witness answering one problem and
// nothing else belongs in LeetCode/, not the tier it's built from). Reused as a
// plain function by the brute-force baseline too, so both strategies fold windows
// with the exact same gcd - only how a window's combined value is *obtained*
// (rescan vs. range query) differs between them.
internal readonly struct GcdOperation : ICombineOperation<int>
{
    public static int Identity => 0;

    public static int Combine(int left, int right)
    {
        while (right != 0)
        {
            (left, right) = (right, left % right);
        }

        return left;
    }
}
