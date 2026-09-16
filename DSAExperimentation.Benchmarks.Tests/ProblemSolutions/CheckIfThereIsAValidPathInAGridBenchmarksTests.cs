using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfThereIsAValidPathInAGridBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a hand-rolled recursive DFS over a visited array
// against the repo's own DepthFirstSearch.Traverse over a street-opening successor function - so a
// harness whose arms disagree is timing two different problems. Both walk the identical
// street-compatibility rule, so agreement between them says the two traversals reached the same
// verdict on the same grid.
public sealed partial class CheckIfThereIsAValidPathInAGridBenchmarksTests
{
    private const int SmallestSize = 10;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameSeededGrid()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The seeded street grid is private and each arm reduces it to one bit, so two harnesses
        // built from the same Size reporting the same verdict for each traversal is the reading the
        // rebuild can be pinned to: one seed means the same street types in the same cells, and the
        // same grid means the same verdict.
        Assert.Equal(first.HasValidPathByRecursiveDfs(), second.HasValidPathByRecursiveDfs());
        Assert.Equal(first.HasValidPathByDepthFirstTraverse(), second.HasValidPathByDepthFirstTraverse());
    }

    [Fact]
    public void HasValidPathByRecursiveDfs_SeededStreetGrid_AgreesWithDepthFirstTraverse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasValidPathByDepthFirstTraverse(), harness.HasValidPathByRecursiveDfs());
    }

    [Fact]
    public void HasValidPathByDepthFirstTraverse_SeededStreetGrid_AgreesWithRecursiveDfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasValidPathByRecursiveDfs(), harness.HasValidPathByDepthFirstTraverse());
    }

    private static CheckIfThereIsAValidPathInAGridBenchmarks BuildHarness()
    {
        var harness = new CheckIfThereIsAValidPathInAGridBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
