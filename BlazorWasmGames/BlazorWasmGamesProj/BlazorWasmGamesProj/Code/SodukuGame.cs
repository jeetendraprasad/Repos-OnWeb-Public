
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace BlazorWasmGamesProj.Code
{


    public class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        private List<string> _suBlockFullFlattened = new();
        private List<string> _suHoriFullFlattened = new();
        private List<string> _suVertFullFlattened = new();
        private static List<string> _hints = new();
        public static List<string> Hints { get { return _hints; } }

        //private Dictionary<string, int> moves = new();
        private List<SodukuGameMove> moves = new();

        public List<SodukuGameMove> Moves
        {
            get
            {
                return moves;
            }
        }
        public Dictionary<string, string> Positions
        {
            get
            {
                Dictionary<string, string> posns = SuHoriFullFlattened.ToDictionary(item => item, item => "0");
                foreach (SodukuGameMove item in moves)
                {
                    posns[item.CellId] = item.GetCellValue();
                }

                return posns;
            }
        }

        public SodukuGameState GameState { get; private set; } = SodukuGameState.CalculationPending;

        public SodukuGame(int rowsBlock, int colsBlock)
        {
            ReInit(rowsBlock, colsBlock);
        }

        public void ReInit(int rowsBlock, int colsBlock)
        {
            _rowsBlock = rowsBlock;
            _colsBlock = colsBlock;

            GameState = SodukuGameState.CalculationPending;
        }

        public void Recalculate()
        {
            _suBlockFullFlattened = SuBlockFull.Flatten();
            _suHoriFullFlattened = SuHoriFull.Flatten();
            _suVertFullFlattened = SuVertFull.Flatten();

            //moves = new Dictionary<string, int>(_rowsBlock * _colsBlock);
            //moves = SuHoriFullFlattened.ToDictionary( item => item, item => 0);

            _hints = Enumerable.Range(1, _rowsBlock * _colsBlock).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();

            GameState = SodukuGameState.CalculationDone_Or_EditModeBegin;
        }

        public bool AddMove(string cellId, string cellValue)
        {
            moves.Add(new SodukuGameMove(cellId, cellValue, _rowsBlock * _colsBlock));
            return true;
        }

        void Solve()
        {
            foreach (KeyValuePair<string, string> elem in Positions)
            {
                Console.WriteLine("{0} and {1}", elem.Key, elem.Value);
            }

            Console.WriteLine("Moves : " + string.Join(',', Moves));

            CheckHori();
        }

        private void CheckHori()
        {
            for (int i = 0; i < SuHoriFullFlattened.Count; i++)
            {
                List<string> su = SuHoriFullFlattened.Skip(i * _rowsBlock * _colsBlock).Take(_rowsBlock * _colsBlock).ToList();

                for (int j = 0; j < su.Count; j++)
                {

                }
            }
        }

        public void SaveNSolve()
        {
            this.GameState = SodukuGameState.EditModeDone_Or_SolvingBegin;

            Solve();

            this.GameState = SodukuGameState.SolvingDone;
        }

        public static string GetBlockId(int x, int y) => string.Format(_blockIdPrefix, x, y);

        public static string GetCellId(string blockId, int x, int y) => string.Format(_cellIdPrefix, blockId, x, y);


        public List<string> SuBlockFullFlattened
        {
            get
            {
                return _suBlockFullFlattened;
            }
            private set
            {
                _suBlockFullFlattened = value;
            }
        }
        List<List<string>> SuBlockFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock; i++)
                    for (int j = 0; j < _colsBlock; j++)
                    {
                        List<string> list = GetSuBlock(i, j);
                        retVal.Add(list);
                    }

                return retVal;
            }
        }
        // Get Solving Unit - for a block
        List<string> GetSuBlock(int rowNoBlock, int colNoBlock)
        {
            List<string> retVal = new List<string>();

            string blockId = string.Format(_blockIdPrefix, rowNoBlock, colNoBlock);

            for (int i = 0; i < _rowsBlock; i++)
                for (int j = 0; j < _colsBlock; j++)
                {
                    string id = string.Format(_cellIdPrefix, blockId, j, i);
                    //Console.WriteLine($"ID = {id}");
                    retVal.Add(id);
                }


            return retVal;
        }

        public List<string> SuHoriFullFlattened
        {
            get
            {
                return _suHoriFullFlattened;
            }
            private set
            {
                _suHoriFullFlattened = value;
            }
        }
        List<List<string>> SuHoriFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock * _colsBlock; i++)
                {
                    //if(i == 1)
                    {
                        List<string> list = GetSuHori(i);
                        retVal.Add(list);
                    }
                }

                return retVal;
            }
        }
        // Get Solving Unit - Horigontal list
        List<string> GetSuHori(int rowNoSoduku)
        {
            List<string> retVal = new List<string>();

            int p = (int)(rowNoSoduku / _colsBlock);
            int r = (int)(rowNoSoduku % _colsBlock);

            for (int j = 0; j < _rowsBlock * _colsBlock; j++)
            {
                int q = (int)(j / _rowsBlock);
                int s = (int)(j % _rowsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(_cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }

        public List<string> SuVertFullFlattened
        {
            get
            {
                return _suVertFullFlattened;
            }
            private set
            {
                _suVertFullFlattened = value;
            }
        }
        List<List<string>> SuVertFull
        {
            get
            {
                List<List<string>> retVal = new();

                for (int i = 0; i < _rowsBlock * _colsBlock; i++)
                {
                    //if(i == 1)
                    {
                        List<string> list = GetSuVert(i);
                        retVal.Add(list);
                    }
                }

                return retVal;
            }
        }
        // Get Solving Unit - Vertical list
        List<string> GetSuVert(int colNoSoduku)
        {
            List<string> retVal = new List<string>();

            int q = (int)(colNoSoduku / _rowsBlock);
            int s = (colNoSoduku % _rowsBlock);

            for (int i = 0; i < _rowsBlock * _colsBlock; i++)
            {
                int p = (int)(i / _colsBlock);
                int r = (int)(i % _colsBlock);

                string blockId = GetBlockId(p, q);

                retVal.Add(string.Format(_cellIdPrefix, blockId, r, s));
            }

            return retVal;
        }
    }

    public enum SodukuGameState
    {
        CalculationPending = 0,
        CalculationDone_Or_EditModeBegin = 1,
        EditModeDone_Or_SolvingBegin = 2,
        SolvingDone = 3,
    }

    public class SodukuGameMove
    {
        public string CellId { get; set; } = "";
        string CellValue { get; set; } = "";

        public List<string> Hints { get; set; } = new();

        private int MaxHint { get; set; }

        public SodukuGameMoveMode MoveMode { get; set; } = SodukuGameMoveMode.Manual;

        public SodukuGameMove(string cellId, string cellValue, int maxHint /* Soduku block rows into size, 1 based */, SodukuGameMoveMode MoveMode = SodukuGameMoveMode.Manual)
        {
            CellId = cellId;
            CellValue = cellValue;

            MaxHint = maxHint;

            Hints = Enumerable.Range(1, MaxHint).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();

            if (Hints.Contains(CellValue))
                Hints = []; // If cell contains any value that can b hint then we remove all hints
        }

        public string GetCellValue() => CellValue;
        public void SetCellValue(string cellValue, bool resetHint /* true is complete reset, false is bring backremoved value */)
        {
            // cases
            // 1: old CellValue = "" , new cellValue = "", resetHint = false(NA)
            // 2: old CellValue = "" , new cellValue = "", resetHint = true(NA)
            // 3: old CellValue = "" , new cellValue = "3", resetHint = false(NA)
            // 4: old CellValue = "" , new cellValue = "3", resetHint = true(NA)
            // 5: old CellValue = "4", new cellValue = "", resetHint = false
            // 6: old CellValue = "4", new cellValue = "", resetHint = true
            // 7: old CellValue = "4", new cellValue = "3", resetHint = false
            // 8: old CellValue = "4", new cellValue = "3", resetHint = true

            // cases
            if (string.IsNullOrEmpty(CellValue) && string.IsNullOrEmpty(cellValue) && resetHint == false /* NA */ )
            {
                // 1: old CellValue = "" , new cellValue = "", resetHint = false(NA)
                // Nothing to do here
            }
            else if (string.IsNullOrEmpty(CellValue) && string.IsNullOrEmpty(cellValue) && resetHint == true /* NA */ )
            {
                // 2: old CellValue = "" , new cellValue = "", resetHint = true(NA)
                // Nothing to do here
            }
            else if (string.IsNullOrEmpty(CellValue) && !string.IsNullOrEmpty(cellValue) && resetHint == false /* NA */ )
            {
                // 3: old CellValue = "" , new cellValue = "3", resetHint = false(NA)

                if (Hints.Contains(cellValue))
                    Hints = []; // If cell contains any value that can b hint then we remove all hints
            }
            else if (string.IsNullOrEmpty(CellValue) && !string.IsNullOrEmpty(cellValue) && resetHint == true /* NA */ )
            {
                // 4: old CellValue = "" , new cellValue = "3", resetHint = true(NA)

                if (Hints.Contains(cellValue))
                    Hints = []; // If cell contains any value that can b hint then we remove all hints
            }
            else if (!string.IsNullOrEmpty(CellValue) && string.IsNullOrEmpty(cellValue) && resetHint == false)
            {
                // 5: old CellValue = "4", new cellValue = "", resetHint = false

                //if (Hints.Contains(CellValue))
                Hints.Add(CellValue);
            }
            else if (!string.IsNullOrEmpty(CellValue) && string.IsNullOrEmpty(cellValue) && resetHint == true)
            {
                // 6: old CellValue = "4", new cellValue = "", resetHint = true

                //if (Hints.Contains(CellValue))
                Hints = Enumerable.Range(1, MaxHint).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();
            }
            else if (!string.IsNullOrEmpty(CellValue) && !string.IsNullOrEmpty(cellValue) && resetHint == false)
            {
                // 7: old CellValue = "4", new cellValue = "3", resetHint = false

                //if (Hints.Contains(CellValue))
                Hints.RemoveAll(x => x == CellValue);
                Hints.Add(cellValue);
            }
            else if (!string.IsNullOrEmpty(CellValue) && !string.IsNullOrEmpty(cellValue) && resetHint == true)
            {
                // 8: old CellValue = "4", new cellValue = "3", resetHint = true

                Hints = Enumerable.Range(1, MaxHint).Select(x => x.ToString(CultureInfo.InvariantCulture)).ToList();
                Hints.Add(cellValue);
            }

            CellValue = cellValue;

            if (Hints.Contains(CellValue))
                Hints = []; // If cell contains any value that can b hint then we remove all hints
        }

        public override string ToString()
        {
            string retVal = "";

            retVal += $"CellId    = {CellId}";
            retVal += " ";
            retVal += $"CellValue = {CellValue}";


            return retVal;
        }
    }

    public enum SodukuGameMoveMode
    {
        Manual,
        Automatic,
    }

    internal class CellIdValueField(int size)
    {
        //int _size = size;

        public int Size { get => _inputVal.Length; }

        public void Init(int size)
        {
            _inputVal = new string[size];
        }

        private string[] _inputVal = new string[size];

        //public string this[int index]
        //{
        //    get => Convert.ToInt32(_inputVal[index]) < 10 ? "10" : _inputVal[index];
        //    set => _inputVal[index] = Convert.ToInt32(value) < 10 ? "10" : Convert.ToInt32(value) > 500 ? "500" : value;
        //}

        public string this[int index]
        {
            get
            {
                int retVal;

                Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (index < 0 || index >= size)
                {
                    return "";
                }
                else if (string.IsNullOrEmpty(_inputVal[index]))
                {
                    return "";
                }
                else if (int.TryParse(_inputVal[index], out retVal) == false)
                {
                    return "";
                }
                else
                {
                    if (retVal > 16)
                        retVal = 16;
                    else if (retVal < 1)
                        retVal = 0;
                }
                return retVal == 0 ? "" : retVal.ToString(CultureInfo.InvariantCulture);
            }
            set
            {

                int retVal = 0;

                Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (index < 0 || index >= size)
                {
                    _inputVal[index] = "";
                }
                else if (string.IsNullOrEmpty(value))
                {
                    _inputVal[index] = "";
                }
                else if (int.TryParse(value, out retVal) == false)
                {
                    _inputVal[index] = "";
                }
                else
                {
                    if (retVal > 16)
                        retVal = 16;
                    else if (retVal < 1)
                        retVal = 0;
                }

                _inputVal[index] = retVal == 0 ? "" : retVal.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
