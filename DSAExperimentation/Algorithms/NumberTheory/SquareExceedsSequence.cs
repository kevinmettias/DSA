using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Algorithms.NumberTheory;

// The virtual monotone sequence IntegerSquareRoot.Floor binary-searches: index i reads Exceeds when
// i^2 is greater than the value, else 0, so the first index that reads Exceeds is one past the floor
// of the square root. Nothing is materialized - each Get is one multiply.
//
// It lives beside its one user rather than in DataStructures/Sequence with the views over stored
// data: "does i^2 exceed this value" fixes its content, and the length cap that keeps the multiply
// inside long is chosen by Floor - the same arrangement as SearchRange beside BinarySearch.
internal readonly struct SquareExceedsSequence(long value, int length) : IRandomAccessSequence<int>
{
    public const int Exceeds = 1;

    public int Length => length;

    public int Get(int index) => IsSquareAboveValue(index) ? Exceeds : 0;

    private bool IsSquareAboveValue(int index) => (long)index * index > value;
}
