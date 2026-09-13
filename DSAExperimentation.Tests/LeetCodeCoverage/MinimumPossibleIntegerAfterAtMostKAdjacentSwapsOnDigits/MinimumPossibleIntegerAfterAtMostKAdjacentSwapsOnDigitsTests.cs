using DSAExperimentation.LeetCode.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

// Harness only. Both the physical List<char> simulation and the Fenwick-tree greedy live in
// MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution; this file pins them to
// LeetCode's published examples plus the two budget extremes - a zero budget, which must
// return the input untouched, and a budget larger than any arrangement needs, which must
// return the sorted digits.
public sealed class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsTests
{
    public static TheoryData<string, int, string> Examples =>
        new()
        {
            { "4321", 4, "1342" },
            { "100", 1, "010" },
            { "36789", 1000, "36789" },
            { "4321", 0, "4321" },
            { "22", 22, "22" },
            { "9438957234785635408", 23, "0345989723478563548" },
            { "4321", 1, "3421" },
            { "54321", 1000, "12345" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIntegerByListRemoval_LeetCodeExamples_ReturnsSmallestReachableArrangement(
        string num, int k, string expected) =>
        Assert.Equal(
            expected,
            MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByListRemoval(num, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIntegerByFenwickTreeGreedy_LeetCodeExamples_ReturnsSmallestReachableArrangement(
        string num, int k, string expected) =>
        Assert.Equal(
            expected,
            MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByFenwickTreeGreedy(num, k));
}
