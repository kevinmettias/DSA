using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReverseNodesInEvenLengthGroupsBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the baseline's answer to one derived from the workload's shape.
// [GlobalSetup] builds the values 1..Length in order, and the boundary rule fixes the answer
// independently of either data structure: the groups are 1, 2, 3, ... nodes with the final one
// taking whatever is left, and it is the final group's REAL length - not the size it was reaching
// for - that decides whether it reverses. The chain is still in ascending order, so each reversed
// run is visible as a descending stretch of the same values.
public sealed partial class ReverseNodesInEvenLengthGroupsBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int FirstGroupLength = 1;
    private const int SecondGroupSize = 2;

    [Fact]
    public void ArrayEvenLengthGroupReverse_GrowingGroups_ReversesEachEvenLengthRun() =>
        Assert.Equal(ExpectedEvenLengthRunsReversed(SmallestLength), ValuesOf(BuildHarness().ArrayEvenLengthGroupReverse()));

    private static int[] ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // The first group is a single node (odd, so it never reverses) and the size grows by one from
    // the second group on, with the final group taking only the nodes that remain.
    private static int[] ExpectedEvenLengthRunsReversed(int length)
    {
        var values = Enumerable.Range(1, length).ToArray();
        var start = FirstGroupLength;
        var groupSize = SecondGroupSize;

        while (start < values.Length)
        {
            var actualLength = Math.Min(groupSize, values.Length - start);

            if (actualLength % SecondGroupSize == 0)
            {
                Array.Reverse(values, start, actualLength);
            }

            start += actualLength;
            groupSize++;
        }

        return values;
    }

    private static ReverseNodesInEvenLengthGroupsBenchmarks BuildHarness()
    {
        var harness = new ReverseNodesInEvenLengthGroupsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
