using DSAExperimentation.LeetCode.Solutions.NumberOfPeopleAwareOfASecret;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfPeopleAwareOfASecret;

// Harness only. Both strategies are NumberOfPeopleAwareOfASecretSolution's -
// including the quadratic sliding-window sum, which the benchmark used to own
// privately as its baseline and nothing asserted.
public sealed partial class NumberOfPeopleAwareOfASecretSolutionTests
{
    public static TheoryData<int, int, int, long> Cases =>
        new()
        {
            // Two days, forget the day after sharing starts.
            { 2, 1, 2, 2 },

            // The smallest day count LC allows: only person 1 exists, and day 1 is
            // inside every forget window, so nobody has forgotten yet.
            { 1, 1, 2, 1 },

            // delay = forget - 1, the narrowest sharing window there is: exactly
            // one new person per day from day 2 on.
            { 6, 1, 2, 2 },

            // The gap between delay and forget leaves day 2 with no sharers at
            // all, so a zero has to propagate through the window sums.
            { 5, 2, 4, 3 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void PeopleWithSecretBySlidingWindowSum_Cases_ReturnsPeopleStillRemembering(
        int dayCount, int delay, int forget, long expected)
    {
        var actual =
            NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretBySlidingWindowSum(dayCount, delay, forget);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void PeopleWithSecretByFenwickRangeSum_Cases_ReturnsPeopleStillRemembering(
        int dayCount, int delay, int forget, long expected)
    {
        var actual =
            NumberOfPeopleAwareOfASecretSolution.PeopleWithSecretByFenwickRangeSum(dayCount, delay, forget);

        Assert.Equal(expected, actual);
    }
}
