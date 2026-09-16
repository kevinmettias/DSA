using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.FindTheNthValueAfterKSeconds;

// LeetCode 3179. Find the N-th Value After K Seconds: an array of arrayLength ones
// is updated secondCount times, each second replacing every a[i] (i >= 1, left to
// right, so each update sees the already-updated a[i-1]) with a[i-1] + a[i]; report
// a[n-1] mod 1e9+7.
//
// Both strategies answer the same question with the same signature, so the
// test harness can assert they agree and the benchmark harness can time them
// against each other without either restating the algorithm.
internal static class FindTheNthValueAfterKSecondsSolution
{
    // The textbook answer: run the secondCount seconds literally, one running-prefix-sum
    // pass per second. O(n * k), deliberately BCL-only (long[] only) - the arm
    // the closed-form strategy below has to justify itself against.
    public static int ValueAfterKSecondsByBruteForce(int arrayLength, int secondCount)
    {
        var values = new long[arrayLength];
        Array.Fill(values, 1L);

        for (var second = 0; second < secondCount; second++)
        {
            for (var i = 1; i < arrayLength; i++)
            {
                values[i] = (values[i] + values[i - 1]) % ModularArithmetic.Modulo;
            }
        }

        return (int)values[arrayLength - 1];
    }

    // Composed: secondCount rounds of prefix-summing an all-ones array is exactly
    // Pascal's triangle by another name, so a[n-1] after secondCount seconds is the
    // closed-form binomial coefficient C(n - 1 + k, k). Computed as a factorial ratio
    // using Domain.Modular's Inverse (Fermat's-little-theorem modular inverse) for the
    // denominator, the same "reporting modulo 1e9+7 is a LeetCode convention,
    // not an algorithm property" reuse every counting problem in this catalogue
    // shares. O(n + k).
    public static int ValueAfterKSecondsByModularBinomial(int arrayLength, int secondCount)
    {
        var upper = arrayLength - 1 + secondCount;
        var factorial = new long[upper + 1];
        factorial[0] = 1;

        for (var i = 1; i <= upper; i++)
        {
            factorial[i] = factorial[i - 1] * i % ModularArithmetic.Modulo;
        }

        var denominator =
            factorial[secondCount] * factorial[arrayLength - 1] % ModularArithmetic.Modulo;
        var result = factorial[upper] * ModularArithmetic.Inverse(denominator) % ModularArithmetic.Modulo;

        return (int)result;
    }
}
