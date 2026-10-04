using System.Globalization;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignSQL;

// LeetCode 2408. Design SQL: a fixed set of named tables, each with a fixed column
// count. ins appends a row under the table's next id and answers whether it did - a
// row of the wrong width, or a table that does not exist, is refused and inserts
// nothing, so it does not use up an id; rmv drops a row and never frees its id for
// reuse; sel reads one cell by (row id, column id), both counted from 1, and answers
// "<null>" for a table, row or column that is not there; exp lists the live rows in
// id order, each as its id and its cells joined by commas, or nothing for a table that
// does not exist.
//
// An instance API rather than a pure function, so "every strategy for the problem"
// (section 17.3) takes the form of two classes implementing the shared ISqlStrategy
// surface below rather than two static methods sharing an <Operation>By<Strategy>
// name - the same shape DesignANumberContainerSystemSolution uses, and for the same
// reason: a Design problem is a sequence of mutating calls against one instance, so
// there is no prepared input to hoist into a benchmark's [GlobalSetup]. Each
// [Benchmark] arm constructs its own instance from LeetCode's own constructor
// arguments and replays the same call script instead.
internal static class DesignSQLSolution
{
    // What sel answers for a table, row or column that does not exist - LC 2408's own
    // spelling of "no such cell".
    private const string NoCell = "<null>";

    // What separates the id and the cells of one exported row.
    private const string CellSeparator = ",";

    // The id of a table's first row; each later one is one greater than the last
    // inserted, removed or not. Column ids count from 1 the same way.
    private const int FirstId = 1;

    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it. Each method is one of
    // LeetCode's four calls, named for the operation the statement says it performs:
    // TryInsert is ins, Remove is rmv, Select is sel and Export is exp.
    internal interface ISqlStrategy
    {
        bool TryInsert(string name, string[] row);

        void Remove(string name, int rowId);

        string Select(string name, int rowId, int columnId);

        string[] Export(string name);
    }

    // The cell at columnId, counted from 1, or "<null>" for a column the row does not
    // have. Every stored row has exactly its table's column count, so the row's own
    // width is the bound.
    private static string CellOf(string[] row, int columnId)
    {
        if (columnId < FirstId || columnId > row.Length)
        {
            return NoCell;
        }

        return row[columnId - FirstId];
    }

    // One exported row: the row's id, then each of its cells, joined by commas.
    private static string ExportLine(int rowId, string[] row)
    {
        var cells = string.Join(CellSeparator, row);

        return rowId.ToString(CultureInfo.InvariantCulture) + CellSeparator + cells;
    }

    // The textbook answer: each table is a plain List of (id, row) pairs appended in
    // insertion order - which is id order, since ids only grow - so exp walks it as
    // it stands, while rmv and sel linear-scan it for the matching id. The arm the
    // row-id-keyed strategy below has to justify itself against. Deliberately
    // without this repo's primitives (section 17.5).
    internal sealed class SqlByListScan : ISqlStrategy
    {
        private readonly Dictionary<string, ScanTable> _tables = new();

        public SqlByListScan(string[] names, int[] columns)
        {
            for (var table = 0; table < names.Length; table++)
            {
                _tables[names[table]] = new ScanTable(columns[table]);
            }
        }

        public bool TryInsert(string name, string[] row) =>
            _tables.TryGetValue(name, out var table) && table.TryInsert(row);

        public void Remove(string name, int rowId)
        {
            if (_tables.TryGetValue(name, out var table))
            {
                table.Remove(rowId);
            }
        }

        public string Select(string name, int rowId, int columnId)
        {
            if (!_tables.TryGetValue(name, out var table))
            {
                return NoCell;
            }

            return table.Select(rowId, columnId);
        }

        public string[] Export(string name)
        {
            if (!_tables.TryGetValue(name, out var table))
            {
                return [];
            }

            return table.Export();
        }

