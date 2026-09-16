using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InterleavingStringBenchmarks (ARCHITECTURE 17.9): the class has a single arm
// - the memoized recursion - so there is no second strategy to reconcile it against and the
// assertion has to come from the arm's own declared contract instead. There is no [Params] property
// and no [GlobalSetup] to call: the arm's inputs are the constants LeetCode 97's own example names,
// so the harness is constructed bare. That example is decisive - "aadbbcbcac" is a valid
// interleaving of "aabcc" and "dbbca" - and the second assertion guards against the arm being
// vacuously true by running the same recursion the arm runs over the problem's other documented
// pair, whose target is not an interleaving of those two sources.
public sealed partial class InterleavingStringBenchmarksTests
{
    // The arm's own two sources, restated from its constants, plus LeetCode 97's second example -
    // the same sources against a target that cannot be formed.
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

    private static InterleavingStringBenchmarks BuildHarness() => new();
}
