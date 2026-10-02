namespace DSAExperimentation.Benchmarks.Baseline;

// Everything a report says about a run that is not one arm's numbers: the job that
// produced them, the machine they were produced on, and the arms themselves.
//
// The first two are carried beside the numbers rather than assumed, because neither is
// recoverable from the numbers and both change what they mean. A mean of 4 ns measured
// by a different job is a different measurement, not a faster or slower version of this
// one, and the only moment that is cheap to notice is before the comparison is read.
internal sealed record BenchmarkRun(
    string Job,
    string Host,
    IReadOnlyList<BenchmarkReading> Readings);