        private sealed class ScanTable(int columnCount)
        {
            private readonly List<(int Id, string[] Row)> _rows = [];
            private int _lastRowId;

            public bool TryInsert(string[] row)
            {
                if (row.Length != columnCount)
                {
                    return false;
                }

                _lastRowId++;
                _rows.Add((_lastRowId, row));

                return true;
            }

            public void Remove(int rowId)
            {
                for (var position = 0; position < _rows.Count; position++)
                {
                    if (_rows[position].Id == rowId)
                    {
                        _rows.RemoveAt(position);
                        return;
                    }
                }
            }

            public string Select(int rowId, int columnId)
            {
                foreach (var (id, row) in _rows)
                {
                    if (id == rowId)
                    {
                        return CellOf(row, columnId);
                    }
                }

                return NoCell;
            }

            public string[] Export()
            {
                var lines = new string[_rows.Count];

                for (var position = 0; position < _rows.Count; position++)
                {
                    var (id, row) = _rows[position];
                    lines[position] = ExportLine(id, row);
                }

                return lines;
            }
        }
    }

    // This repo's own primitives: a HashMap<string, Table> for the table-name
    // lookup, and inside each table a HashMap<int, string[]> keyed directly by row
    // id - the same two-level HashMap-of-HashMap composition
    // DesignMovieRentalSystem's HashMap<int, BinarySearchTree<...>> rehearses. ins,
    // rmv and sel are then Set, TryRemove and TryGetValue over those two maps, so each
    // is a hash lookup rather than a scan, and the id counter, advanced only by an
    // insert that succeeds, alone gives LeetCode's "a removed id is never reused".
    //
    // The map holds only live rows, so a table thinned by removals - the problem's
    // follow-up - costs memory for what is left, not for every id ever issued. What
    // it gives up is order: a HashMap enumerates by bucket, so exp walks the ids
    // issued so far and keeps the ones still present, O(ids issued) per export,
    // where the list scan walks only the live rows.
    internal sealed class SqlByHashMapTables : ISqlStrategy
    {
        private readonly HashMap<string, Table> _tables = new();

        public SqlByHashMapTables(string[] names, int[] columns)
        {
            for (var table = 0; table < names.Length; table++)
            {
                _tables.Set(names[table], new Table(columns[table]));
            }
        }

        public bool TryInsert(string name, string[] row) =>
            _tables.TryGetValue(name, out var table) && table.TryInsert(row);

        public void Remove(string name, int rowId)
        {
            if (_tables.TryGetValue(name, out var table))
            {
                table.Remove(rowId);
            }
        }

        public string Select(string name, int rowId, int columnId)
        {
            if (!_tables.TryGetValue(name, out var table))
            {
                return NoCell;
            }

            return table.Select(rowId, columnId);
        }

        public string[] Export(string name)
        {
            if (!_tables.TryGetValue(name, out var table))
            {
                return [];
            }

            return table.Export();
        }

        private sealed class Table(int columnCount)
        {
            private readonly HashMap<int, string[]> _rows = new();
            private int _lastRowId;

            public bool TryInsert(string[] row)
            {
                if (row.Length != columnCount)
                {
                    return false;
                }

                _lastRowId++;
                _rows.Set(_lastRowId, row);

                return true;
            }

            public void Remove(int rowId) => _rows.TryRemove(rowId);

            public string Select(int rowId, int columnId)
            {
                if (!_rows.TryGetValue(rowId, out var row))
                {
                    return NoCell;
                }

                return CellOf(row, columnId);
            }

            public string[] Export()
            {
                var lines = new string[_rows.Count];
                var next = 0;

                for (var rowId = FirstId; rowId <= _lastRowId; rowId++)
                {
                    if (_rows.TryGetValue(rowId, out var row))
                    {
                        lines[next] = ExportLine(rowId, row);
                        next++;
                    }
                }

                return lines;
            }
        }
    }
}
