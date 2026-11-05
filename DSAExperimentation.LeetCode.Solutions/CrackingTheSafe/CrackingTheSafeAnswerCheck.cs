using DSAExperimentation.Domain.Mathematics.Numerals;

namespace DSAExperimentation.LeetCode.Solutions.CrackingTheSafe;

// LC 753 accepts any string of minimum length that unlocks the safe at some point, which is one holding
// every password of n digits from 0 to k - 1 as a window. The shortest is k^n + n - 1 long, as the recorded
// one is, so an answer stands when it is that long, uses only those digits, and its k^n windows are all
// different. The greedy recursion and the backtracking engine walk the digits in opposite orders.
internal static class CrackingTheSafeAnswerCheck
{
    public static bool IsAccepted(int passwordLength, int alphabetSize, string answer, string recorded)
    {
        if (answer.Length != recorded.Length)
        {
            return false;
        }

        var windowCount = answer.Length - passwordLength + 1;
        var usesTheAlphabet = answer.All(digit => DecimalDigits.TryIndexOf(digit, out var value) && value < alphabetSize);
        var windows = Enumerable.Range(0, windowCount).Select(start => answer.Substring(start, passwordLength));
        return usesTheAlphabet && windows.Distinct().Count() == windowCount;
    }
}
