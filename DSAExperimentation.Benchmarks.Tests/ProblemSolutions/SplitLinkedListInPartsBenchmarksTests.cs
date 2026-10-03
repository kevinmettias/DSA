using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitLinkedListInPartsBenchmarks (ARCHITECTURE 17.9): both arms are
// SplitLinkedListInPartsSolution's splits of the same prepared chain into the same number of
// parts - one rebuilding from an array, the other rewiring in place - so a harness whose arms
// disagree is timing two different problems. Setup is a pure function of Length and each arm
// clones the chain before mutating it, so the same Length must rebuild the same values and one
// harness is safe to call twice in either order.
//
// Each arm returns the parts themselves, and they are derivable from Length alone: node i holds
// the value i, and LC 725 cuts the chain into consecutive parts whose sizes differ by at most one,
// the larger ones first. Length is far larger than the part count, so every part is non-empty.
// Each arm is asserted against those parts, which catches a dropped node and a misplaced cut alike.
public sealed partial class SplitLinkedListInPartsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the part count the benchmark itself splits into.
    private const int ExpectedPartCount = 7;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameChain() =>
        Assert.Equal(AnswerGraphText.Of(BuildHarness().ArrayRebuild()), AnswerGraphText.Of(BuildHarness().ArrayRebuild()));

    [Fact]
    public void ArrayRebuild_TwoHundredNodes_CutsEqualConsecutiveParts() =>
        AssertExpectedParts(BuildHarness().ArrayRebuild());

    [Fact]
    public void InPlaceRewire_TwoHundredNodes_CutsEqualConsecutiveParts() =>
        AssertExpectedParts(BuildHarness().InPlaceRewire());

    private static void AssertExpectedParts(object? answer)
    {
        var parts = Assert.IsType<SinglyLinkedListNode<int>?[]>(answer);

        Assert.Equal(ExpectedPartCount, parts.Count(part => part is not null));
        Assert.Equal(ExpectedParts(), parts.Select(ValuesOf));
    }

    private static IEnumerable<int[]> ExpectedParts()
    {
        var shortPartSize = SmallestLength / ExpectedPartCount;
        var longPartCount = SmallestLength % ExpectedPartCount;
        var start = 0;

        for (var part = 0; part < ExpectedPartCount; part++)
        {
            var size = part < longPartCount ? shortPartSize + 1 : shortPartSize;

            yield return [.. Enumerable.Range(start, size)];

            start += size;
        }
    }

    private static int[] ValuesOf(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return [.. values];
    }

    private static SplitLinkedListInPartsBenchmarks BuildHarness()
    {
        var harness = new SplitLinkedListInPartsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
