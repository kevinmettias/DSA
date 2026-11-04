namespace DSAExperimentation.LeetCode.Solutions.SearchA2DMatrixII;

// LC 240's promise about its input: every row and every column is sorted in ascending order.
// LeetCode's judge never poses an input that breaks it. The staircase search leans on both orders
// the full scan does not need. Generated cases do not always keep the promise, so a harness that
// runs them asks this first.
internal static class SearchA2DMatrixIIPromise
{
    public static bool IsKeptBy(int[][] matrix, int target)
        => matrix.All(IsSorted) && Enumerable.Range(0, matrix[0].Length).All(column => IsSorted([.. matrix.Select(row => row[column])]));

    private static bool IsSorted(int[] values) => values.Zip(values.Skip(1)).All(pair => pair.First <= pair.Second);
}
