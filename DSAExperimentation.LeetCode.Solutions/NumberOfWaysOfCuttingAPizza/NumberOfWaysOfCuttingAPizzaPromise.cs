namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysOfCuttingAPizza;

// LC 1444's promise about its input: the pizza is a rectangle: every row as long as the first.
// LeetCode's judge never poses an input that breaks it. A shorter row is read past its end.
// Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class NumberOfWaysOfCuttingAPizzaPromise
{
    public static bool IsKeptBy(string[] pizza, int pieceCount) => pizza.All(row => row.Length == pizza[0].Length);
}
