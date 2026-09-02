namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestPathWithDifferentAdjacentCharacters.Fixtures;

// Carries each node's own label alongside its fold result so the parent's Combine
// can tell, per child, whether the connecting edge is usable (labels differ) -
// something DiameterAlgebra's plain HeightDiameterState never needed, since binary
// tree diameter has no per-edge validity condition to check. Height/LongestPath are
// counted in nodes, not edges, matching LeetCode 2246's return value directly.
internal readonly record struct PathState(int Height, int LongestPath, char Label);
