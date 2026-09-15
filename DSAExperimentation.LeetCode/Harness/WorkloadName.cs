namespace DSAExperimentation.LeetCode.Harness;

// Which workload a strategy should be timed over - BindWorkload's second position.
// A distinct type from CaseName as well as from StrategyName because the two are
// deliberately not the same lookup, so neither can be passed where the other
// belongs with the compiler none the wiser.
internal readonly record struct WorkloadName(string Text);
