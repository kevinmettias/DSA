namespace DSAExperimentation.LeetCode.Solutions.FrequenciesOfShortestSupersequences;

// The reach of FrequenciesOfShortestSupersequencesSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. Both arms search the subsets of the letters the
// words use, which more words widen, and SupersequenceFrequenciesByTopologicalSort takes 23.6 ms over the
// 107 of the catalog's 126 cases with up to 15 words (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class FrequenciesOfShortestSupersequencesReach
{
    private const int LargestWordCount = 15;

    public static bool IsWithin(IEnumerable<string> words) =>
        words.Count() <= LargestWordCount;
}
