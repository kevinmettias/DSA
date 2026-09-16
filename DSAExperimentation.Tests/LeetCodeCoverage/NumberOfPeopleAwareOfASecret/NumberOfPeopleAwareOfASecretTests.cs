using DSAExperimentation.LeetCode.NumberOfPeopleAwareOfASecret;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPeopleAwareOfASecret;

// Harness only. Both strategies are NumberOfPeopleAwareOfASecretSolution's -
// including the quadratic sliding-window sum, which the benchmark used to own
// privately as its baseline and nothing asserted.
public sealed class NumberOfPeopleAwareOfASecretTests
{
    public static TheoryData<int, int, int, long> Examples =>
        new()
        {
            // LC example 1.
            { 6, 2, 4, 5 },

            // LC example 2: delay 1, so a person shares the day after learning.
            { 4, 1, 3, 6 },

            // Two days, forget the day after sharing starts.
            { 2, 1, 2, 2 },

            // The smallest n LC allows: only person 1 exists, and day 1 is
            // inside every forget window, so nobody has forgotten yet.
            { 1, 1, 2, 1 },

            // delay = forget - 1, the narrowest sharing window there is: exactly
            // one new person per day from day 2 on.
            { 6, 1, 2, 2 },

            // The gap between delay and forget leaves day 2 with no sharers at
            // all, so a zero has to propagate through the window sums.
            { 5, 2, 4, 3 },

            // Long enough that the window slides fully off the front, which is
            // where an off-by-one in the low bound shows up.
            { 10, 2, 5, 30 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void PeopleWithSecretBySlidingWindowSum_LeetCodeExamples_ReturnsPeopleStillRemembering(
        int n, int delay, int forget, long expected)
    {
        var actual =
            NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretBySlidingWindowSum(n, delay, forget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void PeopleWithSecretByFenwickRangeSum_LeetCodeExamples_ReturnsPeopleStillRemembering(
        int n, int delay, int forget, long expected)
    {
        var actual =
            NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretByFenwickRangeSum(n, delay, forget);

        Assert.Equal(expected, actual);
    }
}
