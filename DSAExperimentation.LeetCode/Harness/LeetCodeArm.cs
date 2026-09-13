namespace DSAExperimentation.LeetCode.Harness;

// One runnable combination: which problem, which of its strategies, and which of
// its cases or workloads. ToString is the identity both harnesses display -
// "two-sum/HashMap/no-pair-sums-to-target" as a failing theory's name, and the
// same text as a BenchmarkDotNet parameter - so one string locates a result in
// either tool.
internal readonly record struct LeetCodeArm(string TitleSlug, string StrategyName, string EntryName)
{
    public override string ToString() => $"{TitleSlug}/{StrategyName}/{EntryName}";
}
