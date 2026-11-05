namespace DSAExperimentation.LeetCode.Solutions.CheckIfThereIsAValidParenthesesStringPath;

// LC 2267's promise about its input: the grid is m x n, every row as long as the first, and each cell holds
// '(' or ')'. LeetCode's judge never poses an input that breaks it. Both strategies walk a rectangular grid,
// which rows of different lengths do not fill. Generated cases do not always keep the promise, so a harness
// that runs them asks this first.
internal static class CheckIfThereIsAValidParenthesesStringPathPromise
{
    private const string Parentheses = "()";

    public static bool IsKeptBy(char[][] grid)
    {
        var columnCount = grid.FirstOrDefault()?.Length ?? 0;

        return grid.All(row => row.Length == columnCount && row.All(Parentheses.Contains));
    }
}
