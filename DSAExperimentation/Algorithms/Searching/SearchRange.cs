namespace DSAExperimentation.Algorithms.Searching;

// Immutable by design: BinarySearch narrows toward the target with `range with { Low
// = ... }`/`range with { High = ... }` rather than assigning a field in place, so
// there is never a copy silently holding a stale bound.
/// <summary>The inclusive index range a binary search has left to probe, empty once Low passes High.</summary>
/// <param name="Low">The first index still in the range.</param>
/// <param name="High">The last index still in the range.</param>
public readonly record struct SearchRange(int Low, int High);
