namespace DSAExperimentation.Benchmarks.Fixtures;

// The canonical key of an undirected edge: the two endpoints written low first, so
// a later (v, u) for the same edge hashes to the same key. Both endpoints are ints,
// so the member names are the only thing that says which one is which.
internal readonly record struct UndirectedEdge(int Low, int High);
