using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SplitLinkedListInPartsBenchmarks (ARCHITECTURE 17.9): each arm returns the
// parts themselves, and they are derivable from Length alone: node i holds the value i, and LC 725
// cuts the chain into consecutive parts whose sizes differ by at most one, the larger ones first.
// Length is far larger than the part count, so every part is non-empty. The baseline is asserted
// against those parts, which catches a dropped node and a misplaced cut alike; the generic arm
// check holds the other arm to the baseline's answer.
public sealed partial class SplitLinkedListInPartsBenchmarksTests
{
    private const int SmallestLength = 200;

    // Mirrors the part count the benchmark itself splits into.
    private const int ExpectedPartCount = 7;

    [Fact]
    public void ArrayRebuild_TwoHundredNodes_CutsEqualConsecutiveParts() =>
        AssertExpectedParts(BuildHarness().ArrayRebuild());

    private static void AssertExpectedParts(SinglyLinkedListNode<int>?[] parts)
    {
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
