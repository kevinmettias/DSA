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
// "two-sum/HashMap/no-pair-sums-to-target" - and they travel as one ArmIdentity
// row rather than as three interchangeable string positions, so a row states which
// string is the slug, which the strategy and which the case.
public sealed partial class LeetCodeProblemTests
{
    public static TheoryData<ArmIdentity> CaseArms
    {
        get
        {
            var caseArms = new TheoryData<ArmIdentity>();

            foreach (var arm in LeetCodeProblemRegistry.CaseArms())
            {
                caseArms.Add(new ArmIdentity(
                    TitleSlug: arm.TitleSlug, StrategyName: arm.StrategyName, CaseName: arm.EntryName));
            }

            return caseArms;
        }
    }

    [Theory]
    [MemberData(nameof(CaseArms))]
    public void Strategy_OnRegisteredCase_ProducesExpectedAnswer(ArmIdentity arm)
    {
        var outcome = LeetCodeProblemRegistry.Get(arm.TitleSlug).RunCase(
            new StrategyName(arm.StrategyName), new CaseName(arm.CaseName));

        Assert.True(outcome.Matched, $"{arm.TitleSlug}/{arm.StrategyName}/{arm.CaseName}: {outcome.FailureReason}");
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

        // The large workload is the restricted one - at 400 vertices the
        // exhaustive arm would not finish.
        var largeWorkloadArms = ArmsFor(problem, "cycle-400");

        Assert.Equal(["Dijkstra"], largeWorkloadArms);

        // The small ones deliberately are NOT restricted: a benchmark that never
        // ran the baseline anywhere would have no comparison left to make, which
        // is the failure the filter must not be allowed to cause.
        var smallWorkloadArms = ArmsFor(problem, "branching-14");

        Assert.Equal(["ExhaustiveDfs", "Dijkstra"], smallWorkloadArms);
    }

    private static IEnumerable<string> ArmsFor(LeetCodeProblem problem, string workloadName)
        => problem.WorkloadArms
            .Where(arm => arm.EntryName == workloadName)
            .Select(arm => arm.StrategyName);

    // One registered arm: the problem it belongs to, the strategy it names and the case
    // it runs. All three are strings and only their order would say which is which, so a
    // row names each position rather than leaving three interchangeable ones. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct ArmIdentity(string TitleSlug, string StrategyName, string CaseName);
}
