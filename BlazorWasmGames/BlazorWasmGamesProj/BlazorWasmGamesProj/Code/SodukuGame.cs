
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace BlazorWasmGamesProj.Code
{


    internal class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        private List<string> _suBlockFullFlattened = new();
        private List<string> _suHoriFullFlattened = new();
        private List<string> _suVertFullFlattened = new();
        //private static List<string> _hints = new();
        public Dictionary<string, SodukuCellInfo> Positions = [];



        public SodukuGameState GameState { get; private set; } = SodukuGameState.CalculationPending;

        public SodukuGame(int rowsBlock, int colsBlock)
        {
            ReInit(rowsBlock, colsBlock);
        }

        public void ReInit(int rowsBlock, int colsBlock)
        {
            _rowsBlock = rowsBlock;
            _colsBlock = colsBlock;

            _suBlockFullFlattened = SuBlockFull.Flatten();
            _suHoriFullFlattened = SuHoriFull.Flatten();
            _suVertFullFlattened = SuVertFull.Flatten();

            Positions = _suHoriFullFlattened.Select(x => new KeyValuePair<string, SodukuCellInfo>(x, new() { CellId = x, CellValue = new(rowsBlock * colsBlock), PositionType = PositionTypeEnum.None }))
                .ToDictionary(t => t.Key, t => t.Value);

            GameState = SodukuGameState.CalculationPending;
        }

        public void Recalculate()
        {
            _suBlockFullFlattened = SuBlockFull.Flatten();
            _suHoriFullFlattened = SuHoriFull.Flatten();
            _suVertFullFlattened = SuVertFull.Flatten();

            Positions = _suHoriFullFlattened.Select(x => new KeyValuePair<string, SodukuCellInfo>(x, new() { CellId = x, CellValue = new(_rowsBlock * _colsBlock), PositionType = PositionTypeEnum.None }))
                .ToDictionary(t => t.Key, t => t.Value);


            GameState = SodukuGameState.CalculationDone_Or_EditModeBegin;
        }

        void Solve()
        {
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

    internal class CellIdValueField1
    {
        int _maxValue;
        string _value;

        public CellIdValueField1(int maxValue)
        {
            _maxValue = maxValue;
            _value = "";
        }

        public string Val
        {
            get
            {
                int retVal;

                //Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (string.IsNullOrEmpty(_value))
                {
                    return "";
                }
                else if (int.TryParse(_value, out retVal) == false)
                {
                    return "";
                }
                else
                {
                    if (retVal < 1)
                        retVal = 0;
                    else if (retVal > _maxValue)
                        retVal = _maxValue;
                }

                string result = "";

                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else
                    result = "";

                Console.WriteLine($"GET result = {result}");
                return result;
            }
            set
            {

                int retVal = 0;

                //Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

                if (string.IsNullOrEmpty(value))
                {
                    _value = "";
                }
                else if (int.TryParse(value, out retVal) == false)
                {
                    _value = "";
                }
                else
                {
                    //if (retVal > _maxValue)
                    //    retVal = _maxValue;
                    //else if (retVal < 1)
                    //    retVal = 0;

                    if (retVal < 1)
                        retVal = 0;
                    else if (retVal > _maxValue)
                        retVal = _maxValue;
                }
                string result = "";

                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else result = "";

                _value = result;

                Console.WriteLine($"SET result = {result}");
            }
        }
    }



    internal class CellIdValueField(int size, int maxValue)
    {
        //int _size = size;
        int MaxValue
        {
            get => maxValue;
            set => value = maxValue;
        }

        public int Size { get => _inputVal.Length; }

        public void Init(int size, int maxValue)
        {
            MaxValue = maxValue;
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

                //Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

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
                    if (retVal > maxValue)
                        retVal = maxValue;
                    else if (retVal < 1)
                        retVal = 0;
                }
                return retVal == 0 ? "" : retVal.ToString(CultureInfo.InvariantCulture);
            }
            set
            {

                int retVal = 0;

                //Console.WriteLine($"Index = {index} and _inputVal Size = {_inputVal.Length}");

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
                    if (retVal > maxValue)
                        retVal = maxValue;
                    else if (retVal < 1)
                        retVal = 0;
                }

                _inputVal[index] = retVal == 0 ? "" : retVal.ToString(CultureInfo.InvariantCulture);
            }
        }
    }


    internal class SodukuCellInfo
    {
        public string CellId { get; set; } = "";
        //public string CellValue { get; set; } = "";

        public PositionTypeEnum PositionType { get; set; } = PositionTypeEnum.Manual;

        public CellIdValueField1 CellValue { get; init; } = new(0);
    }

    internal enum PositionTypeEnum
    {
        None,
        Manual,
        Solve,
    }
}
