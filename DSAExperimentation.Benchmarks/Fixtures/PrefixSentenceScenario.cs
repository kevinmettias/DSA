namespace DSAExperimentation.Benchmarks.Fixtures;

// The search word the LC 1455 scenario PrefixSentenceWorkloads generates sentences
// around: longer than any generated word and outside the generated alphabet's runs,
// so no word ever starts with it and every strategy is forced through the whole
// sentence instead of stopping at an early match. Separate from
// PrefixSentenceWorkloads because it is the scenario's input value, which the
// benchmark has to name to drive the same case, not part of how the sentence is
// generated - the separator, alphabet and word-length range stay private to the
// builder.
internal static class PrefixSentenceScenario
{
    public const string UnmatchedSearchWord = "zzzunmatched";
}
