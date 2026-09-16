using DSAExperimentation.LeetCode.IncrementalMemoryLeak;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IncrementalMemoryLeak;

// Harness only. Both the arithmetic baseline and the max-heap simulation are
// IncrementalMemoryLeakSolution's - this file just pins them to LeetCode's
// published examples plus the tie and exhausted-stick edges the heap's ordering
// has to get right, one theory per strategy so a failure names the arm that broke.
public sealed class IncrementalMemoryLeakTests
{
    public static TheoryData<int, int, int[]> Examples =>
        new()
        {
            { 2, 2, [3, 1, 0] },
            { 8, 11, [6, 0, 4] },
            { 0, 0, [1, 0, 0] },
            { 1, 0, [2, 0, 0] },
            { 0, 1, [2, 0, 0] },
            { 100, 1, [14, 9, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MemoryLeakByArithmetic_LeetCodeExamples_ReturnsCrashSecondAndRemainingMemory(
        int memory1, int memory2, int[] expected)
    {
        var actual = IncrementalMemoryLeakSolution.MemoryLeakByArithmetic(memory1, memory2);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MemoryLeakByMaxHeap_LeetCodeExamples_ReturnsCrashSecondAndRemainingMemory(
        int memory1, int memory2, int[] expected)
    {
        var actual = IncrementalMemoryLeakSolution.MemoryLeakByMaxHeap(memory1, memory2);

        Assert.Equal(expected, actual);
    }
}
