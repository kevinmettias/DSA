using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Algorithms.NumberTheory;

// The monotone rule IntegerSquareRoot.Floor bisects: a candidate holds when its square is greater than
// the value, so the first candidate that holds is one past the floor of the square root. Nothing is
// materialized - each IsSatisfiedBy is one multiply, in long so a candidate up to Floor's cap cannot
// overflow.
//
// It lives beside its one user rather than in Algorithms/Searching with the search it is handed to:
// "does i^2 exceed this value" fixes its content, and the cap that keeps the multiply inside long is
// chosen by Floor - the same arrangement as SearchRange beside BinarySearch.
internal readonly struct SquareExceeds(long value) : IMonotonePredicate<int>
{
    public bool IsSatisfiedBy(int candidate) => (long)candidate * candidate > value;
}
