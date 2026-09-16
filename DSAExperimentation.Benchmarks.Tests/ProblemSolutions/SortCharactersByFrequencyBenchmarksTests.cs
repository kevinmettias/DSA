using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortCharactersByFrequencyBenchmarks (ARCHITECTURE 17.9): both arms are
// SortCharactersByFrequencySolution's arrangements of the same generated text. Setup is a pure
// function of Length and its own fixed seed, so the same Length must rebuild the same text.
//
// WEAK BY CONSTRUCTION, and reported as such: LeetCode 451 fixes only that characters are
// grouped by non-increasing frequency and leaves the order of two characters with EQUAL
// frequency free, which the generated 26-letter alphabet makes common - the two arms measurably
// diverge on this workload at the first equal-frequency pair ('o' and 's', both 21 occurrences).
// A whole-string comparison would therefore pin an order the problem never promised, so each
// arrangement is compared through the multiset of its run lengths, which is exactly the freedom
// the problem leaves. That still catches an arm that drops, duplicates or misgroups a character;
// it cannot catch an arm that breaks a frequency tie the other way, because neither is wrong.
public sealed partial class SortCharactersByFrequencyBenchmarksTests
{
    private const int SmallestLength = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            CanonicalArrangement(BuildHarness().DictionaryThenOrderByDescending()),
            CanonicalArrangement(BuildHarness().DictionaryThenOrderByDescending()));

    [Fact]
    public void DictionaryThenOrderByDescending_ThousandCharacters_AgreesWithHashMapThenHeapDescendingPop()
    {
        var harness = BuildHarness();

        AssertRunsDescendInLength(harness.DictionaryThenOrderByDescending());
        Assert.Equal(
            CanonicalArrangement(harness.HashMapThenHeapDescendingPop()),
            CanonicalArrangement(harness.DictionaryThenOrderByDescending()));
    }

    [Fact]
    public void HashMapThenHeapDescendingPop_ThousandCharacters_AgreesWithDictionaryThenOrderByDescending()
    {
        var harness = BuildHarness();

        AssertRunsDescendInLength(harness.HashMapThenHeapDescendingPop());
        Assert.Equal(
            CanonicalArrangement(harness.DictionaryThenOrderByDescending()),
            CanonicalArrangement(harness.HashMapThenHeapDescendingPop()));
    }

    private static SortCharactersByFrequencyBenchmarks BuildHarness()
    {
        var harness = new SortCharactersByFrequencyBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // Each character's run collapsed to its length, the lengths then sorted: the arrangement
    // with the order of equally frequent characters - the part the problem leaves free - sorted
    // away, so two arrangements agree here exactly when they agree on every frequency.
    private static List<int> CanonicalArrangement(string arranged)
    {
        var runLengths = RunsOfTheArrangement(arranged);
        runLengths.Sort();

        return runLengths;
    }

    // Each run of one repeated character is no longer than the run before it - the grouping the
    // problem does fix, read off the arrangement alone and in the order the runs appear.
    private static void AssertRunsDescendInLength(string arranged)
    {
        var previousRunLength = int.MaxValue;

        foreach (var runLength in RunsOfTheArrangement(arranged))
        {
            Assert.True(
                runLength <= previousRunLength,
                $"A run of length {runLength} follows a shorter run of length {previousRunLength}.");

            previousRunLength = runLength;
        }
    }

    // The length of each run of one repeated character, in the order the runs appear.
    private static List<int> RunsOfTheArrangement(string arranged)
    {
        var runLengths = new List<int>();
        var index = 0;

        while (index < arranged.Length)
        {
            var character = arranged[index];
            var runLength = 0;

            while (index < arranged.Length && arranged[index] == character)
            {
                runLength++;
                index++;
            }

            runLengths.Add(runLength);
        }

        return runLengths;
    }
}
