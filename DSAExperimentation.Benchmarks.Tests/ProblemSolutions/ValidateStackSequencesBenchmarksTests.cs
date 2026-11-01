using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateStackSequencesBenchmarks (ARCHITECTURE 17.9): the generic check holds
// the arms to agreeing with each other, which a shared wrong verdict would pass, so this pins the
// baseline's verdict to the one the workload's construction fixes.
//
// Setup builds each popped order through the shared StackSequenceWorkloads fixture's
// BuildValidPopOrder, so the pair really is a valid one: the backtracking arm has to search rather
// than fail on its first branch, and LC 946's answer is true. Both arms return bool, so this harness
// witnesses the verdict only, not the interleaving behind it.
public sealed partial class ValidateStackSequencesBenchmarksTests
{
    // The smallest length both arms run.
    private const int SmallestLength = 10;

    // Setup's documented outcome: the popped order was built as a valid one for the pushed array.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void IsValidByBacktrackingSearch_BuiltValidPopOrder_AcceptsTheSequences() =>
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByBacktrackingSearch(SmallestLength));

    private static ValidateStackSequencesBenchmarks BuildHarness()
    {
        var harness = new ValidateStackSequencesBenchmarks();
        harness.Setup();

        return harness;
    }
}
