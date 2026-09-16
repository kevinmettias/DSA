namespace DSAExperimentation.LeetCode.MinimumCostPathWithAlternatingDirectionsIII;

// LC 4003's four grid moves, each beside the action parity that matches it: right and
// down are the odd-numbered actions' set, left and up the even-numbered ones'. Both
// arms of the solution step by this one table - the BCL Dijkstra because it must, the
// topology because that is how it generates an edge per move - so it is declared once
// here rather than restated beside each.
internal static class AlternatingGridMoveTable
{
    public static readonly (int DeltaRow, int DeltaCol, bool MatchesOddAction)[] Moves =
    [
        (1, 0, true), (0, 1, true),
        (-1, 0, false), (0, -1, false),
    ];
}
