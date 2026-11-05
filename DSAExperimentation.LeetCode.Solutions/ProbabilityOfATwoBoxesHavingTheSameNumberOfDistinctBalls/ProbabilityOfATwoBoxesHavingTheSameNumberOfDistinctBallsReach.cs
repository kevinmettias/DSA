namespace DSAExperimentation.LeetCode.Solutions.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

// The reach of ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution's strategies (ARCHITECTURE
// 17.13): the catalog cases every one of them finishes in the harness's time. GetProbabilityByBacktracking
// takes 13.6 ms over the 77 of the catalog's 80 cases with a length of balls up to 7 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsReach
{
    private const int LargestBallsLength = 7;

    public static bool IsWithin(int[] balls) =>
        balls.Length <= LargestBallsLength;
}
