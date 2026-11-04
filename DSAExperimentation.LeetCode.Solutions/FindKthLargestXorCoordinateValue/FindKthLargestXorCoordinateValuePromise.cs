namespace DSAExperimentation.LeetCode.Solutions.FindKthLargestXorCoordinateValue;

// LC 1738's promise about its input: k is at most the matrix's m * n values. LeetCode's judge never
// poses an input that breaks it. Sorting every value and reading the kth reads past the end.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class FindKthLargestXorCoordinateValuePromise
{
    public static bool IsKeptBy(int[][] matrix, int rank) => rank >= 1 && rank <= matrix.Sum(row => row.Length);
}
