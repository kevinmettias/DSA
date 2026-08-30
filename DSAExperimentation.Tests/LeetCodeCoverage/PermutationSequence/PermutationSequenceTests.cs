using System.Text;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PermutationSequence;

// LeetCode 60. Permutation Sequence: factorial-number-system (factoradic)
// selection over this repo's own DynamicArray<int> as the shrinking pool of
// unused digits - Get(index) reads the next digit, RemoveAt(index) shrinks the
// pool, both DynamicArray operations the classic algorithm already needs.
public sealed class PermutationSequenceTests
{
    [Theory]
    [InlineData(3, 3, "213")]
    [InlineData(4, 9, "2314")]
    [InlineData(3, 1, "123")]
    public void GetPermutation_FactoradicSelection_ReturnsKthPermutation(int n, int k, string expected)
        => Assert.Equal(expected, GetPermutation(n, k));

    private static string GetPermutation(int n, int k)
    {
        var digits = new DynamicArray<int>();
        for (var d = 1; d <= n; d++)
        {
            digits.Add(d);
        }

        var factorial = new int[n + 1];
        factorial[0] = 1;
        for (var i = 1; i <= n; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        k--;
        var result = new StringBuilder();
        for (var remaining = n; remaining >= 1; remaining--)
        {
            var index = k / factorial[remaining - 1];
            k %= factorial[remaining - 1];
            result.Append(digits.Get(index));
            digits.RemoveAt(index);
        }

        return result.ToString();
    }
}
