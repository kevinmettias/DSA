using DSAExperimentation.LeetCode.CrackingTheSafe;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CrackingTheSafe;

// Harness only. Both search strategies are CrackingTheSafeSolution's - this file pins
// them to LeetCode's published examples by checking the produced answer has exactly
// the length a de Bruijn sequence over k digits of length n demands (k^n + n - 1) and
// contains every possible password of length n as a substring.
public sealed class CrackingTheSafeTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 1, 2 },
            { 2, 2 },
            { 2, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CrackSafeByGreedyRecursion_LeetCodeExamples_CoversEveryPassword(int n, int k)
    {
        var safe = CrackingTheSafeSolution.CrackSafeByGreedyRecursion(n, k);

        AssertCoversEveryPassword(safe, n, k);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CrackSafeByBacktrackEngine_LeetCodeExamples_CoversEveryPassword(int n, int k)
    {
        var safe = CrackingTheSafeSolution.CrackSafeByBacktrackEngine(n, k);

        AssertCoversEveryPassword(safe, n, k);
    }

    private static void AssertCoversEveryPassword(string safe, int n, int k)
    {
        var total = (int)Math.Pow(k, n);
        Assert.Equal(total + n - 1, safe.Length);

        for (var password = 0; password < total; password++)
        {
            var candidate = ToBaseK(password, n, k);

            Assert.Contains(candidate, safe);
        }
    }

    private static string ToBaseK(int value, int n, int k)
    {
        var digits = new char[n];
        for (var i = n - 1; i >= 0; i--)
        {
            digits[i] = (char)('0' + (value % k));
            value /= k;
        }

        return new string(digits);
    }
}
