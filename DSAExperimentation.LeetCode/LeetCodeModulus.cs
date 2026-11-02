using DSAExperimentation.Domain.Mathematics.Modular;

namespace DSAExperimentation.LeetCode;

// LeetCode's own convention: a counting answer too large for an int is reported modulo the prime
// 10^9 + 7. The prime is LeetCode's choice, not a fact of the problems, so it sits at the LeetCode
// tier's root beside the other rules LeetCode fixes for every problem. Value is a constant, which
// every arm reads, baselines included: the owner ruled (2026-11-01) that a baseline may read it as it
// reads LeetCodeAnswer.None, since it is LeetCode's notation rather than the library's (ARCHITECTURE
// 17.5). As a prime modulus witness it also chooses the modulus for FactorialTable and
// ModularInverse.
internal readonly struct LeetCodeModulus : IPrimeModulus
{
    public const long Value = 1_000_000_007;

    static long IModulus.Value => Value;
}
