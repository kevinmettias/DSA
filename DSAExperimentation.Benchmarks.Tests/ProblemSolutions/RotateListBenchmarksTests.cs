using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RotateListBenchmarks (ARCHITECTURE 17.9). Arm agreement is BenchmarkArmsTests'
// job; this pins the baseline's answer to one derived from the workload's shape. [GlobalSetup] builds
// the values -100, -99, ... in order - wrapping only past LC 61's highest value, 100, which the
// smallest length never reaches - and the rotation is derived from Length alone. Because the chain
// is in ascending order, the rotation's cut point is visible on the answer itself: rotating right by
// k moves the last k values to the front and leaves the rest in order.
public sealed partial class RotateListBenchmarksTests
{
    private const int SmallestLength = 200;

    // Both arms rotate by a third of the list so they do equivalent work.
    private const int RotationDivisor = 3;

    // LC 61's lowest node value, where the chain starts.
    private const int MinNodeValue = -100;

    [Fact]
    public void ArrayRebuild_OneThirdRotation_MovesTheLastThirdToTheFront() =>
        Assert.Equal(ExpectedRotatedRight(SmallestLength), ValuesOf(BuildHarness().ArrayRebuild()));

    private static int[] ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // Rotating the ascending chain MinNodeValue..lastValue right by length / divisor moves its last
    // length / divisor values to the front and leaves the rest in ascending order.
    private static int[] ExpectedRotatedRight(int length)
    {
        var shift = length / RotationDivisor;
        var lastValue = MinNodeValue + length - 1;
        var lastValueLeftBehind = lastValue - shift;
        var values = new List<int>();

        for (var value = lastValueLeftBehind + 1; value <= lastValue; value++)
        {
            values.Add(value);
        }

        for (var value = MinNodeValue; value <= lastValueLeftBehind; value++)
        {
            values.Add(value);
        }

        return [.. values];
    }

    private static RotateListBenchmarks BuildHarness()
    {
        var harness = new RotateListBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
