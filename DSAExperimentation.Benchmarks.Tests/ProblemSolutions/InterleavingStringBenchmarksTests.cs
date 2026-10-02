using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InterleavingStringBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized recursion and the roll-forward row - so a harness
// whose arms disagree is answering two different questions: both must confirm the same interleaving.
// There is no [Params] property and no [GlobalSetup] to call: the arms' inputs are the constants
// LeetCode 97's own example names, so the harness is constructed bare. That example is decisive -
// "aadbbcbcac" is a valid interleaving of "aabcc" and "dbbca" - and the second assertion of each arm
// guards against it being vacuously true by running the same strategy over the problem's other
// documented pair, whose target is not an interleaving of those two sources.
public sealed partial class InterleavingStringBenchmarksTests
{
    // The arms' own two sources, restated from the benchmarks' constants, plus LeetCode 97's second
    // example - the same sources against a target that cannot be formed.
    private const string FirstSource = "aabcc";
    private const string SecondSource = "dbbca";
    private const string NonInterleavingTarget = "aadbbbaccc";

    [Fact]
    public void IsInterleaveByMemoizedRecursion_LeetCodeExample_ConfirmsTheKnownInterleaving()
    {
        Assert.True(BuildHarness().IsInterleaveByMemoizedRecursion());
        Assert.False(InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            FirstSource,
            SecondSource,
            new InterleavingStringSolution.TargetText(NonInterleavingTarget)));
    }

    [Fact]
    public void IterativeTable_LeetCodeExample_ConfirmsTheKnownInterleaving()
    {
        Assert.True(BuildHarness().IterativeTable());
        Assert.False(InterleavingStringSolution.IsInterleaveByIterativeTable(
            FirstSource,
            SecondSource,
            new InterleavingStringSolution.TargetText(NonInterleavingTarget)));
    }

    [Fact]
    public void IterativeTable_AgreesWithIsInterleaveByMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsInterleaveByMemoizedRecursion(), harness.IterativeTable());
    }

    private static InterleavingStringBenchmarks BuildHarness() => new();
}
