namespace DSAExperimentation.Tests.LeetCodeCoverage.MirrorReflection;

// LeetCode 858. Mirror Reflection: reduce (p, q) by their GCD (the Euclidean
// algorithm, this repo's own inline-primitive precedent from MaxPointsOnALine's
// slope-reduction and Reaching Points' backward-reduction loop) then read the
// receptor off the reduced pair's parity. No repo container/algorithm primitive
// applies to a single running (p, q) pair - the same "lighter repo-primitive fit"
// category Reaching Points/Pow(x,n) already establish.
public sealed class MirrorReflectionTests
{
    [Theory]
    [InlineData(2, 1, 2)]
    [InlineData(3, 1, 1)]
    [InlineData(3, 2, 0)]
    public void MirrorReflection_LeetCodeExamples_ReturnsExpectedReceptor(int p, int q, int expected)
        => Assert.Equal(expected, MirrorReflection(p, q));

    private static int MirrorReflection(int p, int q)
    {
        var g = Gcd(p, q);
        var pPrime = p / g;
        var qPrime = q / g;

        if (pPrime % 2 == 1 && qPrime % 2 == 0)
        {
            return 0;
        }

        return pPrime % 2 == 1 ? 1 : 2;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
