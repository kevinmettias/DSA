namespace DSAExperimentation.LeetCode.Solutions.NumberOfSquarefulArrays;

// The reach of NumberOfSquarefulArraysSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. CountSquarefulPermsByFullPermutationFilter takes 17.7 ms over the
// 34 of the catalog's 99 cases with a length of nums up to 7 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfSquarefulArraysReach
{
    private const int LargestNumsLength = 7;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
