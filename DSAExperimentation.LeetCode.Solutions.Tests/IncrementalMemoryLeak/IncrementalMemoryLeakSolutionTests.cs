using DSAExperimentation.LeetCode.Solutions.IncrementalMemoryLeak;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IncrementalMemoryLeak;

// Harness only. Both the arithmetic baseline and the max-heap simulation are
// IncrementalMemoryLeakSolution's - this file just pins them to a lopsided pair
// whose larger stick takes every allocation until the crash and leaves the smaller
// one untouched, one theory per strategy so a failure names the arm that broke.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class IncrementalMemoryLeakSolutionTests
{
    public static TheoryData<int, int, int[]> Cases =>
        new()
        {
            { 100, 1, [14, 9, 1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MemoryLeakByArithmetic_Cases_ReturnsCrashSecondAndRemainingMemory(
        int memory1, int memory2, int[] expected)
    {
        var actual = IncrementalMemoryLeakSolution.MemoryLeakByArithmetic(memory1, memory2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MemoryLeakByMaxHeap_Cases_ReturnsCrashSecondAndRemainingMemory(
        int memory1, int memory2, int[] expected)
    {
        var actual = IncrementalMemoryLeakSolution.MemoryLeakByMaxHeap(memory1, memory2);

        Assert.Equal(expected, actual);
    }
}
