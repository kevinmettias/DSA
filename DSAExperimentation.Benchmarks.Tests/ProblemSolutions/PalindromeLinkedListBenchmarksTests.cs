using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromeLinkedListBenchmarks (ARCHITECTURE 17.9): the baseline's verdict
// against the fixed answer the class comment already names, which the generic check cannot supply -
// it holds the stack reversal to the baseline. Setup builds a genuine palindrome, so the answer is
// true.
public sealed partial class PalindromeLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void FastSlowReversal_GenuinePalindromeList_ReportsPalindrome() =>
        Assert.True(BuildHarness().FastSlowReversal());

    private static PalindromeLinkedListBenchmarks BuildHarness()
    {
        var harness = new PalindromeLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
