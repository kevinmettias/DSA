using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage;

// THE test harness. Not "a test for the harness" - this one theory is what proves
// every registered strategy of every registered problem against every one of that
// problem's cases, and it is meant to be the only per-case test in this project
// once the per-problem test classes have been converted to registrations.
//
// The arm is passed as three strings rather than as a LeetCodeArm because xUnit's
// TheoryData is public and LeetCodeArm is internal (CS0053), the same constraint
// the per-problem tests already work around by carrying LeetCode's own array
// shapes. The strings are the arm's own identity, so a failure still reads
// "two-sum/HashMap/no-pair-sums-to-target".
public sealed class LeetCodeProblemTests
{
    public static TheoryData<string, string, string> CaseArms
    {
        get
        {
            var data = new TheoryData<string, string, string>();

            foreach (var arm in LeetCodeProblemRegistry.CaseArms())
            {
                data.Add(arm.TitleSlug, arm.StrategyName, arm.EntryName);
            }

            return data;
        }
    }

    [Theory]
    [MemberData(nameof(CaseArms))]
    public void Strategy_OnRegisteredCase_ProducesExpectedAnswer(
        string titleSlug, string strategyName, string caseName)
    {
        var outcome = LeetCodeProblemRegistry.Get(titleSlug).RunCase(strategyName, caseName);

        Assert.True(outcome.Matched, $"{titleSlug}/{strategyName}/{caseName}: {outcome.FailureReason}");
    }

    // A registry that discovers nothing still produces a green run - every theory
    // above simply never executes - so the count itself has to be asserted, or a
    // broken reflection scan would look exactly like success.
    [Fact]
    public void Registry_AfterDiscovery_FindsRegisteredProblems()
    {
        Assert.NotEmpty(LeetCodeProblemRegistry.All);
        Assert.NotEmpty(LeetCodeProblemRegistry.CaseArms());
    }

    [Fact]
    public void EveryProblem_Registration_UsesItsLeetCodeTitleSlug()
    {
        var malformed = LeetCodeProblemRegistry.All
            .Select(problem => problem.TitleSlug)
            .Where(slug => slug != slug.ToLowerInvariant() || slug.Contains(' ') || slug.Length == 0)
            .ToList();

        Assert.Empty(malformed);
    }

    // A workload that names a strategy nobody registered would silently measure
    // nothing rather than failing, so the pairing is checked here instead.
    [Fact]
    public void EveryWorkloadArm_NamesAStrategyItsProblemRegistered()
    {
        foreach (var problem in LeetCodeProblemRegistry.All)
        {
            Assert.All(problem.WorkloadArms, arm => Assert.Contains(arm.StrategyName, problem.StrategyNames));
        }
    }

    // The benchmark harness has no test of its own - it only runs under
    // BenchmarkDotNet - so the arm list it enumerates is pinned here. This is also
    // the regression guard for the restriction itself: if Workload's strategy
    // filter ever stopped being applied, the exponential arm would rejoin this
    // list and the benchmark run would stop terminating.
    [Fact]
    public void WorkloadArms_ForAnExponentialBaseline_MeasureOnlyTheViableStrategy()
    {
        var problem = LeetCodeProblemRegistry.Get("path-with-maximum-probability");

        Assert.Equal(["ExhaustiveDfs", "Dijkstra"], problem.StrategyNames);
        Assert.Equal(["Dijkstra"], problem.WorkloadArms.Select(arm => arm.StrategyName));
    }
}
