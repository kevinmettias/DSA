namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

// The ordering LC 3027's sweep strategy rests on: points by x ascending, y
// descending on ties. Separate from the solution because the sweep's precondition is
// stated in terms of it - the hoisted overload takes an already-sorted sequence, and
// a benchmark's [GlobalSetup] has to sort with this exact rule - so naming the rule
// is what lets both callers get it without either one duplicating it.
internal static class XThenDescendingYOrder
{
    // A property rather than a field: the type IS the ordering, so it arrives at a
    // call site as the rule itself and not as one of a set of values to choose from.
    public static IComparer<int[]> Rule { get; } = Comparer<int[]>.Create(Compare);

    private static int Compare(int[] left, int[] right)
    {
        // x decides the order; two points sharing an x fall back to y descending.
        var differsInX = left[0] != right[0];
        return differsInX ? left[0].CompareTo(right[0]) : right[1].CompareTo(left[1]);
    }
}
