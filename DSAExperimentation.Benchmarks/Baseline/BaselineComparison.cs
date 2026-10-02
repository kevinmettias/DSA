namespace DSAExperimentation.Benchmarks.Baseline;

// The verdict of one comparison, with the two ways a comparison can be worthless kept
// apart from the one way the code can have got slower.
//
// A baseline whose job no longer matches, or whose arms no longer all appear, is not a
// baseline for this run: the numbers either side of it are not comparable, and reporting
// that as "no regressions" would be the most misleading answer available. Missing and
// added arms are separated from each other for the same reason - a deleted arm and a
// newly added one call for different responses, and one `Count` covering both would hide
// which happened.
//
// `WithinTolerance` is counted here rather than left to the caller to derive, because a
// comparison that accounts for every arm is the only kind whose silence means anything.
internal sealed record BaselineComparison(
    string BaselineJob,
    string CurrentJob,
    IReadOnlyList<BenchmarkDelta> Regressions,
    IReadOnlyList<BenchmarkDelta> Improvements,
    IReadOnlyList<string> Missing,
    IReadOnlyList<string> Added,
    int WithinTolerance)
{
    public bool JobsMatch => string.Equals(BaselineJob, CurrentJob, StringComparison.Ordinal);

    public bool DescribesTheRun => Missing.Count == 0;

    public bool IsUsable => JobsMatch && DescribesTheRun;

    public bool Regressed => Regressions.Count > 0;
}
