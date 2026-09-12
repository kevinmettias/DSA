using DSAExperimentation.LeetCode.CountTheRepetitions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheRepetitions;

// Harness only: both strategies live in CountTheRepetitionsSolution and are asserted
// against the same examples, including the large-n1 case that only cycle detection
// can traverse quickly (the naive arm still finishes it here - it is only LeetCode's
// real n1 <= 10^6 judge that the O(n1 * |s1|) walk cannot meet in time).
public sealed class CountTheRepetitionsTests
{
    public static TheoryData<string, int, string, int, int> Examples =>
        new()
        {
            { "acb", 4, "ab", 2, 2 },
            { "acb", 1, "acb", 1, 1 },
            { "a", 3, "b", 1, 0 },
            { "acb", 1_000_000, "ab", 100, 10_000 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxRepetitionsByNaiveSimulation_LeetCodeExamples_ReturnsMaxRepeatCount(
        string s1, int n1, string s2, int n2, int expected) =>
        Assert.Equal(expected, CountTheRepetitionsSolution.GetMaxRepetitionsByNaiveSimulation(s1, n1, s2, n2));

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxRepetitionsByHashMapCycleDetection_LeetCodeExamples_ReturnsMaxRepeatCount(
        string s1, int n1, string s2, int n2, int expected) =>
        Assert.Equal(expected, CountTheRepetitionsSolution.GetMaxRepetitionsByHashMapCycleDetection(s1, n1, s2, n2));
}
