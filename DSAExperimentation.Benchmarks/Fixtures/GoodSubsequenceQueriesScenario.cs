namespace DSAExperimentation.Benchmarks.Fixtures;

// The p the LC 3901 scenario GoodSubsequenceQueriesWorkloads builds its array for:
// LC's own statement is "a subsequence whose gcd is exactly p", and both of the
// solution's strategies take that value as their own p argument, so the benchmark
// that drives them has to name it to drive the same case. Separate from
// GoodSubsequenceQueriesWorkloads because it is the scenario's input value rather
// than part of how the array is generated - the builder's own value range is private
// to it - and owned here so the builder and the benchmark read one definition.
internal static class GoodSubsequenceQueriesScenario
{
    public const int P = 3;
}
