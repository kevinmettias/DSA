namespace DSAExperimentation.LeetCode.Solutions.SearchA2DMatrix;

// LC 74's promise about its input: each row is sorted, and each row starts after the last row ends.
// LeetCode's judge never poses an input that breaks it. The binary search reads the rows as one
// sorted list, which they are not otherwise. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class SearchA2DMatrixPromise
{
    public static bool IsKeptBy(int[][] matrix, int target)
    {
        var values = matrix.SelectMany(row => row).ToArray();

        return values.Zip(values.Skip(1)).All(pair => pair.First <= pair.Second)
            && Enumerable.Range(1, matrix.Length - 1).All(row => matrix[row][0] > matrix[row - 1][^1]);
    }
}
