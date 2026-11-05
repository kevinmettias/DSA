namespace DSAExperimentation.LeetCode.Solutions.CheckIfAParenthesesStringCanBeValid;

// LC 2116's promise about its input: the string and its lock mask are the same length, n each. LeetCode's
// judge never poses an input that breaks it. The reachable-count table is sized by one and indexed by the
// other. Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class CheckIfAParenthesesStringCanBeValidPromise
{
    public static bool IsKeptBy(CheckIfAParenthesesStringCanBeValidSolution.ParenthesisString bracketString, CheckIfAParenthesesStringCanBeValidSolution.LockMask lockMask) => bracketString.Text.Length == lockMask.Digits.Length;
}
