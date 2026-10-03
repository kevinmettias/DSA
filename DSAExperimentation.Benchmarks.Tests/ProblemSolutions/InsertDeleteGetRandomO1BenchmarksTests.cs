using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for InsertDeleteGetRandomO1Benchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot
// pin: that the script drains the set completely, which follows from Setup's construction rather than from either
// arm. Each arm replays the same fixed script against an instance it builds inside the call and returns every
// Insert verdict, then every Remove verdict. The script inserts the distinct values 0..Count-1 and then removes a
// permutation of them, so every insert finds the value absent and every removal finds it present: all 2 * Count
// verdicts are true and nothing is left behind. A harness that skipped its removals, or removed values it never
// inserted, would answer false somewhere.
public sealed partial class InsertDeleteGetRandomO1BenchmarksTests
{
    private const int SmallestCount = 200;

    // One Insert and one Remove per value.
    private const int VerdictsPerValue = 2;

    [Fact]
    public void ListScan_InsertThenRemoveEveryValue_DrainsTheSetCompletely() =>
        AssertDrainsTheSetCompletely(BuildHarness().ListScan());

    [Fact]
    public void HashMapSwapRemove_InsertThenRemoveEveryValue_DrainsTheSetCompletely() =>
        AssertDrainsTheSetCompletely(BuildHarness().HashMapSwapRemove());

    private static void AssertDrainsTheSetCompletely(bool[] verdicts)
    {
        Assert.Equal(VerdictsPerValue * SmallestCount, verdicts.Length);
        Assert.DoesNotContain(false, verdicts);
    }

    private static InsertDeleteGetRandomO1Benchmarks BuildHarness()
    {
        var harness = new InsertDeleteGetRandomO1Benchmarks { Count = SmallestCount };
        harness.Setup();

        return harness;
    }
}
