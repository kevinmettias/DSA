using DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

namespace DSAExperimentation.LeetCode.Catalog.Sync;

// A `dataset` import's totals: questions covered, cases kept, and cases dropped by reason, largest
// first, so the report says what the import lost and why as plainly as what it kept.
internal sealed class DatasetTally
{
    private readonly Dictionary<DatasetDropReason, int> _dropped = [];
    private int _questions;
    private int _kept;

    public void Add(DatasetConversion conversion)
    {
        if (conversion.Cases.Count > 0)
        {
            _questions++;
        }

        _kept += conversion.Cases.Count;

        foreach (var (reason, count) in conversion.Dropped)
        {
            _dropped[reason] = _dropped.GetValueOrDefault(reason) + count;
        }
    }

    public void Report(TextWriter output)
    {
        output.WriteLine($"{_kept} cases kept for {_questions} questions.");

        foreach (var (reason, count) in _dropped.OrderByDescending(entry => entry.Value))
        {
            output.WriteLine($"  dropped {count}: {reason}");
        }
    }
}
