using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InterleavingStringBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized recursion and the roll-forward row - so a harness
// whose arms disagree is answering two different questions: both must confirm the same interleaving.
// Setup's target comes from InterleavingStringWorkloads, which builds it by interleaving the two
// sources - a construction its own tests follow letter by letter - so the decisive verdict is true.
// The second assertion of each arm guards against that being vacuously true by running the same
// strategy over LeetCode 97's documented false pair, whose target is not an interleaving of its two
// sources.
public sealed partial class InterleavingStringBenchmarksTests
{
    private const int SmallestSourceLength = 10;

    // LeetCode 97's second example: two sources and a target that cannot be formed from them.
    private const string FirstSource = "aabcc";
    private const string SecondSource = "dbbca";
    private const string NonInterleavingTarget = "aadbbbaccc";

    [Fact]
    public void IsInterleaveByMemoizedRecursion_SeededInterleaving_ConfirmsIt()
    {
        Assert.True(BuildHarness().IsInterleaveByMemoizedRecursion());
        Assert.False(InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            FirstSource,
            SecondSource,
            new InterleavingStringSolution.TargetText(NonInterleavingTarget)));
    }

    [Fact]
    public void IterativeTable_SeededInterleaving_ConfirmsIt()
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

    private static InterleavingStringBenchmarks BuildHarness()
    {
        var harness = new InterleavingStringBenchmarks { SourceLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
