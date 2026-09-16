using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BeautifulArrayBenchmarks (ARCHITECTURE 17.9): its two arms are BeautifulArraySolution's
// competing strategies for the same question - a pruned prefix search against a memoized divide and conquer - but
// this is the one class in the batch where ARM AGREEMENT IS NOT AN AVAILABLE ASSERTION, and deliberately so.
// LC 932 accepts ANY beautiful permutation, and the solution's own note records that the two strategies return
// different ones from n = 8 onward: the backtracking arm returns the lexicographically smallest, the divide and
// conquer arm returns the one its parity recursion builds. Neither is wrong, so disagreeing here is not a defect
// and there is nothing for the root-cause rule to fix. What the harness asserts instead is the property the
// problem actually pins, independently re-derived: each arm's answer is a permutation of 1..length in which no
// element is the arithmetic mean of one before it and one after, at BOTH [Params] sizes - the larger one being
// exactly where the two answers diverge, so a test that quietly compared them would fail for the wrong reason.
public sealed partial class BeautifulArrayBenchmarksTests
{
    // The smaller of [Params(6, 8)] lengths.
    private const int SmallestLength = 6;

    // The larger [Params] value, where the solution documents the two answers first diverging.
    private const int LargestLength = 8;

    // 2 * a[k] == a[i] + a[j] is the averaging rule the whole problem is about.
    private const int ArithmeticMeanMultiplier = 2;

    // The permutation is 1-based, so value 0 is never a valid element and the seen-table needs a
    // slot above the largest one.
    private const int SmallestValue = 1;

    [Fact]
    public void PrunedBacktrackingSearch_SixAndEightElementPermutations_ReturnsBeautifulArrays()
    {
        Assert.True(IsBeautiful(Harness(SmallestLength).PrunedBacktrackingSearch(), SmallestLength));
        Assert.True(IsBeautiful(Harness(LargestLength).PrunedBacktrackingSearch(), LargestLength));
    }

    [Fact]
    public void MemoizedDivideAndConquer_SixAndEightElementPermutations_ReturnsBeautifulArrays()
    {
        Assert.True(IsBeautiful(Harness(SmallestLength).MemoizedDivideAndConquer(), SmallestLength));
        Assert.True(IsBeautiful(Harness(LargestLength).MemoizedDivideAndConquer(), LargestLength));
    }

    private static BeautifulArrayBenchmarks Harness(int length) => new() { Length = length };

    // LC 932's contract, re-derived rather than read off either arm: every value from 1 to length
    // appears exactly once, and no triple i < k < j averages.
    private static bool IsBeautiful(int[] answer, int length)
    {
        if (answer.Length != length)
        {
            return false;
        }

        var placed = new bool[length + SmallestValue];

        foreach (var value in answer)
        {
            if (value < SmallestValue || value >= placed.Length || placed[value])
            {
                return false;
            }

            placed[value] = true;
        }

        return HasNoAveragingTriple(answer);
    }

    private static bool HasNoAveragingTriple(int[] answer)
    {
        for (var i = 0; i < answer.Length; i++)
        {
            for (var k = i + 1; k < answer.Length; k++)
            {
                for (var j = k + 1; j < answer.Length; j++)
                {
                    if ((ArithmeticMeanMultiplier * answer[k]) == answer[i] + answer[j])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}
