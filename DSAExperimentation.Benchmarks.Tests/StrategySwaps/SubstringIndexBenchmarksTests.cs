using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for SubstringIndexBenchmarks (ARCHITECTURE 17.9). The generic check compares the
// arms on the smallest parameters only, which is one query, so every query's arms are compared
// here, and the baseline's answer to each is pinned by scanning the same text for the same patterns
// - a shared wrong answer would pass the comparison alone.
public sealed partial class SubstringIndexBenchmarksTests
{
    private const int SmallestTextLength = 1_000;

    public static TheoryData<SubstringIndexQuery> Queries =>
        new(Enum.GetValues<SubstringIndexQuery>());

    [Theory]
    [MemberData(nameof(Queries))]
    public void Arms_EveryQuery_AnswerAlike(SubstringIndexQuery query)
    {
        var harness = BuildHarness(query);
        var baseline = harness.SortedSuffixArray();

        Assert.Equal(
            (baseline, baseline),
            (harness.CompressedSuffixTree(), harness.MinimalSuffixAutomaton()));
    }

    [Fact]
    public void SortedSuffixArray_HasSubstring_CountsThePatternsTheTextHolds() =>
        Assert.Equal(
            Patterns().Count(pattern => Text().Contains(pattern, StringComparison.Ordinal)),
            BuildHarness(SubstringIndexQuery.HasSubstring).SortedSuffixArray());

    [Fact]
    public void SortedSuffixArray_CountOccurrences_SumsEveryPatternsOccurrences() =>
        Assert.Equal(
            Patterns().Sum(pattern => (long)OccurrencesOf(Text(), pattern)),
            BuildHarness(SubstringIndexQuery.CountOccurrences).SortedSuffixArray());

    [Fact]
    public void SortedSuffixArray_FindLongestRepeat_IsWhatAPairwiseScanFinds() =>
        Assert.Equal(
            LongestRepeatLength(Text()),
            BuildHarness(SubstringIndexQuery.FindLongestRepeat).SortedSuffixArray());

    [Fact]
    public void SortedSuffixArray_Build_CountsTheEmptyPatternAtEveryInsertionPoint() =>
        Assert.Equal(
            SmallestTextLength + 1, BuildHarness(SubstringIndexQuery.Build).SortedSuffixArray());

    private static SubstringIndexBenchmarks BuildHarness(SubstringIndexQuery query)
    {
        var harness = new SubstringIndexBenchmarks
        {
            Query = query,
            TextLength = SmallestTextLength,
        };
        harness.Setup();

        return harness;
    }

    private static string Text() =>
        DnaSequenceWorkloads.BuildSequence(SmallestTextLength, SubstringIndexBenchmarks.TextSeed);

    private static string[] Patterns() => SubstringIndexBenchmarks.DrawPatterns(Text());

    private static int OccurrencesOf(string text, string pattern) =>
        Enumerable.Range(0, text.Length - pattern.Length + 1)
            .Count(start => text.AsSpan(start, pattern.Length).SequenceEqual(pattern));

    private static int LongestRepeatLength(string text)
    {
        var longest = 0;

        for (var first = 0; first < text.Length; first++)
        {
            for (var second = first + 1; second < text.Length; second++)
            {
                var shared = text.AsSpan(first).CommonPrefixLength(text.AsSpan(second));
                longest = Math.Max(longest, shared);
            }
        }

        return longest;
    }
}
