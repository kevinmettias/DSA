using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseNodesInKGroupBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the workload's shape.
// [GlobalSetup] builds the values 1..Length in order - they wrap only past LC 25's highest value,
// 1,000, which the smallest length never reaches. The group size is fixed at four and Length is a
// multiple of it, so every group is complete and no truncated tail is left over - the answer is the
// same ascending range with each run of four rendered backwards, which is stated here from the
// fixture's own shape rather than read back out of an arm.
public sealed partial class ReverseNodesInKGroupBenchmarksTests
{
    private const int SmallestLength = 200;

    // Both arms take the group size from the class's own constant.
    private const int GroupSize = 4;

    [Fact]
    public void ArrayGroupReverse_FullGroupsOfFour_ReversesEveryGroup() =>
        Assert.Equal(ExpectedGroupsReversed(SmallestLength), ValuesOf(BuildHarness().ArrayGroupReverse()));

    private static int[] ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // Length is an exact multiple of the group size, so every run is complete.
    private static int[] ExpectedGroupsReversed(int length)
    {
        var values = Enumerable.Range(1, length).ToArray();

        for (var start = 0; start < values.Length; start += GroupSize)
        {
            Array.Reverse(values, start, GroupSize);
        }

        return values;
    }

    private static ReverseNodesInKGroupBenchmarks BuildHarness()
    {
        var harness = new ReverseNodesInKGroupBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
