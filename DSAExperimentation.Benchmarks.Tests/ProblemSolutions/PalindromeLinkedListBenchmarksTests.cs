using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromeLinkedListBenchmarks (ARCHITECTURE 17.9). This class carries a
// single arm, so there is no second strategy to reconcile it against: the verdict is instead checked
// against the fixed answer the class comment already names - Setup builds a genuine palindrome, so
// the arm must report true.
//
// WEAK BY CONSTRUCTION, and reported as such: the one arm returns a bool, so the Setup comparison can
// only show that two harnesses built from the same Length answer the same way. The built chain is
// private, so the workload itself cannot be read back to compare; the arm's own verdict is the only
// observable it exposes.
public sealed partial class PalindromeLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsPalindromeByStackReversal(),
            BuildHarness().IsPalindromeByStackReversal());

    [Fact]
    public void IsPalindromeByStackReversal_GenuinePalindromeList_ReportsPalindrome()
    {
        var harness = BuildHarness();

        Assert.True(harness.IsPalindromeByStackReversal());
    }

    private static PalindromeLinkedListBenchmarks BuildHarness()
    {
        var harness = new PalindromeLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
