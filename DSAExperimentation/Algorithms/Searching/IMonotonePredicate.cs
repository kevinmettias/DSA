namespace DSAExperimentation.Algorithms.Searching;

// The rule MonotonePredicateSearch bisects: whether a candidate value is feasible. It is consulted at
// every probe and decides which half survives, but like the successor relation of §12.1 its space is
// open - Koko's hour count, a greedy station plan, an inclusion-exclusion count - so no witness this
// library enumerates could close over it (§5, row 2). §12.4's question then decides how it is passed:
// the arrays, budgets and plans a rule reads never arrive through Holds(candidate), so Holds is an
// instance member on a struct type parameter, as IGridCellFilter.CanEnter is. Each rule is its own
// instantiation and Holds a direct, inlinable call - no delegate, no interface dispatch.
//
// Monotonicity is a precondition law, stated and never checked, like BinarySearch's sortedness (§9.1):
// FirstTrue assumes false...false true...true over the range and LastTrue the mirror image. A rule
// that breaks it still returns some boundary, just not a meaningful one. Holds is only ever asked about
// a candidate inside the range the caller passed, so a rule may index its own data unguarded. The
// search copies the rule by value, so a rule reads; one that recorded what it was asked would record
// into its own copy.
internal interface IMonotonePredicate<Integer>
{
    bool Holds(Integer candidate);
}
