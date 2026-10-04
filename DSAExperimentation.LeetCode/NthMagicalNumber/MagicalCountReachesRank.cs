using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.NthMagicalNumber;

// LC 878's binary search asks one question of every candidate: are at least rank
// magical numbers at or below it, counted by inclusion-exclusion over the multiples of
// the first factor, of the second, and of their lcm. The count never falls as the
// candidate grows, so the rule is monotone. It is stated over long because the answer
// can reach rank * min(factor) = 10^9 * 4 * 10^4, far past int. This rule answers that
// one question for that one problem, so it lives beside the solution (ARCHITECTURE.md
// section 17.3: a witness that answers one problem belongs in that problem's folder).
internal readonly struct MagicalCountReachesRank(int rank, int firstFactor, int secondFactor, long lcm)
    : IMonotonePredicate<long>
{
    public bool Holds(long candidate)
    {
        var multiplesUpToCandidate = (candidate / firstFactor) + (candidate / secondFactor) - (candidate / lcm);

        return multiplesUpToCandidate >= rank;
    }
}
