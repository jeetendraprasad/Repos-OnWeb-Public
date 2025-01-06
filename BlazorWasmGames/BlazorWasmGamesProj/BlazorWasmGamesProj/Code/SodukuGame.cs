
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace BlazorWasmGamesProj.Code
{


    internal class SodukuGame
    {
        int _rowsBlock = 0, _colsBlock = 0;

        const string _blockIdPrefix = "B[{0},{1}]";                // block names are B[{0-based rows},{0-based cols}]
        const string _cellIdPrefix = "{0}:C[{1},{2}]";             // cell names are BLOCK_ID:[{0-based rows},{0-based cols}]

        private List<string> _suBlockFullFlattened = [];
        private List<string> _suHoriFullFlattened = [];
        private List<string> _suVertFullFlattened = [];
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

            Positions = _suHoriFullFlattened.Select(x => new KeyValuePair<string, SodukuCellInfo>(x,
                new SodukuCellInfo(cellId: x, positionType: PositionTypeEnum.None, maxCellValue: _rowsBlock * _colsBlock, "")
            ))
                .ToDictionary(t => t.Key, t => t.Value);

            GameState = SodukuGameState.CalculationPending;

            //KeyValuePair<string, SodukuCellInfo> a = Positions.First();
        }

        public void Recalculate()
        {
            _suBlockFullFlattened = SuBlockFull.Flatten();
            _suHoriFullFlattened = SuHoriFull.Flatten();
            _suVertFullFlattened = SuVertFull.Flatten();

            Positions = _suHoriFullFlattened.Select(x => new KeyValuePair<string, SodukuCellInfo>(x,
                                new SodukuCellInfo(cellId: x, positionType: PositionTypeEnum.None, maxCellValue: _rowsBlock * _colsBlock, "")
                ))
                .ToDictionary(t => t.Key, t => t.Value);


            GameState = SodukuGameState.CalculationDone_Or_EditModeBegin;
        }

        void Solve()
        {
            //int anySolvedPosition = Positions.Where(item => item.Value.CellValueValAsInt > 0).ToList().Count;
            int automaticSolvedPosition_AtStart = Positions.Where(item => item.Value.CellValueValAsInt > 0 && item.Value.GetPositionType() == PositionTypeEnum.Solve).ToList().Count;

            CheckHori();
            //CheckVert();
            //CheckBlock();

            //Dictionary<string, int> dict = new Dictionary<string, int>();
            //dict = dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value + 1);

            //mydict.Keys.ToList().ForEach(k => mydict[k] = false);

            foreach (string key in Positions.Keys.ToList())
            {
                SodukuCellInfo cellInfo = Positions[key];

                if (cellInfo.Hints.Count == 1 && cellInfo.CellValueValAsInt > 0)
                {
                    cellInfo.CellValueVal = cellInfo.Hints[0].ToString(CultureInfo.InvariantCulture);
                    cellInfo.SetPositionType(PositionTypeEnum.Solve);
                }
            }

            int manualSolvedPosition_AtEnd = Positions.Where(item => item.Value.CellValueValAsInt > 0 && item.Value.GetPositionType() == PositionTypeEnum.Manual).ToList().Count;

            if (automaticSolvedPosition_AtStart - manualSolvedPosition_AtEnd > 0)
                Console.WriteLine("We have dome some automatic solving.");

        }

        private void CheckHori()
        {
            for (int i = 0; i < SuHoriFullFlattened.Count; i++)
            {
                List<string> su = SuHoriFullFlattened.Skip(i * _rowsBlock * _colsBlock).Take(_rowsBlock * _colsBlock).ToList();

                CheckInternal(su);
            }
        }

        private void CheckVert()
        {
            for (int i = 0; i < SuVertFullFlattened.Count; i++)
            {
                List<string> su = SuVertFullFlattened.Skip(i * _rowsBlock * _colsBlock).Take(_rowsBlock * _colsBlock).ToList();

                CheckInternal(su);
            }
        }

        private void CheckBlock()
        {
            for (int i = 0; i < SuBlockFullFlattened.Count; i++)
            {
                List<string> su = SuBlockFullFlattened.Skip(i * _rowsBlock * _colsBlock).Take(_rowsBlock * _colsBlock).ToList();

                CheckInternal(su);
            }
        }

        private void CheckInternal(List<string> su)
        {

            Console.WriteLine($"Solving for unit \r\n {JsonSerializer.Serialize(su ?? [])}");

            for (int j = 0; j < su.Count; j++)
            {
                string our = su[j];
                List<string> others = su.Where(x => x != su[j]).ToList(); // to do add value check

                if (Positions[our].CellValueValAsInt != 0)
                    foreach (string other in others)
                    {
                        if (Positions[other].CellValueValAsInt > 0)
                        {
                            Console.WriteLine($"From position {our} removing hint {Positions[other].CellValueValAsInt}");
                            Positions[our].RemoveHint(Positions[other].CellValueValAsInt);
                        }
                    }
                else
                {
                    Console.WriteLine($"For position {our} removing all hints");
                    Positions[our].Hints.Clear();
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
                List<List<string>> retVal = [];

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
            List<string> retVal = [];

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
                List<List<string>> retVal = [];

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
            List<string> retVal = [];

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
                List<List<string>> retVal = [];

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
            List<string> retVal = [];

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

    internal class CellIdValueField1(int maxValue)
    {
        readonly int _maxValue = maxValue;
        string? _value = "";

        public string? Val
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

                string? result;
                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else
                    result = null;

                //Console.WriteLine($"GET result = {(result == null ? "null" : result)} and _value = {_value}");
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
                    if (retVal < 1)
                        retVal = 0;
                    else if (retVal > _maxValue)
                        retVal = _maxValue;
                }
                string? result;

                if (retVal != 0)
                    result = retVal.ToString(CultureInfo.InvariantCulture);
                else
                    result = null;

                _value = result;

                //Console.WriteLine($"SET result = {(result == null ? "null" : result)} and value = {value}");
            }
        }
    }



    internal class CellIdValueField(int size, int maxValue)
    {
        private readonly string[] _inputVal = new string[size];

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
        public SodukuCellInfo(string cellId, PositionTypeEnum positionType, int maxCellValue, string cellValue)
        {
            _cellId = cellId;
            _positionType = positionType;
            _cellValueField1 = new(maxCellValue)
            {
                Val = cellValue
            };

            _hints = Enumerable.Range(1, maxCellValue).ToList();
        }

        string _cellId = "";

        PositionTypeEnum _positionType;

        public void SetPositionType(PositionTypeEnum positionType)
        {
            _positionType = positionType;
        }

        public PositionTypeEnum GetPositionType()
        {
            return _positionType;
        }

        CellIdValueField1 _cellValueField1;

        public int CellValueValAsInt
        {
            get
            {
                string? strVal = CellValueVal;
                if (strVal == null)
                    return 0;
                int intVal = 0;
                if (int.TryParse(strVal, out intVal) == true)
                {
                    return intVal;
                }
                else
                    return 0;
            }
        }

        public string? CellValueVal
        {
            get { return _cellValueField1.Val; }
            set { _cellValueField1.Val = value; }
        }

        readonly List<int> _hints = [];

        public List<int> Hints
        {
            get
            {
                int intVal = 0;
                _ = int.TryParse(CellValueVal, out intVal);
                Console.WriteLine($"CellValueVal = {(CellValueVal == null ? "null" : CellValueVal)}");

                if (intVal > 0)
                {
                    Console.WriteLine($"CellValueVal {CellValueVal} converted to int is greater than 0");
                    return [];
                }
                else
                    return _hints;
            }
        }

        public void RemoveHint(int num)
        {
            _hints.Remove(num);
        }
    }

    internal enum PositionTypeEnum
    {
        None,
        Manual,
        Solve,
    }
}
