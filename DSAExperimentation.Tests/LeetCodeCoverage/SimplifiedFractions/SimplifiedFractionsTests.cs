namespace DSAExperimentation.Tests.LeetCodeCoverage.SimplifiedFractions;

// LeetCode 1447. Simplified Fractions: numerator/denominator is already in simplest
// form iff gcd(numerator, denominator) == 1, so enumerating every denominator 2..n and
// every numerator 1..denominator-1 with a gcd check produces every simplified fraction
// exactly once, with no duplicate to filter. No repo container or algorithm primitive
// applies here - there is nothing to compose over two running integers and a result
// list, the same "nothing to compose over plain integers" case PowXnTests/
// CheckIfItIsAGoodArrayTests already are, and the same private Euclidean Gcd helper
// NthMagicalNumberTests/XOfAKindInADeckOfCardsTests/CheckIfItIsAGoodArrayTests already
// reuse inline rather than promoting to a shared production type.
public sealed class SimplifiedFractionsTests
{
    [Fact]
    public void SimplifiedFractions_NIsThree_ReturnsEveryCoprimeNumeratorDenominatorPair()
    {
        var result = SimplifiedFractionsUpTo(3);

        Assert.Equal(["1/2", "1/3", "2/3"], result);
    }

    [Fact]
    public void SimplifiedFractions_NIsOne_ReturnsEmptyList()
    {
        var result = SimplifiedFractionsUpTo(1);

        Assert.Empty(result);
    }

    [Fact]
    public void SimplifiedFractions_NIsFour_OmitsReducibleTwoFourths()
    {
        var result = SimplifiedFractionsUpTo(4);

        Assert.Equal(["1/2", "1/3", "2/3", "1/4", "3/4"], result);
        Assert.DoesNotContain("2/4", result);
    }

    private static List<string> SimplifiedFractionsUpTo(int n)
    {
        var fractions = new List<string>();

        for (var denominator = 2; denominator <= n; denominator++)
        {
            for (var numerator = 1; numerator < denominator; numerator++)
            {
                if (Gcd(numerator, denominator) == 1)
                {
                    fractions.Add($"{numerator}/{denominator}");
                }
            }
        }

        return fractions;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
