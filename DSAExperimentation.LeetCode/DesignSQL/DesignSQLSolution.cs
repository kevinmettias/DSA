using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.DesignSQL;

// LeetCode 2408. Design SQL: a fixed set of named tables, each insertRow appending a
// row under an ever-increasing id that deleteRow never reuses, and selectCell
// reading one cell of one row by (table name, row id, column id).
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
    // LeetCode guarantees selectCell only ever names a live row, so what a missing
    // one reports is unspecified by the problem. Both strategies agree on the empty
    // string so a harness can hold them to one observable behaviour rather than to
    // one arm's scan returning "" while the other's keyed lookup dereferences null.
    private const string NoCell = "";

    // The shared surface both strategies implement, so a harness can replay one
    // call script against either without restating it.
    internal interface ISqlStrategy
    {
        void InsertRow(string name, string[] row);

        void DeleteRow(string name, int rowId);

        string SelectCell(string name, int rowId, int columnId);
    }

    // The textbook answer: each table is a plain List of (id, row) pairs appended in
    // insertion order, and both deleteRow and selectCell linear-scan it for the
    // matching id - the arm the row-id-keyed strategy below has to justify itself
    // against. Deliberately without this repo's primitives (section 17.5).
    internal sealed class SqlByListScan : ISqlStrategy
    {
        private readonly Dictionary<string, ScanTable> _tables = new();

        // LeetCode's constructor also states each table's column count. The rows
        // handed to insertRow already carry their own width, so the parameter is
        // mirrored (section 17.9: the solution tier keeps LeetCode's own signature)
        // without being stored by either strategy.
        public SqlByListScan(string[] names, int[] columns)
        {
            foreach (var name in names)
            {
                _tables[name] = new ScanTable();
            }
        }

        public void InsertRow(string name, string[] row) => _tables[name].Insert(row);

        public void DeleteRow(string name, int rowId) => _tables[name].Delete(rowId);

        public string SelectCell(string name, int rowId, int columnId) => _tables[name].Select(rowId, columnId);

        private sealed class ScanTable
        {
            private readonly List<(int Id, string[] Row)> _rows = [];
            private int _nextRowId = 1;

            public void Insert(string[] row) => _rows.Add((_nextRowId++, row));

            public void Delete(int rowId)
            {
                for (var i = 0; i < _rows.Count; i++)
                {
                    if (_rows[i].Id == rowId)
                    {
                        _rows.RemoveAt(i);
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
                        return row[columnId - 1];
                    }
                }

                return NoCell;
            }
        }
    }

    // This repo's own primitives: a HashMap<string, Table> for the table-name
    // lookup, and inside each table a HashMap<int, string[]> keyed directly by row
    // id - the same two-level HashMap-of-HashMap composition
    // DesignMovieRentalSystem's HashMap<int, BinarySearchTree<...>> rehearses.
    // insertRow/deleteRow/selectCell are then Set/TryRemove/TryGetValue over those
    // two maps, so every operation is a hash lookup rather than a scan, and the
    // ever-increasing id counter alone gives LeetCode's "a deleted id is never
    // reused" contract.
    internal sealed class SqlByHashMapTables : ISqlStrategy
    {
        private readonly HashMap<string, Table> _tables = new();

        public SqlByHashMapTables(string[] names, int[] columns)
        {
            foreach (var name in names)
            {
                _tables.Set(name, new Table());
            }
        }

        public void InsertRow(string name, string[] row) => TableFor(name).Insert(row);

        public void DeleteRow(string name, int rowId) => TableFor(name).Delete(rowId);

        public string SelectCell(string name, int rowId, int columnId) => TableFor(name).Select(rowId, columnId);

        private Table TableFor(string name)
        {
            _tables.TryGetValue(name, out var table);
            return table;
        }

        private sealed class Table
        {
            private readonly HashMap<int, string[]> _rows = new();
            private int _nextRowId = 1;

            public void Insert(string[] row) => _rows.Set(_nextRowId++, row);

            public void Delete(int rowId) => _rows.TryRemove(rowId);

            public string Select(int rowId, int columnId)
                => _rows.TryGetValue(rowId, out var row) ? CellAt(row, columnId) : NoCell;

            // Column ids are 1-based; the stored row is a 0-based array.
            private static string CellAt(string[] row, int columnId) => row[columnId - 1];
        }
    }
}
