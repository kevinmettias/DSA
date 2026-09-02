using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignSpreadsheet.DesignSpreadsheetSolution;

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

    [Params(200, 2_000)]
    public int CellCount;

    private List<Func<ISpreadsheetStrategy, int?>> _script = null!;

    [GlobalSetup]
    public void Setup() => _script = BuildScript(CellCount, new Random(Seed));

    [Benchmark(Baseline = true)]
    public long Dictionary() => Replay(new SpreadsheetByDictionary(CellCount));

    [Benchmark]
    public long HashMap() => Replay(new SpreadsheetByHashMap(CellCount));

    // Sums every GetValue result rather than discarding it, so the JIT can't
    // eliminate the replay as dead code - the same "return the real answer, not a
    // weaker proxy" shape DesignTaskManagerBenchmarks already follows.
    private long Replay(ISpreadsheetStrategy strategy)
    {
        var sum = 0L;

        foreach (var op in _script)
        {
            sum += op(strategy) ?? 0;
        }

        return sum;
    }

    private static List<Func<ISpreadsheetStrategy, int?>> BuildScript(int cellCount, Random random)
    {
        var script = new List<Func<ISpreadsheetStrategy, int?>>();

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

        for (var i = 0; i < cellCount; i++)
        {
            var formula = RandomFormula(cellCount, random);
            script.Add(strategy => strategy.GetValue(formula));
        }

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

        return script;
    }

    private static string RandomCell(int cellCount, Random random)
    {
        var column = (char)('A' + random.Next(0, ColumnCount));
        var row = random.Next(1, cellCount + 1);
        return $"{column}{row}";
    }

    private static string RandomFormula(int cellCount, Random random)
        => random.Next(0, 2) == 0
            ? $"={RandomCell(cellCount, random)}+{RandomCell(cellCount, random)}"
            : $"={RandomCell(cellCount, random)}+{random.Next(0, ValueUpperBound)}";
}
