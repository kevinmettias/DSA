namespace DSAExperimentation.Benchmarks.Fixtures;

// The defining inputs of the LC 2515 scenario CircularArrayWorkloads materializes: a
// circle of filler words with exactly one word - the target - sitting at the index
// diametrically opposite StartIndex, the worst case for either search direction.
// Separate from CircularArrayWorkloads because these are the scenario's values, which
// a benchmark has to name to drive the same case, not part of how the word array is
// built. Owned here so both the builder and the benchmark read one definition.
internal static class CircularArrayScenario
{
    public const string Target = "target-word";
    public const int StartIndex = 0;
}
