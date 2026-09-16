using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.GoodSubsequenceQueries;

// LC 3901's own ICombineOperation witness: gcd is associative, and gcd(0, x) = x
// makes 0 the identity element both "position isn't a multiple of the modulus" and
// "no overlap" query branches need. Same shape as MinimumStabilityFactorOfArray's own
// GcdOperation (LC 3605) - kept as a separate copy in this problem's folder
// rather than shared, per ARCHITECTURE.md 17.3: a witness answering one problem
// and nothing else belongs in that problem's own LeetCode/ folder, not a tier
// other problems reach into. Reused as a plain function by the brute-force
// baseline too, so both strategies fold the exact same gcd - only how a range's
// combined value is obtained (rescan vs. range query) differs between them.
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
