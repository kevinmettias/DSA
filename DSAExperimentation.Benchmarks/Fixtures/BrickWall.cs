namespace DSAExperimentation.Benchmarks.Fixtures;

// One seeded LC 803 workload: the 0/1 wall and the [row, col] hit list drawn
// against it, returned together because a hit list only means anything paired
// with the exact grid whose brick positions it was sampled from.
internal readonly record struct BrickWall(int[][] Grid, int[][] Hits);
