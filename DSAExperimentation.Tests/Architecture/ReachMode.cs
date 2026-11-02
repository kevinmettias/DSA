namespace DSAExperimentation.Tests.Architecture;

// What a walk is reading: the harness, which hands the baseline its input; a value a benchmark field
// is assigned, followed only into solution code and the types it creates; or the baseline itself.
internal enum ReachMode
{
    Harness,
    Slice,
    Baseline,
}
