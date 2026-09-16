using DSAExperimentation.LeetCode.CountTheRepetitions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheRepetitions;

// Harness only: both strategies live in CountTheRepetitionsSolution and are asserted
// against the same examples, including the large-n1 case that only cycle detection
// can traverse quickly (the naive arm still finishes it here - it is only LeetCode's
// real n1 <= 10^6 judge that the O(n1 * |s1|) walk cannot meet in time).
public sealed class CountTheRepetitionsTests
{
    public static TheoryData<RepetitionCase> Examples =>
        new()
        {
            { new RepetitionCase(S1: "acb", N1: 4, S2: "ab", N2: 2, Expected: 2) },
            { new RepetitionCase(S1: "acb", N1: 1, S2: "acb", N2: 1, Expected: 1) },
            { new RepetitionCase(S1: "a", N1: 3, S2: "b", N2: 1, Expected: 0) },
            { new RepetitionCase(S1: "acb", N1: 1_000_000, S2: "ab", N2: 100, Expected: 10_000) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxRepetitionsByNaiveSimulation_LeetCodeExamples_ReturnsMaxRepeatCount(
        RepetitionCase example)
    {
        var actual = CountTheRepetitionsSolution.GetMaxRepetitionsByNaiveSimulation(
            example.S1, example.N1, example.S2, example.N2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GetMaxRepetitionsByHashMapCycleDetection_LeetCodeExamples_ReturnsMaxRepeatCount(
        RepetitionCase example)
    {
        var actual = CountTheRepetitionsSolution.GetMaxRepetitionsByHashMapCycleDetection(
            example.S1, example.N1, example.S2, example.N2);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two strings with their repetition counts, and the
    // largest number of s2 repetitions obtainable from s1. The five values are one
    // thing - a repetition query - so they travel as one named case rather than as five
    // positions a caller has to count off; s1/n1 and s2/n2 are otherwise adjacent
    // strings whose order nothing at the call site would catch. Nested because it is
    // only ever used inside this test class: it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct RepetitionCase(string S1, int N1, string S2, int N2, int Expected);
}
