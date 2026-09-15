namespace DSAExperimentation.LeetCode.Harness;

// Which case a strategy should be run against - RunCase's second position, and
// never a workload: a case is a correctness claim every strategy must satisfy,
// while a workload is a measurement some arms are not viable on. Distinct from
// StrategyName so the two lookups cannot be transposed silently.
internal readonly record struct CaseName(string Text);
