using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromeLinkedListBenchmarks (ARCHITECTURE 17.9). This class carries two
// arms - the stack reversal and the fast/slow in-place reversal - so the pair must agree as well as
// each matching the fixed answer the class comment already names: Setup builds a genuine palindrome,
// so both arms must report true.
//
// WEAK BY CONSTRUCTION, and reported as such: both arms return a bool, so the Setup comparison can
// only show that two harnesses built from the same Length answer the same way. The built chain is
// private, so the workload itself cannot be read back to compare; each arm's own verdict is the only
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

    [Fact]
    public void FastSlowReversal_GenuinePalindromeList_ReportsPalindrome()
    {
        var harness = BuildHarness();

        Assert.True(harness.FastSlowReversal());
    }

    [Fact]
    public void FastSlowReversal_AgreesWithIsPalindromeByStackReversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsPalindromeByStackReversal(), harness.FastSlowReversal());
    }

    private static PalindromeLinkedListBenchmarks BuildHarness()
    {
        var harness = new PalindromeLinkedListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
