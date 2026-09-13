using DSAExperimentation.LeetCode.CountAllValidPickupAndDeliveryOptions;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountAllValidPickupAndDeliveryOptions;

// Harness only. Both strategies are CountAllValidPickupAndDeliveryOptionsSolution's -
// this file pins them to LeetCode's published examples (n = 1, 2, 3), two more terms
// of the closed form (2n)! / 2^n that the recurrence generates (n = 4, 5), and n = 100,
// which is past the point where the running product must be reduced mod 1e9+7.
public sealed class CountAllValidPickupAndDeliveryOptionsTests
{
    public static TheoryData<int, long> Examples =>
        new()
        {
            { 1, 1L },
            { 2, 6L },
            { 3, 90L },
            { 4, 2_520L },
            { 5, 113_400L },
            { 100, 14_159_051L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOrdersByTabulation_LeetCodeExamples_ReturnsValidSequenceCount(int n, long expected) =>
        Assert.Equal(expected, CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByTabulation(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountOrdersByMemoizedRecurrence_LeetCodeExamples_ReturnsValidSequenceCount(int n, long expected) =>
        Assert.Equal(expected, CountAllValidPickupAndDeliveryOptionsSolution.CountOrdersByMemoizedRecurrence(n));
}
