using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.FlattenAMultilevelDoublyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FlattenAMultilevelDoublyLinkedListBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: the flattened order, known from the fixture's own structure rather
// than from either arm. The class has no [GlobalSetup] - each arm rebuilds its own list inside the
// call, because flattening is destructive - so the harness is the bare initializer plus Length.
//
// Each arm returns the flattened list's head as object? (the node type is internal, CS0050). The arm
// builds a Length-node chain 1, 2, ... and hangs one single-node child valued Length plus its parent's
// value off each node but the last, so a correct flatten splices each child straight after its parent:
// Length + (Length - 1) nodes alternating chain value and child value, which catches a dropped,
// duplicated or misplaced splice.
public sealed partial class FlattenAMultilevelDoublyLinkedListBenchmarksTests
{
    private const int SmallestLength = 200;


    [Fact]
    public void BruteForceRescanFromHead_ChildOnEveryNodeButTheLast_SplicesEachChildAfterItsParent() =>
        Assert.Equal(FlattenedValues(), ValuesOf(BuildHarness().BruteForceRescanFromHead()));

    [Fact]
    public void StackBasedOnePass_ChildOnEveryNodeButTheLast_SplicesEachChildAfterItsParent() =>
        Assert.Equal(FlattenedValues(), ValuesOf(BuildHarness().StackBasedOnePass()));

    private static IEnumerable<int> FlattenedValues() =>
        Enumerable.Range(1, SmallestLength - 1)
            .SelectMany(value => new[] { value, SmallestLength + value })
            .Append(SmallestLength);

    private static List<int> ValuesOf(object? answer)
    {
        var values = new List<int>();

        for (var node = Assert.IsType<Node>(answer); node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return values;
    }

    private static FlattenAMultilevelDoublyLinkedListBenchmarks BuildHarness() =>
        new() { Length = SmallestLength };
}
