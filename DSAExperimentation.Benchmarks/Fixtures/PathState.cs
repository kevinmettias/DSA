namespace DSAExperimentation.Benchmarks.Fixtures;

// Carries each node's own label alongside its fold result so the parent's Combine
// can tell, per child, whether the connecting edge is usable (labels differ).
// Mirrors the Tests project's own PathState exactly.
internal readonly record struct PathState(int Height, int LongestPath, char Label);
