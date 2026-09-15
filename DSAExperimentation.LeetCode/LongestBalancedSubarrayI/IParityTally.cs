namespace DSAExperimentation.LeetCode.LongestBalancedSubarrayI;

// LC 3719's one decision, named: which structure holds a single parity's distinct
// values while a window extends rightward. Both strategies run the identical O(n^2)
// scan and level the two parities' totals at every end index, so the container behind
// a parity's tally is the entire difference between them - and this interface is where
// that container's contract gets written down, which the bare `Func<Func<int, int>>`
// factory the scan used to take had nowhere to put.
//
// The tally itself stays a plain HashSet<int> or Set<int> rather than becoming an
// object of its own: the arms differ in the SET they hand back, and giving each arm a
// stateless singleton over a scope type it does not have to name keeps both arms'
// scans one method.
internal interface IParityTally<TSet>
{
    // The empty tally one parity of a newly started window begins from. A window needs
    // two of these, and each has to be its own: counting *distinct* values is only
    // meaningful per parity, so nothing may be carried between the two sides or from
    // the previous window.
    TSet Fresh();

    // Records `value` in one parity's tally and answers how many distinct values that
    // parity holds now. Seeing a value for the second time is not an error here - it is
    // the ordinary case the scan levels out - so the answer is the running distinct
    // total, not a "was this one new" flag.
    int Count(TSet seen, int value);
}
