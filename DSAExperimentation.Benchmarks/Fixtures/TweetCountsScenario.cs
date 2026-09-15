namespace DSAExperimentation.Benchmarks.Fixtures;

// The measured call of the LC 1348 scenario: the frequency the benchmark asks for
// and the name it asks about. Separate from TweetCountsPerFrequencyBenchmarks
// because these are the query's values, which the benchmark has to name to drive
// the same case, not part of how the tweets are recorded - and because they are
// the benchmark's own statement of LC 1348's spelling, so a benchmark that asked
// the solution what "hour" is would keep measuring after that spelling changed.
internal static class TweetCountsScenario
{
    public const string QueryFrequency = "hour";
    public const string QueryName = "tweet0";
}
