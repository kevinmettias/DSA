using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.LeetCode.UglyNumberIII;

// LC 1201's binary search asks one question of every candidate: are at least rank ugly
// numbers at or below it. The count never falls as the candidate grows, so the rule is
// monotone. This rule answers that one question for that one problem, so it lives
// beside the solution - the same placement NthMagicalNumber's MagicalCountReachesRank
// gets.
//
// Two values meet here and stay apart: the window searched, whose end is derived below
// rather than passed in - rank multiples of the smallest factor alone already reach
// rank, so it is bounded before the first probe and no caller can hand in a window the
// count does not actually hold over - and the counting rule itself, which is
// MultiplesOfThreeFactors' subject and not this type's. That end is rank times the
// smallest factor, up to 10^18 at LeetCode's limits and past int well before then, so
// the window and every candidate in it are long.
internal readonly struct UglyCountReachesRank : IMonotonePredicate<long>
{
    private readonly int _target;
    private readonly MultiplesOfThreeFactors _multiples;

    // The last candidate the search needs to consider.
    public long UpperBound { get; }

    public UglyCountReachesRank(int rank, int firstFactor, int secondFactor, int thirdFactor)
    {
        _target = rank;
        var smallestFactor = Math.Min(firstFactor, secondFactor);
        UpperBound = (long)rank * Math.Min(smallestFactor, thirdFactor);
        _multiples = new MultiplesOfThreeFactors(firstFactor, secondFactor, thirdFactor, UpperBound);
    }

    // Holds(candidate) treats the candidate itself as the ugly number in question, the
    // same "the answer is the search space" shape MagicalCountReachesRank uses.
    public bool Holds(long candidate) => _multiples.CountUpTo(candidate) >= _target;
}
