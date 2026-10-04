using DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleII;

namespace DSAExperimentation.LeetCode.FindTheNumberOfWaysToPlacePeopleI;

// The order LC 3025's sorted sweep is defined over: points by x ascending, y
// descending on ties. LC 3027 owns the sweep and the rule it rests on
// (ARCHITECTURE 17.3), so this is that rule under this problem's name - kept because
// this problem's benchmark sorts its prepared input with it in [GlobalSetup].
internal static class PointOrder
{
    public static IComparer<int[]> ByXThenDescendingY => XThenDescendingYOrder.Rule;
}
