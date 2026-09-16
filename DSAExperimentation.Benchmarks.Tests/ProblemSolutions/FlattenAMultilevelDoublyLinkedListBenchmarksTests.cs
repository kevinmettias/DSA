using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlattenAMultilevelDoublyLinkedListBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question, so a harness whose arms disagree is timing two
// different problems. The class has no [GlobalSetup] - each arm rebuilds its own list inside the
// call, because flattening is destructive - so the harness is the bare initializer plus Length.
//
// The agreement is weak by construction and the assertions say so: both arms return only the *count*
// of nodes in the flattened list, never the flattened list itself, so the count is a proxy for the
// answer rather than the answer. The count is still pinned to the fixture's own structure - the arm
// builds a Length-node chain and hangs one single-node child off each node but the last, so a correct
// flatten leaves exactly Length + (Length - 1) nodes - which catches a dropped or duplicated splice
// but not two arms that both splice the same wrong nodes.
public sealed partial class FlattenAMultilevelDoublyLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;

    // Each of the first Length - 1 nodes of the arm's own chain carries one child node.
    private const int ChildCount = SmallestLength - 1;

    [Fact]
    public void BruteForceRescanFromHead_AgreesWithStackBasedOnePass()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFlattenedNodeCount, harness.BruteForceRescanFromHead());
        Assert.Equal(harness.StackBasedOnePass(), harness.BruteForceRescanFromHead());
    }

    [Fact]
    public void StackBasedOnePass_AgreesWithBruteForceRescanFromHead()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFlattenedNodeCount, harness.StackBasedOnePass());
        Assert.Equal(harness.BruteForceRescanFromHead(), harness.StackBasedOnePass());
    }

    private static int ExpectedFlattenedNodeCount => SmallestLength + ChildCount;

    private static FlattenAMultilevelDoublyLinkedListBenchmarks BuildHarness() =>
        new() { Length = SmallestLength };
}
