using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubsetsIIBenchmarks (ARCHITECTURE 17.9): the class carries a single arm - this
// repo's backtracking combinator with the duplicate-skipping candidate rule - so there is no second
// strategy to reconcile it against and the assertion has to come from the workload instead. The arm
// returns the multiset power set as an outer list whose order LeetCode 90 never fixes, which is the
// one family OfUnorderedSet exists for; within each subset both the arm and the oracle below emit the
// values ascending, so that inner order stays checked.
//
// The oracle is derived from what [GlobalSetup] lays down rather than read back out of the arm: Setup
// writes DuplicateGroupSize copies of each of 0, 1, 2, ... in order (the benchmark keeps that
// duplicate-group size private, so it is mirrored here), so the answer is exactly the set of
// sub-multisets of that multiset. Enumerating all 2^Length index subsets, sorting each and collapsing
// duplicates states that set without knowing anything about how the arm prunes its search.
public sealed partial class SubsetsIIBenchmarksTests
{
    // Mirrors SubsetsIIBenchmarks' own private DuplicateGroupSize.
    private const int DuplicateGroupSize = 2;
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(BuildHarness().BacktrackSkipDuplicates()),
            AnswerText.OfUnorderedSet(BuildHarness().BacktrackSkipDuplicates()));

    [Fact]
    public void BacktrackSkipDuplicates_DuplicatePairs_ReturnsThePowerSetOfTheMultiset() =>
        Assert.Equal(
            AnswerText.OfUnorderedSet(ExpectedMultisetPowerSet()),
            AnswerText.OfUnorderedSet(BuildHarness().BacktrackSkipDuplicates()));

    // Mirrors [GlobalSetup]'s own expression: DuplicateGroupSize copies of each value, in order.
    private static int[] WorkloadValues() =>
        Enumerable.Range(0, SmallestLength).Select(index => index / DuplicateGroupSize).ToArray();

    // Every sub-multiset of the workload, found by enumerating index subsets rather than values, so a
    // duplicate value contributes its copies separately and the collapse below is what removes the
    // repeats - the same set the arm's skip rule is supposed to produce.
    private static List<List<int>> ExpectedMultisetPowerSet()
    {
        var values = WorkloadValues();
        var subsets = new List<List<int>>();
        var seen = new HashSet<string>();

        for (var mask = 0; mask < 1 << SmallestLength; mask++)
        {
            var subset = new List<int>();

            for (var index = 0; index < values.Length; index++)
            {
                if ((mask & (1 << index)) != 0)
                {
                    subset.Add(values[index]);
                }
            }

            subset.Sort();

            if (seen.Add(AnswerText.Of(subset)))
            {
                subsets.Add(subset);
            }
        }

        return subsets;
    }

    private static SubsetsIIBenchmarks BuildHarness()
    {
        var harness = new SubsetsIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
