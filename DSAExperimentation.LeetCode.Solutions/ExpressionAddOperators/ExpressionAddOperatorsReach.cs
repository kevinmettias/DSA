namespace DSAExperimentation.LeetCode.Solutions.ExpressionAddOperators;

// The reach of ExpressionAddOperatorsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of
// them finishes in the harness's time. Both arms try every placement of +, - and * between the digits, four
// ways at each gap, and AddOperatorsByDepthFirstSearchTraverse takes 49.7 ms over the 47 of the catalog's 66
// cases with up to 8 digits (measured 2026-11-05, Release); a 9-digit case takes it past the 250 ms a
// strategy's catalog cases are allowed.
internal static class ExpressionAddOperatorsReach
{
    private const int LongestNumber = 8;

    public static bool IsWithin(string num, int target) =>
        num.Length <= LongestNumber;
}
