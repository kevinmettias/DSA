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
            var isOutsideRangeOrAlreadyPlaced =
                value < SmallestValue || value >= placed.Length || placed[value];

            if (isOutsideRangeOrAlreadyPlaced)
            {
                return false;
            }

            placed[value] = true;
        }

        return HasNoAveragingTriple(answer);
    }

    // The triple walk is one helper per starting index rather than three nested loops in
    // one body: same i < k < j order, same first-hit early exit, but each body stays two
    // levels deep and the k/j walk carries a name that says what it is looking for.
    private static bool HasNoAveragingTriple(int[] answer)
    {
        for (var i = 0; i < answer.Length; i++)
        {
            if (HasAveragingTripleStartingAt(answer, i))
            {
                return false;
            }
        }

        return true;
    }

    // Whether any pair after `startIndex` averages back to the value at it: for i < k < j,
    // a[k] is the mean of a[i] and a[j] exactly when 2 * a[k] is their sum.
    private static bool HasAveragingTripleStartingAt(int[] answer, int startIndex)
    {
        for (var k = startIndex + 1; k < answer.Length; k++)
        {
            for (var j = k + 1; j < answer.Length; j++)
            {
                if ((ArithmeticMeanMultiplier * answer[k]) == answer[startIndex] + answer[j])
                {
                    return true;
                }
            }
        }

        return false;
    }
}
