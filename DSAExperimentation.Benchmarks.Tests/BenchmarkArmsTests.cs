using System.Reflection;
using DSAExperimentation.Benchmarks.Tests.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;

namespace DSAExperimentation.Benchmarks.Tests;

// The properties every benchmark class owes, checked once for all of them instead of once per class:
// rebuilding the workload from the same parameters reproduces it and every arm's answer, every arm
// answers what the baseline answers, since arms that disagree are timing different questions, and
// every arm answers the same when called again on the harness it already ran on, since that is how
// BenchmarkDotNet calls it.
// Every class with a [Benchmark] method is a case by construction, so a new benchmark is covered the
// moment it exists; ArmAgreement holds the only per-problem knowledge, which is when exact agreement
// is not the claim.
public sealed partial class BenchmarkArmsTests
{
    // How much of each side of the first difference a failure message shows.
    private const int ExcerptRadius = 40;

    public static TheoryData<string> BenchmarkClasses => CasesWhere(_ => true);

    public static TheoryData<string> ComparableBenchmarkClasses =>
        CasesWhere(type => !ArmAgreement.IncomparableAnswers.ContainsKey(type));

    public static TheoryData<string> IncomparableBenchmarkClasses =>
        CasesWhere(ArmAgreement.IncomparableAnswers.ContainsKey);

    [Theory]
    [MemberData(nameof(BenchmarkClasses))]
    public void Setup_SmallestParameters_RebuildsTheSameWorkload(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);

        Assert.Equal(
            AnswerGraphText.Of(benchmark.Prepare(benchmark.Baseline)),
            AnswerGraphText.Of(benchmark.Prepare(benchmark.Baseline)));
    }

    // A rebuilt workload is not enough on its own: an arm that also reads state the harness does not
    // hold - a static generator, a cache filled by an earlier call - answers differently anyway.
    [Theory]
    [MemberData(nameof(BenchmarkClasses))]
    public void Arms_RebuiltHarness_AnswerTheSameAgain(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);

        Assert.All(benchmark.Arms, arm => Assert.Equal(AnswerOf(benchmark, arm), AnswerOf(benchmark, arm)));
    }

    [Theory]
    [MemberData(nameof(ComparableBenchmarkClasses))]
    public void Arms_SmallestParameters_AnswerWhatTheBaselineAnswers(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);
        var expected = AnswerOf(benchmark, benchmark.Baseline);

        var disagreements = benchmark.Arms
            .Select(arm => DisagreementOf(benchmark, arm, expected))
            .OfType<string>()
            .ToList();

        Assert.True(disagreements.Count == 0, string.Join(Environment.NewLine, disagreements));
    }

    [Theory]
    [MemberData(nameof(IncomparableBenchmarkClasses))]
    public void Arms_AnswersThatDifferByDesign_EachRunToCompletion(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);

        Assert.All(benchmark.Arms, arm => Assert.Null(Record.Exception(() => AnswerOf(benchmark, arm))));
    }

    // Every check above hands each call a harness of its own, but BenchmarkDotNet calls an arm over and
    // over on one harness, re-running only its [IterationSetup] in between. An arm that consumes or
    // advances state no iteration setup rebuilds - a queue it drains, a cursor it moves - answers
    // differently from its second call on, and from then on it is timing a different workload. Only
    // the first two calls are compared, so state that changes the answer only on a later call, like
    // ApplyDiscountEveryNOrders' cashier discounting every third customer, goes unseen.
    [Theory]
    [MemberData(nameof(BenchmarkClasses))]
    public void Arms_SameHarnessCalledAgain_AnswerTheSameAgain(string benchmarkName)
    {
        var benchmark = BenchmarkClass.Named(benchmarkName);

        var drifts = benchmark.Arms
            .Select(arm => DriftOf(benchmark, arm))
            .OfType<string>()
            .ToList();

        Assert.True(drifts.Count == 0, string.Join(Environment.NewLine, drifts));
    }

    private static TheoryData<string> CasesWhere(Func<Type, bool> includes) =>
        new(BenchmarkClass.All
            .Where(benchmark => includes(benchmark.Type))
            .Select(benchmark => benchmark.Name));

    private static string AnswerOf(BenchmarkClass benchmark, MethodInfo arm) =>
        TextOf(benchmark, arm, benchmark.Run(benchmark.Prepare(arm), arm));

    // Rendered the moment it is returned: an answer may be state the harness goes on to rewrite. A generated
    // catalog class's buffer is read as LeetCode reads its answers (CatalogArmAnswers).
    private static string TextOf(BenchmarkClass benchmark, MethodInfo arm, object? answer)
    {
        if (CatalogArmAnswers.IsCatalogClass(benchmark.Type))
        {
            return CatalogArmAnswers.AgreementTextOf(benchmark.Type, arm, answer);
        }

        return ArmAgreement.UnorderedAnswers.Contains(benchmark.Type)
            ? AnswerGraphText.OfUnordered(answer)
            : AnswerGraphText.Of(answer);
    }

    private static string? DisagreementOf(BenchmarkClass benchmark, MethodInfo arm, string expected)
    {
        var actual = arm == benchmark.Baseline ? expected : AnswerOf(benchmark, arm);

        if (BenchmarkClass.IsAnsweredByHarness(arm) && AnswerGraphText.Of(benchmark.Prepare(arm)) == actual)
        {
            return $"unobservable: {arm.Name} returns void and leaves the harness exactly as setup built it.";
        }

        var at = FirstDifference(expected, actual);

        return at < 0
            ? null
            : $"disagrees: {arm.Name} answered ...{Excerpt(actual, at)}... where {benchmark.Baseline.Name} answered ...{Excerpt(expected, at)}... (character {at})";
    }

    private static int FirstDifference(string expected, string actual)
    {
        var shorter = Math.Min(expected.Length, actual.Length);

        for (var i = 0; i < shorter; i++)
        {
            if (expected[i] != actual[i])
            {
                return i;
            }
        }

        return expected.Length == actual.Length ? -1 : shorter;
    }

    private static string Excerpt(string text, int at)
    {
        var start = Math.Max(0, at - ExcerptRadius);

        return text[start..Math.Min(text.Length, at + ExcerptRadius)];
    }

    private static string? DriftOf(BenchmarkClass benchmark, MethodInfo arm)
    {
        var harness = benchmark.Prepare(arm);
        var first = TextOf(benchmark, arm, benchmark.Run(harness, arm));

        benchmark.PrepareNextCall(harness, arm);
        var again = TextOf(benchmark, arm, benchmark.Run(harness, arm));
        var at = FirstDifference(first, again);

        return at < 0
            ? null
            : $"drifts: {arm.Name} answered ...{Excerpt(again, at)}... on its second call where its first answered ...{Excerpt(first, at)}... (character {at})";
    }
}
