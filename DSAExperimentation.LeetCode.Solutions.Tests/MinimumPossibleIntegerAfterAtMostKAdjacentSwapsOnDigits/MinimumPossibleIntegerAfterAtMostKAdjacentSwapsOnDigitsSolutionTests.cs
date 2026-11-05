using DSAExperimentation.LeetCode.Solutions.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

// Harness only. Both the physical List<char> simulation and the Fenwick-tree greedy live in
// MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution; this file pins them to
// the two budget extremes - a zero budget, which must
// return the input untouched, and a budget larger than any arrangement needs, which must
// return the sorted digits.
public sealed partial class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolutionTests
{
    public static TheoryData<string, int, string> Cases =>
        new()
        {
            { "4321", 0, "4321" },
            { "22", 22, "22" },
            { "9438957234785635408", 23, "0345989723478563548" },
            { "4321", 1, "3421" },
            { "54321", 1000, "12345" },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinIntegerByListRemoval_Cases_ReturnsSmallestReachableArrangement(
        string num, int maxSwaps, string expected)
    {
        var actual = MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByListRemoval(num, maxSwaps);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinIntegerByFenwickTreeGreedy_Cases_ReturnsSmallestReachableArrangement(
        string num, int maxSwaps, string expected)
    {
        var actual = MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByFenwickTreeGreedy(num, maxSwaps);
        Assert.Equal(expected, actual);
    }
}
