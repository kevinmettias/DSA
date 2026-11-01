using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InterleavingStringBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the smallest workload, so the verdict that workload fixes is pinned
// on the baseline only. Setup's target comes from InterleavingStringWorkloads, which builds it by
// interleaving the two sources - a construction its own tests follow letter by letter - so the
// decisive verdict is true. Each arm is also run over LeetCode 97's documented false pair, whose
// target is not an interleaving of its two sources, an input the generic check never sees, which
// guards against the true verdict being vacuous.
public sealed partial class InterleavingStringBenchmarksTests
{
    private const int SmallestSourceLength = 10;

    // LeetCode 97's second example: two sources and a target that cannot be formed from them.
    private const string FirstSource = "aabcc";
    private const string SecondSource = "dbbca";
    private const string NonInterleavingTarget = "aadbbbaccc";

    [Fact]
    public void IsInterleaveByMemoizedRecursion_LeetCodeFalseExample_RejectsIt() =>
        Assert.False(InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            FirstSource,
            SecondSource,
            new InterleavingStringSolution.TargetText(NonInterleavingTarget)));

    [Fact]
    public void IterativeTable_SeededInterleaving_ConfirmsIt()
    {
        Assert.True(BuildHarness().IterativeTable());
        Assert.False(InterleavingStringSolution.IsInterleaveByIterativeTable(
            FirstSource,
            SecondSource,
            new InterleavingStringSolution.TargetText(NonInterleavingTarget)));
    }

    private static InterleavingStringBenchmarks BuildHarness()
    {
        var harness = new InterleavingStringBenchmarks { SourceLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
