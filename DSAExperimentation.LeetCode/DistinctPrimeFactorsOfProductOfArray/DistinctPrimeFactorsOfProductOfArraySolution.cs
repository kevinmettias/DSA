using System.Numerics;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.DistinctPrimeFactorsOfProductOfArray;

// LeetCode 2521. Distinct Prime Factors of Product of Array: how many distinct
// prime factors the product of every element has.
//
// The product's prime factors are exactly the union of each element's own prime
// factors, so the product itself never has to exist. The two strategies are
// precisely the two readings of that sentence: build the product and factor it, or
// factor each element and union the results.
internal static class DistinctPrimeFactorsOfProductOfArraySolution
{
    private const int SmallestPrime = 2;

    // The textbook answer: multiply everything out into a BigInteger, then trial
    // divide the product by every candidate up to the largest element - no prime
    // factor of the product can exceed that, since every one of them divides some
    // element. Deliberately BCL-only, and deliberately paying for a product whose
    // limb count grows with every multiply.
    public static int DistinctPrimeFactorsByProductTrialDivision(int[] nums)
    {
        var product = BuildProduct(nums);
        var largest = LargestElement(nums);
        var count = 0;

        for (var divisor = SmallestPrime; divisor <= largest; divisor++)
        {
            if (product % divisor != 0)
            {
                continue;
            }

            count++;

            while (product % divisor == 0)
            {
                product /= divisor;
            }
        }

        return count;
    }

    private static BigInteger BuildProduct(int[] nums)
    {
        BigInteger product = 1;

        foreach (var num in nums)
        {
            product *= num;
        }

        return product;
    }

    private static int LargestElement(int[] nums)
    {
        var largest = 0;

        foreach (var num in nums)
        {
            largest = Math.Max(largest, num);
        }

        return largest;
    }

    // Factor each element on its own - every element is small, so trial division
    // stops at its own square root - and dedup the factors through this repo's own
    // Set<int>, the same trial-division-into-Set<int> composition
    // NumberOfCommonFactors and LargestComponentSizeByCommonFactor use. The
    // astronomically large product is never formed.
    public static int DistinctPrimeFactorsByElementFactorSet(int[] nums)
    {
        var primes = new Set<int>();

        foreach (var num in nums)
        {
            AddPrimeFactors(num, primes);
        }

        return primes.Count;
    }

    private static void AddPrimeFactors(int value, Set<int> primes)
    {
        var remaining = value;

        for (var divisor = SmallestPrime; divisor * divisor <= remaining; divisor++)
        {
            if (remaining % divisor != 0)
            {
                continue;
            }

            primes.TryAdd(divisor);

            while (remaining % divisor == 0)
            {
                remaining /= divisor;
            }
        }

        if (remaining > 1)
        {
            primes.TryAdd(remaining);
        }
    }
}
