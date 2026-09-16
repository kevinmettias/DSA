using DSAExperimentation.LeetCode.CrackingTheSafe;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CrackingTheSafe;

// Harness only. Both search strategies are CrackingTheSafeSolution's - this file pins
// them to LeetCode's published examples by checking the produced answer has exactly
// the length a de Bruijn sequence demands (k^n + n - 1) and contains every possible
// password of length passwordLength as a substring.
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
    public void CrackSafeByGreedyRecursion_LeetCodeExamples_CoversEveryPassword(
        int passwordLength, int alphabetSize)
    {
        var safe = CrackingTheSafeSolution.CrackSafeByGreedyRecursion(passwordLength, alphabetSize);

        AssertCoversEveryPassword(safe, passwordLength, alphabetSize);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CrackSafeByBacktrackEngine_LeetCodeExamples_CoversEveryPassword(
        int passwordLength, int alphabetSize)
    {
        var safe = CrackingTheSafeSolution.CrackSafeByBacktrackEngine(passwordLength, alphabetSize);

        AssertCoversEveryPassword(safe, passwordLength, alphabetSize);
    }

    private static void AssertCoversEveryPassword(string safe, int passwordLength, int alphabetSize)
    {
        var total = (int)Math.Pow(alphabetSize, passwordLength);
        Assert.Equal(total + passwordLength - 1, safe.Length);

        for (var password = 0; password < total; password++)
        {
            var candidate = ToBaseK(password, passwordLength, alphabetSize);

            Assert.Contains(candidate, safe);
        }
    }

    private static string ToBaseK(int value, int passwordLength, int alphabetSize)
    {
        var digits = new char[passwordLength];
        for (var i = passwordLength - 1; i >= 0; i--)
        {
            digits[i] = (char)('0' + (value % alphabetSize));
            value /= alphabetSize;
        }

        return new string(digits);
    }
}
