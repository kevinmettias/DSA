namespace DSAExperimentation.Benchmarks.Fixtures;

// The text size the LC 792 scenario MatchingSubsequenceWorkloads generates: long
// enough that the per-word two-pointer arm's O(words * s.Length) worst case is
// measurable rather than dwarfed by setup. Separate from
// MatchingSubsequenceWorkloads because it is the scenario's size, which the
// benchmark has to name to build the same case, not part of how the text is
// generated - those values (the 25-letter alphabet, the word length, the excluded
// 26th letter that makes every word unmatchable) stay private to the builder.
internal static class MatchingSubsequenceScenario
{
    public const int TextLength = 20_000;
}
