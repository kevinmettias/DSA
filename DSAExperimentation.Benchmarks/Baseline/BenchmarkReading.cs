namespace DSAExperimentation.Benchmarks.Baseline;

// One arm of one benchmark class at one parameter value: the coarsest thing a run says
// that is worth remembering between runs.
//
// Allocation is nullable because a report can omit the memory diagnoser, and "not
// measured" is not the same claim as "allocated nothing". A baseline that read the two
// alike would report an improvement every time a diagnoser went missing, which is
// precisely the run least able to show one.
internal readonly record struct BenchmarkReading(
    string Name,
    double MeanNanoseconds,
    long? AllocatedBytes);
