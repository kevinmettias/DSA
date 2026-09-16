using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheDuplicateNumberBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - the all-pairs scan against reading nums[i] as a pointer
// from node i and handing the implicit list to the repo's Floyd cycle detection - so a harness
// whose arms disagree is timing two different problems. Setup builds values 1..Length with Length
// appended again, so the one duplicate sits at the end of every scan and the same Length must
// rebuild the same array.
public sealed partial class FindTheDuplicateNumberBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NestedLoopBruteForce(), BuildHarness().NestedLoopBruteForce());

    [Fact]
    public void NestedLoopBruteForce_SmallestLength_AgreesWithLinkedListCycleDetection()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinkedListCycleDetection(), harness.NestedLoopBruteForce());
    }

    [Fact]
    public void LinkedListCycleDetection_SmallestLength_AgreesWithNestedLoopBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NestedLoopBruteForce(), harness.LinkedListCycleDetection());
    }

    private static FindTheDuplicateNumberBenchmarks BuildHarness()
    {
        var harness = new FindTheDuplicateNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
