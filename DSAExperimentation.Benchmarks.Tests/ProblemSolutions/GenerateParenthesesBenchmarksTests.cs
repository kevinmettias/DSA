using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for GenerateParenthesesBenchmarks (ARCHITECTURE 17.9): both arms are
// GenerateParenthesesSolution's - the generic Backtrack engine driving ParenthesesState against the
// specialized recursion - so a harness whose arms disagree is timing two different problems. The
// class carries no [GlobalSetup] and no tuned fixture: Pairs is the whole workload, so there is
// nothing to rebuild and no setup invariant to assert. Both arms build LeetCode 22's real answer, a
// List<string> of every well-formed combination, and both take '(' before ')' at every node of the
// same decision tree, so the outer order is pinned as well as the contents and AnswerText.Of can
// compare them position by position. The count is decisive independently of either arm: the
// combinations of Pairs pairs are counted by the Pairs-th Catalan number, which is 42 at five.
public sealed partial class GenerateParenthesesBenchmarksTests
{
    private const int SmallestPairs = 5;

    // The fifth Catalan number: the number of well-formed combinations of five pairs.
    private const int ExpectedCombinationCount = 42;

    [Fact]
    public void RecursiveSpecialized_FivePairs_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCombinationCount, harness.RecursiveSpecialized().Count);
        Assert.Equal(AnswerText.Of(harness.Backtracking()), AnswerText.Of(harness.RecursiveSpecialized()));
    }

    [Fact]
    public void Backtracking_FivePairs_AgreesWithRecursiveSpecialized()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedCombinationCount, harness.Backtracking().Count);
        Assert.Equal(AnswerText.Of(harness.RecursiveSpecialized()), AnswerText.Of(harness.Backtracking()));
    }

    private static GenerateParenthesesBenchmarks BuildHarness() => new() { Pairs = SmallestPairs };
}
