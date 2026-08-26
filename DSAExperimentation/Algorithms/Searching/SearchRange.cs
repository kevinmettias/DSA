namespace DSAExperimentation.Algorithms.Searching;

// Immutable by design: BinarySearch narrows toward the target with `range with { Low
// = ... }`/`range with { High = ... }` rather than assigning a field in place, so
// there is never a copy silently holding a stale bound.
internal readonly record struct SearchRange(int Low, int High);
