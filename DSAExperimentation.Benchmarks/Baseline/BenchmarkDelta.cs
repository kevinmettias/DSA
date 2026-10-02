namespace DSAExperimentation.Benchmarks.Baseline;

// One benchmark's move between a baseline and a fresh run.
//
// Both numbers travel with the ratio rather than the ratio alone, because a ratio hides
// its own scale: "2.8x" reads the same whether the arm took 4 ns or 4 ms, and only one of
// those is worth interrupting a day for.
//
// Allocation is carried the same way for a less obvious reason. It is exact per RUN and
// not per operation: BenchmarkDotNet reports total bytes divided by the number of
// operations it chose, and that divisor differs between runs, so the figure moves by a
// hundred bytes or so on a large arm without anything having changed. Measured on this
// repository, an unmodified arm reported 17,537,358 bytes and then 17,537,486. Treating
// any increase as a regression made the tool report that as a regression while the arm's
// own time had improved, so allocation is held to the same relative tolerance as time.
internal sealed record BenchmarkDelta(
    string Name,
    double BaselineNanoseconds,
    double CurrentNanoseconds,
    long? BaselineBytes,
    long? CurrentBytes)
{
    public double Ratio => CurrentNanoseconds / BaselineNanoseconds;

    public bool AllocationUnknown => BaselineBytes is null || CurrentBytes is null;

    public long? AllocationChange =>
        BaselineBytes is { } before && CurrentBytes is { } after ? after - before : null;

    // Relative to the baseline, with no absolute floor: a run that allocated nothing per
    // operation and now allocates something has changed, and a floor would be exactly the
    // thing that hid it. The cost of the choice is that a small arm can be reported on a few
    // bytes of movement, which is noisy and loud - the other way round from being silent.
    public bool AllocationRegressed(double tolerance) =>
        BaselineBytes is { } before && CurrentBytes is { } after && after - before > tolerance * before;

    public bool AllocationImproved(double tolerance) =>
        BaselineBytes is { } before && CurrentBytes is { } after && before - after > tolerance * before;
}
