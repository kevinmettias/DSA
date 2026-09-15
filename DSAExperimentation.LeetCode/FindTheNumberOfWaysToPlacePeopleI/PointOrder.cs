namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

// The order LC 3025's sorted sweep is defined over: points by x ascending, y
// descending on ties.
//
// Owned here rather than in FindTheNumberOfWaysToPlacePeopleISolution because the
// benchmark sorts its prepared input with this exact rule in [GlobalSetup] - the
// ordering is the sweep's precondition, so both sides have to read one definition
// of it instead of the benchmark restating the comparison.
internal static class PointOrder
{
    public static readonly IComparer<int[]> ByXThenDescendingY = Comparer<int[]>.Create((a, b) =>
    {
        // The x coordinates decide unless they tie, in which case the y comparison
        // runs the other way - the one departure from plain ascending order.
        if (a[0] != b[0])
        {
            return a[0].CompareTo(b[0]);
        }

        return b[1].CompareTo(a[1]);
    });
}
