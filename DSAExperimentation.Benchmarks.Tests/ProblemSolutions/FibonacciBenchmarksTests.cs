using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FibonacciBenchmarks (ARCHITECTURE 17.9): its three arms are competing
// strategies for the same recurrence - the naive double recursion, this repo's Memoizer, and the
// O(1)-space iterative loop - so a harness whose arms disagree is computing two different sequences.
// All three return a plain int. The class has no [GlobalSetup]: the [Params] term index is the whole
// workload, so each arm is called on its own harness and the smallest index is used. The flagged
// Replay member is the recurrence the memoized arm's Memoizer drives, and the memoized arm's result
// is the only observable of it from outside the class, so it is asserted through that arm.
public sealed partial class FibonacciBenchmarksTests
{
    private const int SmallestTermIndex = 20;

    // LC 509's own published value for the twentieth term, which is the term index this harness
    // measures.
    private const int ExpectedTerm = 6765;

    [Fact]
    public void NaiveRecursive_TwentiethTerm_AgreesWithIterativeConstantSpace()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTerm, harness.NaiveRecursive());

        Assert.Equal(harness.IterativeConstantSpace(), harness.NaiveRecursive());
    }

    [Fact]
    public void TopDownMemoized_TwentiethTerm_AgreesWithNaiveRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTerm, harness.TopDownMemoized());

        Assert.Equal(harness.NaiveRecursive(), harness.TopDownMemoized());
    }

    [Fact]
    public void IterativeConstantSpace_TwentiethTerm_AgreesWithNaiveRecursive()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTerm, harness.IterativeConstantSpace());

        Assert.Equal(harness.NaiveRecursive(), harness.IterativeConstantSpace());
    }

    [Fact]
    public void Replay_MemoizedRecurrenceDrivenByMemoizer_AgreesWithIterativeConstantSpace()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IterativeConstantSpace(), harness.TopDownMemoized());
    }

    private static FibonacciBenchmarks BuildHarness() =>
        new() { TermIndex = SmallestTermIndex };
}
