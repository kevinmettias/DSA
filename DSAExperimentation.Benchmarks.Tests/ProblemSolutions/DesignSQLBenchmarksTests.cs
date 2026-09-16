using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DesignSQLBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a table linear-scanned for a row id against a table keyed by
// one - so a harness whose arms disagree is timing two different problems. Setup builds the rows
// and the ids to delete from RowCount alone, so the same RowCount must rebuild the same workload,
// and that workload has to read back a cell from every row it inserted.
public sealed partial class DesignSQLBenchmarksTests
{
    private const int SmallestRowCount = 500;

    [Fact]
    public void Setup_SameRowCount_RebuildsTheSameInsertAndDeleteWorkload()
    {
        // The replay inserts a row per index, then reads column 1 of every row id in insertion
        // order, then deletes the even ones. Each inserted row is "r{index}c{column}", so the sum
        // of the read cells' lengths is a closed form over RowCount that only a table handing back
        // the row it was given can produce.
        Assert.Equal(ExpectedReadLength(SmallestRowCount), BuildHarness().ListScanTable());
        Assert.Equal(BuildHarness().ListScanTable(), BuildHarness().ListScanTable());
    }

    [Fact]
    public void ListScanTable_InsertReadDeleteScript_AgreesWithHashMapTable()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapTable(), harness.ListScanTable());
    }

    [Fact]
    public void HashMapTable_InsertReadDeleteScript_AgreesWithListScanTable()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListScanTable(), harness.HashMapTable());
    }

    private static DesignSQLBenchmarks BuildHarness()
    {
        var harness = new DesignSQLBenchmarks { RowCount = SmallestRowCount };
        harness.Setup();

        return harness;
    }

    // Row id n was inserted as "r{n - 1}c1", which is what the replay reads back for every n from
    // 1 through RowCount.
    private static long ExpectedReadLength(int rowCount) =>
        Enumerable.Range(0, rowCount).Sum(index => (long)$"r{index}c1".Length);
}
