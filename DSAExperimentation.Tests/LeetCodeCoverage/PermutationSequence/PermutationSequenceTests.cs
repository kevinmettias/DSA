namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationSequence;

public sealed class PermutationSequenceTests
{
    [Theory]
    [InlineData(3, 3, "213")]
    [InlineData(4, 9, "2314")]
    [InlineData(3, 1, "123")]
    public void GetPermutation_FactoradicSelection_ReturnsKthPermutation(int n, int k, string expected) => Assert.Equal(expected, GetPermutation(n, k));

    private static string GetPermutation(int n, int k)
    {
        var digits = Enumerable.Range(1, n).ToList(); var factorial = new int[n + 1]; factorial[0] = 1;
        for (var i = 1; i <= n; i++) factorial[i] = factorial[i - 1] * i;
        k--; var chars = new List<char>();
        for (var remaining = n; remaining >= 1; remaining--)
        {
            var index = k / factorial[remaining - 1]; k %= factorial[remaining - 1];
            chars.Add((char)('0' + digits[index])); digits.RemoveAt(index);
        }
        return new string(chars.ToArray());
    }
}
