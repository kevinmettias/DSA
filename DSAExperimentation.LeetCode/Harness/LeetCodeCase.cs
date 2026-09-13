namespace DSAExperimentation.LeetCode.Harness;

// One named example: an input every strategy must accept and the answer every
// strategy must produce for it. Named rather than positional because the name is
// what a failing theory prints - "two-sum/HashMap/no-pair-sums-to-target" says
// what broke, where "case 4" sends you counting entries in a list.
internal sealed record LeetCodeCase<TInput, TOutput>(string Name, TInput Input, TOutput Expected);
