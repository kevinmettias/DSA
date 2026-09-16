using DSAExperimentation.LeetCode.RandomPickIndex;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RandomPickIndex;

// Harness only. Both strategies are RandomPickIndexSolution's - this replays
// repeated Pick(target) calls against each IRandomPickIndex implementation built
// from LeetCode's published nums. Pick's result is nondeterministic, so each
// example carries the full set of indices that are valid to return for its
// target rather than one exact value - the same "expected slot carries a
// candidate set" idea InsertDeleteGetRandomO1Tests already uses for GetRandom -
// plus, where the original test asserted it, that every valid index eventually
// gets returned across many calls.
public sealed class RandomPickIndexTests
{
    public static TheoryData<PickExample> Examples =>
        new()
        {
            { new PickExample(Nums: [1, 2, 3, 3, 3], Target: 3, ValidIndices: [2, 3, 4], Trials: 50, ExpectAllSeen: false) },
            { new PickExample(Nums: [5, 1, 5, 2, 5, 5], Target: 2, ValidIndices: [3], Trials: 1, ExpectAllSeen: true) },
            { new PickExample(Nums: [5, 1, 5, 2, 5, 5], Target: 5, ValidIndices: [0, 2, 4, 5], Trials: 200, ExpectAllSeen: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void Pick_LeetCodeExamples_AlwaysReturnsAValidIndexByReservoirSampling(PickExample example)
        => AssertPicksAreValid(
            new RandomPickIndexSolution.RandomPickIndexByReservoirSampling(example.Nums), example);

    [Theory]
    [MemberData(nameof(Examples))]
    public void Pick_LeetCodeExamples_AlwaysReturnsAValidIndexByHashMapGrouping(PickExample example)
        => AssertPicksAreValid(
            new RandomPickIndexSolution.RandomPickIndexByHashMapGrouping(example.Nums), example);

    private static void AssertPicksAreValid(
        RandomPickIndexSolution.IRandomPickIndex solution, PickExample example)
    {
        var seen = new HashSet<int>();

        for (var i = 0; i < example.Trials; i++)
        {
            var picked = solution.Pick(example.Target);
            Assert.Contains(picked, example.ValidIndices);
            seen.Add(picked);
        }

        if (example.ExpectAllSeen)
        {
            Assert.Equal(example.ValidIndices.OrderBy(index => index), seen.OrderBy(index => index));
        }
    }

    // One LeetCode example: the array Pick is asked about, the target whose occurrences are
    // the candidates, the whole candidate set (Pick may return any of them), how many calls
    // the example replays, and whether those calls must cover every candidate. The five
    // travel together into every assertion, so each is named rather than left as a position.
    public readonly record struct PickExample(
        int[] Nums,
        int Target,
        int[] ValidIndices,
        int Trials,
        bool ExpectAllSeen);
}
