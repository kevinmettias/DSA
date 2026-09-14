using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.NumberOfCommonFactors;

// LeetCode 2427. Number of Common Factors: how many positive integers divide both
// a and b.
//
// Every common factor of a and b is exactly a divisor of gcd(a, b), so the two
// strategies differ in how far they are willing to look: the baseline tests every
// candidate from 1 up to min(a, b) against both numbers, while the composed answer
// reduces to the gcd first and enumerates only ITS divisors up to sqrt(gcd),
// picking up the far side of each divisor pair for free.
internal static class NumberOfCommonFactorsSolution
{
    // The textbook answer: try every candidate divisor up to min(a, b) and keep the
    // ones that divide both. Deliberately written without this repo's primitives -
    // no gcd reduction, no set - it is the arm the composed solution below has to
    // justify itself against. O(min(a, b)).
    public static int CountCommonFactorsByLinearScan(int first, int second)
    {
        var count = 0;
        var limit = Math.Min(first, second);

        for (var i = 1; i <= limit; i++)
        {
            if (first % i == 0 && second % i == 0)
            {
                count++;
            }
        }

        return count;
    }

    // Reduce to gcd(a, b) by Euclid, then walk i up to sqrt(gcd): each divisor i
    // found pairs with gcd/i on the far side, so nothing above the square root ever
    // has to be tested directly. This repo's own Set<int> (HashMap-backed) absorbs
    // the one case where those two coincide - a perfect-square gcd, where i*i == gcd
    // offers the same divisor twice - which is the whole reason the count is a set
    // size rather than a running total. O(sqrt(gcd) + log min(a, b)).
    public static int CountCommonFactorsByDivisorEnumeration(int first, int second)
    {
        var greatestCommonDivisor = GreatestCommonDivisor(first, second);
        var divisors = new Set<int>();

        for (var i = 1; (long)i * i <= greatestCommonDivisor; i++)
        {
            if (greatestCommonDivisor % i == 0)
            {
                divisors.TryAdd(i);
                divisors.TryAdd(greatestCommonDivisor / i);
            }
        }

        return divisors.Count;
    }

    // Euclid's algorithm: plain arithmetic, with no data structure of its own.
    private static int GreatestCommonDivisor(int first, int second)
    {
        while (second != 0)
        {
            (first, second) = (second, first % second);
        }

        return first;
    }
}
