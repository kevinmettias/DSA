using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidateStackSequencesBenchmarks (ARCHITECTURE 17.9): its two arms are
// ValidateStackSequencesSolution's competing strategies for the same question - exhaustive
// push/pop-timing backtracking against the single greedy stack sweep - so a harness whose arms
// disagree is answering two different questions about LC 946.
//
// Setup builds each popped order through the shared StackSequenceWorkloads fixture's
// BuildValidPopOrder, so the pair really is a valid one: the backtracking arm has to search rather
// than fail on its first branch, and LC 946's answer is true. That decisive literal is asserted
// alongside the arms' agreement so a shared wrong verdict cannot pass. Both arms return bool, so this
// harness witnesses the verdict only, not the interleaving behind it.
public sealed partial class ValidateStackSequencesBenchmarksTests
{
    // The smallest length both arms run.
    private const int SmallestLength = 10;

    // Setup's documented outcome: the popped order was built as a valid one for the pushed array.
    private const bool ExpectedIsValid = true;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(
            BuildHarness().IsValidByGreedyStackSweep(SmallestLength),
            BuildHarness().IsValidByGreedyStackSweep(SmallestLength));
        Assert.Equal(ExpectedIsValid, BuildHarness().IsValidByGreedyStackSweep(SmallestLength));
    }

    [Fact]
    public void IsValidByBacktrackingSearch_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.IsValidByGreedyStackSweep(SmallestLength),
            harness.IsValidByBacktrackingSearch(SmallestLength));
        Assert.Equal(ExpectedIsValid, harness.IsValidByBacktrackingSearch(SmallestLength));
    }

    [Fact]
    public void IsValidByGreedyStackSweep_SmallestLength_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.IsValidByBacktrackingSearch(SmallestLength),
            harness.IsValidByGreedyStackSweep(SmallestLength));
        Assert.Equal(ExpectedIsValid, harness.IsValidByGreedyStackSweep(SmallestLength));
    }

    private static ValidateStackSequencesBenchmarks BuildHarness()
    {
        var harness = new ValidateStackSequencesBenchmarks();
        harness.Setup();

        return harness;
    }
}
