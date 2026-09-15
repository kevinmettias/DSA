namespace DSAExperimentation.Benchmarks.Fixtures;

// The size ratio the LC 572 scenario SubtreeOfAnotherTreeWorkloads builds its two
// chains at: the sub-root chain is half the full tree's node count, which is the
// worst case for the per-node comparison arm without making the smaller chain
// trivially small. Separate from SubtreeOfAnotherTreeWorkloads because it is the
// scenario's sizing, which the benchmark has to name to build the same case, not
// part of how a chain is built - build's own filler and last-node values stay
// private to the builder.
internal static class SubtreeOfAnotherTreeScenario
{
    public const int SubRootSizeDivisor = 2;
}
