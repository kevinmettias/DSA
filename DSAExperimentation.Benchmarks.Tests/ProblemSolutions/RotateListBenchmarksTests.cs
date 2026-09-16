using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RotateListBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - materializing the values, rotating the array by slicing and rebuilding a
// fresh list against finding the cut point on the existing nodes and rewriting three pointers - so a
// harness whose arms disagree is timing two different problems. [GlobalSetup] builds the values
// 1..Length in order and the rotation is derived from Length alone, so the same Length must rebuild
// the same workload; otherwise two published numbers were never comparable in the first place.
//
// Because the chain is in ascending order, the rotation's cut point is visible on the answer itself:
// rotating right by k moves the last k values to the front and leaves the rest in order. Both arms
// return the new head as object? (the node type is internal, CS0050) and rebuild or rewire the chain
// inside the measured call, so one harness is safe to call twice in either order.
public sealed partial class RotateListBenchmarksTests
{
    private const int SmallestLength = 200;

    // Both arms rotate by a third of the list so they do equivalent work.
    private const int RotationDivisor = 3;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())),
            AnswerText.Of(ValuesOf(BuildHarness().ArrayRebuild())));

    [Fact]
    public void ArrayRebuild_OneThirdRotation_AgreesWithPointerRewire()
    {
        var harness = BuildHarness();
        var rotated = harness.ArrayRebuild();

        Assert.Equal(ExpectedRotatedRight(SmallestLength), ValuesOf(rotated));
        Assert.Equal(AnswerText.Of(ValuesOf(harness.PointerRewire())), AnswerText.Of(ValuesOf(rotated)));
    }

    [Fact]
    public void PointerRewire_OneThirdRotation_AgreesWithArrayRebuild()
    {
        var harness = BuildHarness();
        var rotated = harness.PointerRewire();

        Assert.Equal(ExpectedRotatedRight(SmallestLength), ValuesOf(rotated));
        Assert.Equal(AnswerText.Of(ValuesOf(harness.ArrayRebuild())), AnswerText.Of(ValuesOf(rotated)));
    }

    private static int[] ValuesOf(object? head)
    {
        var values = new List<int>();

        for (var node = (SinglyLinkedListNode<int>?)head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    // Rotating a 1..length chain right by length / divisor moves its last length / divisor values to
    // the front and leaves the rest in ascending order.
    private static int[] ExpectedRotatedRight(int length)
    {
        var shift = length / RotationDivisor;
        var values = new List<int>();

        for (var value = length - shift + 1; value <= length; value++)
        {
            values.Add(value);
        }

        for (var value = 1; value <= length - shift; value++)
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
