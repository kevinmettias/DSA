using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Domain.Mathematics.NumberTheory;

namespace DSAExperimentation.Tests.Domain.Mathematics.NumberTheory;

// The gcd's identity, held to Algorithms/NumberTheory's procedures over values of either sign:
// Euclid's algorithm (GreatestCommonDivisor) and the gcd as a segment tree's combine
// (GcdOperation).
public sealed partial class CommonDivisorsTests
{
    private const int Bound = 1_000;

    private static IEnumerable<int> Values => Enumerable.Range(-Bound, (2 * Bound) + 1);

    [Fact]
    public void GcdIdentity_LeavesEveryValueItsMagnitude() =>
        Assert.All(
            Values,
            value =>
            {
                var magnitude = Math.Abs(value);
                var identityFirst = GreatestCommonDivisor.Of(CommonDivisors.GcdIdentity, value);
                var identitySecond = GreatestCommonDivisor.Of(value, CommonDivisors.GcdIdentity);

                Assert.Equal(magnitude, identityFirst);
                Assert.Equal(magnitude, identitySecond);
            });

    [Fact]
    public void GcdIdentity_IsTheCombineOperationsIdentity()
    {
        Assert.Equal(CommonDivisors.GcdIdentity, GcdOperation<int>.Identity);
        Assert.Equal(CommonDivisors.GcdIdentity, GcdOperation<long>.Identity);
    }

    // Any other value e fails as an identity at |e| + 1, which it shares only the divisor 1 with.
    [Fact]
    public void GcdIdentity_IsTheOnlyIdentity() =>
        Assert.All(
            Values.Where(candidate => candidate != CommonDivisors.GcdIdentity),
            candidate =>
            {
                var next = Math.Abs(candidate) + 1;
                var divisor = GreatestCommonDivisor.Of(candidate, next);

                Assert.NotEqual(next, divisor);
            });
}
