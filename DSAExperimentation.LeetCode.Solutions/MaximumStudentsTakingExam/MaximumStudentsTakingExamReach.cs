namespace DSAExperimentation.LeetCode.Solutions.MaximumStudentsTakingExam;

// The reach of MaximumStudentsTakingExamSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. MaxStudentsByBruteForceRecursion takes 204.3 ms over the 39 of
// the catalog's 61 cases with an element count of seats up to 36 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MaximumStudentsTakingExamReach
{
    private const int LargestSeatsElements = 36;

    public static bool IsWithin(char[][] seats) =>
        seats.Sum(row => row.Length) <= LargestSeatsElements;
}
