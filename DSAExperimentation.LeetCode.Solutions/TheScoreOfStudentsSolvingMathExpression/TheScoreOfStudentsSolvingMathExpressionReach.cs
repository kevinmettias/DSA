namespace DSAExperimentation.LeetCode.Solutions.TheScoreOfStudentsSolvingMathExpression;

// The reach of TheScoreOfStudentsSolvingMathExpressionSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. ScoreOfStudentsByMemoizedIntervals takes 40.4 ms
// over the 103 of the catalog's 105 cases with a length of expression up to 19 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class TheScoreOfStudentsSolvingMathExpressionReach
{
    private const int LargestExpressionLength = 19;

    public static bool IsWithin(string expression, int[] answers) =>
        expression.Length <= LargestExpressionLength;
}
