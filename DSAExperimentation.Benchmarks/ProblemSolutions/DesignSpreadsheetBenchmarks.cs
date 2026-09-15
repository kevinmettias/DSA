using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignSpreadsheet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignSpreadsheetSolution's, the same classes
// DesignSpreadsheetTests proves correct. [GlobalSetup] builds one fixed, valid call
// script - SetCell across a spread of cell references, interspersed GetValue calls
// against both formula shapes ("=cell+cell" and "=cell+literal") and a handful of
// ResetCell calls - so script construction, including which cell references get
// used, is charged to setup rather than to the replay each [Benchmark] arm
// measures.
[MemoryDiagnoser]
public class DesignSpreadsheetBenchmarks
{
    private const int Seed = 3484;
    private const int ValueUpperBound = 100_000;
    private const int ColumnCount = 26;

    private List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>> _script = new();

    [Params(200, 2_000)]
    public int CellCount { get; set; }

    [GlobalSetup]
    public void Setup() => _script = BuildScript(CellCount, new Random(Seed));

    private static List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>> BuildScript(int cellCount, Random random)
    {
        var script = new List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>>();

        AppendSetCellCalls(script, cellCount, random);
        AppendGetValueCalls(script, cellCount, random);
        AppendResetCellCalls(script, cellCount, random);

        return script;
    }

    // SetCell across a spread of cell references, each call closing over its own
    // freshly drawn cell and value.
    private static void AppendSetCellCalls(
        List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>> script, int cellCount, Random random)
    {
        for (var i = 0; i < cellCount; i++)
        {
            var cell = RandomCell(cellCount, random);
            var value = random.Next(0, ValueUpperBound);
            script.Add(strategy =>
            {
                strategy.SetCell(cell, value);
                return null;
            });
        }
    }

    // GetValue against both formula shapes, so the replay exercises the two parsing
    // paths rather than only one.
    private static void AppendGetValueCalls(
        List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>> script, int cellCount, Random random)
    {
        for (var i = 0; i < cellCount; i++)
        {
            var formula = RandomFormula(cellCount, random);
            script.Add(strategy => strategy.GetValue(formula));
        }
    }

    private static string RandomFormula(int cellCount, Random random)
    {
        var picksTwoCells = random.Next(0, 2) == 0;

        return picksTwoCells
            ? $"={RandomCell(cellCount, random)}+{RandomCell(cellCount, random)}"
            : $"={RandomCell(cellCount, random)}+{random.Next(0, ValueUpperBound)}";
    }

    // A tenth of the cells reset, interleaved after the reads.
    private static void AppendResetCellCalls(
        List<Func<DesignSpreadsheetSolution.ISpreadsheetStrategy, int?>> script, int cellCount, Random random)
    {
        var resetCount = cellCount / 10;

        for (var i = 0; i < resetCount; i++)
        {
            var cell = RandomCell(cellCount, random);
            script.Add(strategy =>
            {
                strategy.ResetCell(cell);
                return null;
            });
        }
    }

    [Benchmark(Baseline = true)]
    public long Dictionary() => Replay(new DesignSpreadsheetSolution.SpreadsheetByDictionary(CellCount));

    [Benchmark]
    public long HashMap() => Replay(new DesignSpreadsheetSolution.SpreadsheetByHashMap(CellCount));

    // Sums every GetValue result rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape DesignTaskManagerBenchmarks already follows.
    private long Replay(DesignSpreadsheetSolution.ISpreadsheetStrategy strategy)
    {
        var sum = 0L;

        foreach (var op in _script)
        {
            sum += op(strategy) ?? 0;
        }

        return sum;
    }

    private static string RandomCell(int cellCount, Random random)
    {
        var column = (char)('A' + random.Next(0, ColumnCount));
        var row = random.Next(1, cellCount + 1);
        return $"{column}{row}";
    }
}
